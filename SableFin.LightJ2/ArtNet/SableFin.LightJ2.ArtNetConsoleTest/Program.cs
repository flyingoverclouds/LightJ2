using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SableFin.LightJ2.ArtNet;

namespace SableFin.LightJ2.ArtNetConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** ArtNet UDP receiver ***");
            Run();
            Console.WriteLine("Press enter to EXIT");
            Console.ReadLine();
        }

        static async void Run()
        {
            var engine = new ArtNetEngine();
            engine.Start();
        }
    }
}
