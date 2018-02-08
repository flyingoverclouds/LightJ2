using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNetToOpenDmxUsxbServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var openDmx = new Enttec.OpenDmxUsbInterface();
            Console.WriteLine("Open OpenDMX interface ...");
            openDmx.Start();

            // demarrage du listener


            //var artnetListener = new MiniArtNet.ArtnetListener("ArtNet2OpenDmx", "Arnet to OpenDmx gateway - SableFin.Net", 0, 0, 1);
            //var taskStart = artnetListener.StartAsync();



            var dsl = new DmxSocketListener(openDmx, 10256);
            dsl.StartListening();
            Console.WriteLine("Listening for incoming DmxNet packet ....");
            PrintCommandHelp();
            // demarrage du test local
            while (true)
            {
                Console.Write("Channel (or command) : ");
                string s = Console.ReadLine();
                if (s.ToUpper()=="EXIT")
                {
                    return;
                }
                if (s.ToUpper() == "SHOW VALUE")
                {
                    Console.WriteLine("DMX Value are visible.");
                    dsl.ShowValue = true;
                    continue;
                }
                if (s.ToUpper() == "HIDE VALUE")
                {
                    Console.WriteLine("DMX Value are not visible.");
                    dsl.ShowValue = false;
                    continue;
                }
                Console.Write("Value : ");
                string v=string.Empty;
                try
                {
                    v = Console.ReadLine();
                    short channel = short.Parse(s);
                    byte value = byte.Parse(v);
                    openDmx.SetDmxValue(channel, value);
                }
                catch
                {
                    Console.WriteLine("ERROR : unable to set channel {0} to value {1}",s,v);
                }
                Console.WriteLine("");
            }
            Console.ReadLine();
            
        }


        bool CheckParameters()
        {
            return true;
        }

        void PrintParameterHelp()
        {
            Console.WriteLine("ArtNetToOpenDmxUSB v1.0  #  (c) Nicolas CLERC 2016 - (c) SableFin.Net");
            Console.WriteLine("    -univers 25 : Set the universe value to 25");
            Console.WriteLine("");
        }


        static void PrintCommandHelp()
        {
            Console.WriteLine("Accepted commande : ");
            Console.WriteLine("    SHOW VALUE : start printing DMX value when received");
            Console.WriteLine("    HIDE VALUE : stop printing DMX value when received");
            Console.WriteLine("    EXIT : terminate program");
            Console.WriteLine("    xx : DMX512 channel number to enter specific value");

        }
    }
}