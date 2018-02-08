using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet
{
    public class ArtNetDmxSender : IDisposable
    {
        // TODO : add support of targe net / Subnet / universe

        Action<byte[], int> copyDmxBufferCallback=null;
        public ArtNetDmxSender(Action<byte[], int> copyDmxBufferCallback)
        {
            this.copyDmxBufferCallback = copyDmxBufferCallback;
        }

        private string _targetNodeIp= null;
        public string TargetNodeIp {
            get {
                return _targetNodeIp;
            }
            set {
                _targetNodeIp = value;
            }
        }

        public short TargetNodePort { get; set; } = SimpleArtNetEngine.ArtNetUDPPort;


        public bool Pause
        {
            get
            {
                return _pause;
            }

            set
            {
                _pause = value;
            }
        }

        bool _pause = false;
        bool resetUdpDatagram = true;
        CancellationTokenSource cancelTokenSource = null;

        public void Start()
        {
            if (cancelTokenSource != null)
                throw new InvalidOperationException("already started. Stop it before restart it");
            cancelTokenSource = new CancellationTokenSource();
            var t = new Task(RunSenderTask);
            t.Start();
        }

        async void RunSenderTask()
        {
            var dmxPacket = Packet.PacketBuilder.GetDmxPacket(new byte[512]);

            while (true) // boucle infinie
            {
                resetUdpDatagram = false;
                if (string.IsNullOrEmpty(_targetNodeIp))
                {
                    await Task.Delay(1000);
                }
                else
                {
                    using (var udpSocket = new DatagramSocket()) // creation de la socjet udp
                    {
                        udpSocket.Control.QualityOfService = SocketQualityOfService.LowLatency;
                        await udpSocket.ConnectAsync(new HostName(_targetNodeIp), $"{this.TargetNodePort}");
                        using (var writer = new DataWriter(udpSocket.OutputStream))
                        {
                            try
                            {
                                while (!resetUdpDatagram) // tant que pas de reset de la socket
                                {
                                    cancelTokenSource.Token.ThrowIfCancellationRequested();
                                    if (!_pause)
                                    {
                                        if (copyDmxBufferCallback != null)
                                            copyDmxBufferCallback(dmxPacket, Packet.PacketBuilder.DmxPacketPayloadOffset);
                                        writer.WriteBytes(dmxPacket);
                                        await writer.StoreAsync();
                                    }
                                    await Task.Delay(40);
                                }
                                Debug.WriteLine($"Ending loop DMX sender to reset udpdatagram");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"EXCEPTION : ArtNetPoller.RunPollerTask() : {ex.ToString()}");
                            }
                        }
                    }
                }
            }
        }
        public void Stop()
        {
            if (cancelTokenSource == null)
                return;
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
