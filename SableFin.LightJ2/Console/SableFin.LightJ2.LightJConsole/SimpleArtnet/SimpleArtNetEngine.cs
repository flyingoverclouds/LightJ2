using SableFin.LightJ2.LightJConsole.SimpleArtnet.Packet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet
{
    /// <summary>
    /// Simmplet arnet engine
    /// Send Poll packet, handle PollReply and maintain a list of discovered ArtNet device with pertinent data (available universe),
    /// send DMX frame to a specific universe 
    /// </summary>
    public class SimpleArtNetEngine : IDisposable
    {
        public const short ArtNetUDPPort = 6454;

        private string _engineName;

        ArtNetPoller poller = null;
        ArtNetDmxSender dmxSender = null;

        Action<Device> devicePolledCallback=null;

        Action<byte[], int> copyDmxBufferCallback=null;

        public SimpleArtNetEngine(Action<Device> calledOnPollReplyCallback=null, 
            Action<byte[], int> copyDmxBufferCallback=null,
            string deviceName = "SimpleArtNetEngine")
        {
            this.devicePolledCallback = calledOnPollReplyCallback;
            this.copyDmxBufferCallback = copyDmxBufferCallback;
            this._engineName = deviceName;
        }

        /// <summary>
        /// Start the artnet engine. 
        /// Throw excpetion if start failed
        /// </summary>
        public void Start()
        {
            if (poller == null)
            {
                ArtNetPoller.DelayBetweenPoll = 30000;
                poller = new ArtNetPoller();
                poller.Start();
            }
            if (dmxSender==null)
            {
                dmxSender = new ArtNetDmxSender(copyDmxBufferCallback);
                dmxSender.TargetNodeIp = null; // no artnet node selected
                dmxSender.Start();
            }
            StartListener();
        }


        /// <summary>
        /// Stop the current ArtNet engine. NEver failed
        /// </summary>
        public void Stop()
        {
            try
            {
                if (poller != null)
                {
                    poller.Stop();
                    poller = null;
                }
                if (dmxSender != null)
                {
                    dmxSender.Stop();
                    dmxSender = null;
                }
                StopListener();
            }
            catch (Exception ex)
            {
                // TODO : log excpetion in Stop
                Debug.WriteLine($"EXCEPTION in SimplateArtNetEngine.Stop() : {ex.ToString()}");
            }

        }


        #region ArtNet UDP listener (poll reply)
        DatagramSocket artnetSocket = null;
        async void StartListener()
        {
            artnetSocket = new DatagramSocket();
            artnetSocket.Control.MulticastOnly = true;       
            artnetSocket.Control.QualityOfService = SocketQualityOfService.LowLatency;

            artnetSocket.MessageReceived += artnetSocket_MessageReceived;
            await artnetSocket.BindServiceNameAsync($"{ArtNetUDPPort}");

        }

        void StopListener()
        {
            if (artnetSocket != null)
                artnetSocket.Dispose();
            artnetSocket = null;
        }


        async private void artnetSocket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                //Debug.WriteLine($"Message from {args.RemoteAddress}:{args.RemotePort} to {args.LocalAddress}:{ArtNetUDPPort}");
                byte[] buffer = new byte[args.GetDataReader().UnconsumedBufferLength];
                args.GetDataReader().ReadBytes(buffer);
                var t = new Task(ParsePacket, buffer);
                t.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EXCEPTION IN SimpleArtNetEngine.artnetSocket_MessageREceived() : {ex.ToString()}");
            }
        }

        async void ParsePacket(object state)
        {
            if (poller.Pause)
                return;
            poller.Pause = true;
            var buffer = (byte[])state;
            if (!Packet.PacketBuilder.IsArtNetPacket(buffer))
            {
                //Debug.WriteLine("    NOT ART-NET");
                return;
            }
            var opcode = Packet.PacketBuilder.GetOpCode(buffer);
            //Debug.WriteLine("    Art-Net packet.");
            switch (opcode)
            {
                case Packet.ArtNetOpCode.OpPoll:
                    //Debug.WriteLine("    Art-Net : OpPoll");
                    var pktPoll = PacketParser.ParsePoll(buffer);
                    poller.Pause = false;
                    break;
                case Packet.ArtNetOpCode.OpPollReply:
                    //Debug.WriteLine("    Art-Net : OpPollReply");
                    var pktPollReply = PacketParser.ParsePollReply(buffer);
                    poller.Pause = false;
                    RegisterDeviceAndUniverse(pktPollReply);
                    break;
                case Packet.ArtNetOpCode.OpDmx:
                    //Debug.WriteLine("    Art-Net : OpDmx");
                    //var pktDmx = PacketParser.ParseDmx(buffer);
                    break;
            }
        }
        #endregion

        #region ArtNet DMX send
        public async void SendDmx(byte[] dmxBuffer, string nodeIp, short nodePort= SimpleArtNetEngine.ArtNetUDPPort)
        {
            var dmxPacket = PacketBuilder.GetDmxPacket(dmxBuffer);

            try
            {
                using (var udpSocket = new DatagramSocket())
                {
                    udpSocket.Control.QualityOfService = SocketQualityOfService.LowLatency;
                    await udpSocket.ConnectAsync(new HostName(nodeIp), $"{nodePort}");
                    using (var writer = new DataWriter(udpSocket.OutputStream))
                    {
                        writer.WriteBytes(dmxPacket);
                        await writer.StoreAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EXCEPTION : SimpleArtNetEngine.SendDmx() : {ex.ToString()}");
            }
        }

        #endregion

        #region Universe list & device list management
        ObservableCollection<ArtNetDmxTarget> dmxTargets = new ObservableCollection<ArtNetDmxTarget>();

        void RegisterDeviceAndUniverse(Packet.ArtPollReply pktPollReply)
        {
            foreach (var n in pktPollReply.Ports)
            {
                Debug.WriteLine($"Register Device : {pktPollReply.IpNode}:{pktPollReply.IpPortNode}  netswitch={pktPollReply.NetSwitch} subswitch={pktPollReply.SubSwitch}   InputUniverse={n.InputUniverse}   OutputUniverse={n.OutputUniverse}");

                // HACK : directly set the IP of the node that pollReply
                if (dmxSender != null)
                    if (dmxSender.TargetNodeIp != pktPollReply.IpNode) // si nouvelle IP -> on l'affecte
                        dmxSender.TargetNodeIp = pktPollReply.IpNode;


                // TODO : dynamicly update the liste of detected Artnet Node

                //bool found = false;
                //foreach(var dt in dmxTargets)
                //{

                //}
                //if (!found)
                //{
                //    ArtNetDmxTarget adt = new ArtNetDmxTarget();
                //    adt.Name = pktPollReply.LongName;
                //    adt.ShortName = pktPollReply.ShortName;
                //    IPAddress adr;
                //    if (IPAddress.TryParse(pktPollReply.IpNode, out adr))
                //    {
                //        IPEndPoint
                //        adt.IpAddress.
                //    }
                //}
            }
        }
        
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
