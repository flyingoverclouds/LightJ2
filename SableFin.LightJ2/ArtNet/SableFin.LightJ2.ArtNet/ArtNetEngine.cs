using SableFin.LightJ2.ArtNet.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNet
{
   
    public class ArtNetEngine
    {
        const short artNetPort = 6454;
        public ArtNetEngine()
        {
            
        }

        UdpClient artnetUdpClient = null;
        IPEndPoint groupEP = null;

        Thread thEngine = null;

        public void Start()
        {
            if (thEngine?.ThreadState == ThreadState.Running)
                return;
            thEngine?.Abort();
            thEngine = new Thread(new ThreadStart(Run));
            thEngine.Start();
        }

        async void Run()
        {
            artnetUdpClient = new UdpClient(artNetPort);
            groupEP = new IPEndPoint(IPAddress.Any, artNetPort);

            ArtNetPacket anpacket = null;
            try
            {
                while (true)
                {
                    Console.WriteLine("...");
                    byte[] bytes = artnetUdpClient.Receive(ref groupEP);
                    anpacket = await ArtNetPacket.Unpack(groupEP,bytes);
                    Console.WriteLine("Received broadcast from {0} :\n{1}\n", groupEP.ToString(), anpacket?.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION : " + ex.ToString());
            }
            finally
            {
                artnetUdpClient.Close();
            }
        }
   


        public void Stop()
        {
            thEngine?.Abort();
            thEngine?.Join();
            
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("artNetPort={0}\r\n", artNetPort);
            sb.AppendFormat("IsListening={0}\r\n",thEngine.ThreadState==ThreadState.Running);
            return sb.ToString();
        }
    }
}
