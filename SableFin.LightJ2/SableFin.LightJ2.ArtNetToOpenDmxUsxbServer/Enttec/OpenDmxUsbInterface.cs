using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace SableFin.LightJ2.ArtNetToOpenDmxUsxbServer.Enttec
{

    public class OpenDmxUsbInterface
    {

        public static byte[] buffer = new byte[513];
        
        public static bool done = false;
        public static int bytesWritten = 0;
        public static FT_STATUS status;

        public const byte BITS_8 = 8;
        public const byte STOP_BITS_2 = 2;
        public const byte PARITY_NONE = 0;
        public const UInt16 FLOW_NONE = 0;
        public const byte PURGE_RX = 1;
        public const byte PURGE_TX = 2;

        public OpenDmxUsbInterface()
        {
            
        }
     

        /// <summary>
        /// Démare le thread d'envoie des données au port OpenDMX
        /// </summary>
        public void Start()
        {
           
            Thread thread = new Thread(new ThreadStart(OpenFtdiDeviceThenWaitAndWrite));
            thread.Name = "DMX frame sender loop";
            thread.IsBackground = true;
            thread.Start();
            SetDmxValue(0, 0);  //Set DMX Start Code
        }

        /// <summary>
        /// Positionne la valeur sur le canal DMX souhaité
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        public void SetDmxValue(int channel, byte value)
        {
            if (buffer != null)
            {
                buffer[channel + 1] = value;
            }
        }


        void OpenFtdiDeviceThenWaitAndWrite()
        {
            Console.WriteLine("Starting DMX Frame sender thread : id {0} [{1}]", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
            uint handle = 0;
            try
            {
                status = FtdiDevice.FT_Open(0, ref handle).CheckStatus("FT_Open()");
            }
            catch (System.DllNotFoundException dllExc)
            {
                Console.WriteLine("DllException : " + dllExc.Message);
                Console.WriteLine("OpenDMX serial chipset driver may not be installed in your system !");
                Console.WriteLine("You can download the latest version here : http://www.ftdichip.com/Drivers/D2XX.htm");
                Console.WriteLine("Your DMX interface can not be used, is not correctly installed or is not recognized.");
                return;
            }
            if (status != FT_STATUS.FT_OK)
            {
                if (status == FT_STATUS.FT_DEVICE_NOT_FOUND)
                {
                    Console.WriteLine("No OpenDMX Interface.");
                    return;
                }
                else
                {
                    Console.WriteLine($"ERROR : unknow error in FT_Open() : {status}");
                }
            }
            Console.WriteLine("OpenDMX interface found.");
            WriteDmxDataLoop(handle);
           
        }

        // Cette méhtode itère et envoyé les données le buffer en continue sur le port DMX
        void WriteDmxDataLoop(uint handle)
        {
            while (!done)
            {
                InitOpenDMX(handle);
                FtdiDevice.FT_SetBreakOn(handle).CheckStatus("FT_SetBreakOn()");
                FtdiDevice.FT_SetBreakOff(handle).CheckStatus("FT_SetBreakOff()");
                bytesWritten = write(handle, buffer, buffer.Length);
                Thread.Sleep(20);
            }
        }

        // Emission d'une trame en DMX
        int write(uint handle, byte[] data, int length)
        {
            IntPtr ptr = Marshal.AllocHGlobal((int)length);
            Marshal.Copy(data, 0, ptr, (int)length);
            uint bytesWritten = 0;
            status = FtdiDevice.FT_Write(handle, ptr, (uint)length, ref bytesWritten).CheckStatus("FT_Write()");
            return (int)bytesWritten;
        }

        /// <summary>
        /// Initialise la configuration du port OpenDMX.
        /// </summary>
        void InitOpenDMX(uint handle)
        {
            var status = FtdiDevice.FT_ResetDevice(handle).CheckStatus("FT_ResetDevice()");
            status = FtdiDevice.FT_SetDivisor(handle, (char)12).CheckStatus("FT_SetDivisor()");  // set baud rate
            status = FtdiDevice.FT_SetDataCharacteristics(handle, BITS_8, STOP_BITS_2, PARITY_NONE).CheckStatus("FT_SetDataCharacteristics()");
            status = FtdiDevice.FT_SetFlowControl(handle, (char)FLOW_NONE, 0, 0).CheckStatus("FT_SetFlowControl()");
            status = FtdiDevice.FT_ClrRts(handle).CheckStatus("FT_ClrRts()");
            status = FtdiDevice.FT_Purge(handle, PURGE_TX).CheckStatus("FT_Purge() 1");
            status = FtdiDevice.FT_Purge(handle, PURGE_RX).CheckStatus("FT_Purge() 2");
        }
    }

   

}
