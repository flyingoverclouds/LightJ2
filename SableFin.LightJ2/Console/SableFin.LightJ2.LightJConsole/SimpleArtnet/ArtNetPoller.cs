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
    class ArtNetPoller : IDisposable
    {
        /// <summary>
        /// Delayt between 2 Pool packet (in ms)
        /// Default is 2000ms
        /// </summary>
        static public int DelayBetweenPoll { get; set; } = 2000;


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

        CancellationTokenSource cancelTokenSource = null;
        
        
        public ArtNetPoller()
        {

        }

        public void Start()
        {
            if (cancelTokenSource!= null)
                throw new InvalidOperationException("already started. Stop it before restart it");
            cancelTokenSource= new CancellationTokenSource();
            var t = new Task(RunPollerTask);
            t.Start();
        }

        public void Stop()
        {
            if (cancelTokenSource == null)
                return;
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }

        bool _pause = false;



        async void RunPollerTask()
        {
            var pollPacket = Packet.PacketBuilder.GetPollPacket();

            using (var udpSocket = new DatagramSocket())
            {
                udpSocket.Control.QualityOfService = SocketQualityOfService.LowLatency;
                await udpSocket.ConnectAsync(new HostName("255.255.255.255"), $"{SimpleArtNetEngine.ArtNetUDPPort}");
                using (var writer = new DataWriter(udpSocket.OutputStream))
                {
                    try
                    {
                        while (true)
                        {
                            
                            cancelTokenSource.Token.ThrowIfCancellationRequested();
                            if (!_pause)
                            {
                                writer.WriteBytes(pollPacket);
                                await writer.StoreAsync();
                            }
                            await Task.Delay(DelayBetweenPoll);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"EXCEPTION : ArtNetPoller.RunPollerTask() : {ex.ToString()}");
                    }
                }
            }
        }
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
