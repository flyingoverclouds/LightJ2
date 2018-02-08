using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.UI.Popups;

namespace SableFin.LightJ2.Network
{
    /// <summary>
    /// Implemente l'envoi des changement de valeur DMX vers l'agent LightJOpenDmx
    /// </summary>
    class LighJOpenDmxAgentServiceProxy : DmxServiceProxy
    {
        private string _hostname;
        private int _port;
        private volatile byte[] syncFrame;

        private StreamSocket cnxSvc = null;
        private Stream cnxSvcStream = null;

        public LighJOpenDmxAgentServiceProxy(string hostname,int port)
        {
            this._hostname = hostname;
            this._port = port;

            syncFrame = new byte[4];
            syncFrame[0] = 0xFF;
            syncFrame[1] = 0xFF;
            syncFrame[2] = 0xFF;
            syncFrame[3] = 0xFF;
        }

        public override async void Initialize()
        {
            await DoDmxServerReconnectionAsync();
        }


        private bool whileConnecting = false;
        private object whileconnectingLock = new Object();
        /// <summary>
        /// Start a reconnecion to the DMX service 
        /// </summary>
        /// <returns></returns>
        private async Task DoDmxServerReconnectionAsync()
        {
            lock (whileconnectingLock)
            {
                if (whileConnecting)
                    return;
                whileConnecting = true;
            }
            cnxSvcStream = null;
            try
            {
                cnxSvc = new StreamSocket();
                await cnxSvc.ConnectAsync(new HostName(_hostname), _port.ToString());
                cnxSvcStream = cnxSvc.OutputStream.AsStreamForWrite();
                Debug.WriteLine("CONNECTED TO service");
            }
            catch (Exception ex)
            {
                cnxSvcStream = null;    
                // si exception c'est la plupart du temps parce que le services n'est pas démarré ==> MODE SIMULATION 
                //new MessageDialog("Unable to connect on the OpenDmxUsb agent on " + _hostname + ":" + _port.ToString() +
                //                  "\r\n\r\nCheck your configuration and/or your network connectivity. \r\n\r\n LightJ will continue in simulator mode.",
                //    "NETWORK ERROR").ShowAsync();

                Debug.WriteLine($"Unable to connect on the OpenDmxUsb agent on {_hostname}:{_port}  Check your configuration and/or your network connectivity");
            }
            finally
            {
                lock (whileconnectingLock)
                {
                    whileConnecting = false;
                }
            }
        }

        public override async Task SetChannelValue(short channel, byte value)
        {
            byte[] frame = new byte[4];
            frame[0] = (byte)((channel >> 8) & 0xFF);
            frame[1] = (byte)(channel & 0xFF);
            frame[2] = 0;
            frame[3] = value;
            SendFrame(frame);
        }

        async Task SendFrame(byte[] frame)
        {
            if (cnxSvcStream == null)
            {
                DoDmxServerReconnectionAsync();
                return;
            }
            try
            {
                cnxSvcStream.Write(frame, 0, 4);
                cnxSvcStream.Flush();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while sending frame to socket !");
                cnxSvcStream = null;
                DoDmxServerReconnectionAsync();
            }
        }
        public override async Task SendSyncFrame()
        {
            if (cnxSvcStream == null)
                return;
            cnxSvcStream.Write(syncFrame, 0, 4);
        }

    }
}
