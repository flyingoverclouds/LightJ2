using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.Network
{
    abstract class DmxServiceProxy
    {
        private string _errorMessage="";
        public string GetLastErrorMessage()
        {
            return _errorMessage;
        }

        
        public abstract Task SetChannelValue(short channel, byte value);

        public abstract Task SendSyncFrame();

        public abstract void Initialize();
    }
}
