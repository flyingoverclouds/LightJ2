using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.Network
{
    /// <summary>
    /// Cette classe gère le dialogue avec le service DMX qui gère les interface.
    /// Si aucune interface 
    /// ACTUELLEMENT CLASSE FAKE
    /// </summary>
    class DebugDmxServiceProxy : DmxServiceProxy
    {
        override async public Task SetChannelValue(short channel, byte value)
        {
            Debug.WriteLine("DMXPROXY : [" + channel.ToString("D3") + "] = " + value.ToString("D3"));
        }

        public override async Task SendSyncFrame()
        {
            // nothing to do in an debug proxy
            Debug.WriteLine("DMXPROX : SyncFrame");
        }
        public override void Initialize()
        {

        }
    }
}
