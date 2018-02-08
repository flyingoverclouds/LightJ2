using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using SableFin.LightJ2.ArtNetToOpenDmxUsxbServer.Enttec;

namespace SableFin.LightJ2.ArtNetToOpenDmxUsxbServer
{

        /// <summary>
        /// This class implement the socket listener wich listen the receiving socket and forware DMS packet to the DMX Sender
        /// </summary>
        class DmxSocketListener
        {
            OpenDmxUsbInterface dmxBus = null;
            int listenPort = 0;

            /// <summary>
            /// Create a new socket listener for LuxMusicaDmxApp
            /// </summary>
            /// <param name="dmxbus">a class subclassing DmxBusBase to manage the physical DmxBus. If null, data are lost.</param>
            /// <param name="listenPort">TCP Port open for listening</param>
            public DmxSocketListener(OpenDmxUsbInterface dmxBus, int listenPort)
            {
                this.ShowValue = false;
                this.dmxBus = dmxBus;
                this.listenPort = listenPort;
            }


            private Thread listeningThread = null;
            private TcpListener currentListener = null;

            /// <summary>
            /// Starting listening the socket and forwarding data to DMX bus
            /// </summary>
            public bool StartListening()
            {
                if (currentListener != null)
                    return false;

                //IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                currentListener = new TcpListener(listenPort);
                currentListener.Start();

                // demarrage du thread
                listeningThread = new Thread(DoListeningThreadRun);
                listeningThread.Name = "DMXLISTENER";
                listeningThread.IsBackground = true;
                listeningThread.Start();
                return true;
            }

            void DoListeningThreadRun()
            {
                while (true)
                {
                    if (!currentListener.Pending())
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    TcpClient client = currentListener.AcceptTcpClient();
                    Console.WriteLine("New client from : " + client.Client.ToString());
                    Thread dmxStreamThread = new Thread(DoTcpClientThreadRun);
                    dmxStreamThread.IsBackground = true;
                    dmxStreamThread.Name = "DMXSTREAM";
                    dmxStreamThread.Start(client);
                }
            }


            void DoTcpClientThreadRun(object newClient)
            {
                TcpClient tcpClient = newClient as TcpClient;
                byte[] buffer = new byte[4];
                int receivedSize = 0;
                try
                {
                    using (var dataStream = tcpClient.GetStream())
                    {

                        while (dataStream.CanRead)
                        {

                            receivedSize = dataStream.Read(buffer, 0, 1); // lecteur octet par octet
                            if (receivedSize == 0)
                            {
                                Console.WriteLine("DMX client disconnect.");

                                break;
                            }
                            AddReceivedData(buffer, receivedSize);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    Console.WriteLine("Closing client connexion ");
                    tcpClient.Close();
                }
            }




            /// <summary>
            /// Stop listening the socket and close It.
            /// </summary>
            public bool StopListening()
            {
                if (currentListener == null)
                    return false;

                if (listeningThread != null)
                {
                    listeningThread.Abort();
                    listeningThread.Join();
                    listeningThread = null;
                }
                currentListener.Stop();
                currentListener = null;
                return true;
            }


            byte[] ipDmxFrame = new byte[4];
            private int currentNdx = 0;

            private int nbFF = 0;

            /// <summary>
            /// Cette méthode analayse le contenu des données recues
            /// </summary>
            void AddReceivedData(byte[] buffer, int size)
            {
                if (size == 0)
                    return;

                var data = buffer[0];
                if (data == 0xFF)
                    nbFF++;
                else
                    nbFF = 0;
                if (nbFF == 4) // on a recu 4 FF consécutif ==> on resynchronise tout
                {
                    ipDmxFrame[0] = 0;
                    ipDmxFrame[1] = 0;
                    ipDmxFrame[2] = 0;
                    ipDmxFrame[3] = 0;
                    currentNdx = 0;
                    nbFF = 0;
                    size = 0;
                    return;
                }

                ipDmxFrame[currentNdx++] = data;
                if (currentNdx == 4) // On a une trame complete
                {
                    ExecuteDmxFrame(ipDmxFrame);
                    ipDmxFrame[0] = 0;
                    ipDmxFrame[1] = 0;
                    ipDmxFrame[2] = 0;
                    ipDmxFrame[3] = 0;
                    currentNdx = 0;
                }
            }


            public bool ShowValue { get; set; }
            void ExecuteDmxFrame(byte[] dmxFrame)
            {
                short channel = (short)((dmxFrame[0] << 8) + dmxFrame[1]);
                if (channel > 512)
                {
                    // invalide DMX channel. ignoring frame
                    Console.WriteLine("Invalid DMX channel : " + channel.ToString());
                    return;
                }
                if (dmxFrame[2] != 0)
                {
                    // invalid gap value 
                    Console.WriteLine("Invalid 3rd gap value : " + dmxFrame[2].ToString());
                    return;
                }

                if (ShowValue)
                    Console.WriteLine("DMX[{0:D3}] = {1:D3}", channel, dmxFrame[3]);
            if (dmxBus != null)
            {
                dmxBus.SetDmxValue(channel - 1, dmxFrame[3]);
            }
            else
                    Console.WriteLine("Error : dmxBus==null !!!!");
            }

        }
}
