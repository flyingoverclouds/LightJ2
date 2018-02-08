using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.Network
{
    static class DmxServiceProxyFactory
    {
        // create a debug proxy
        static DmxServiceProxy _proxy = new DebugDmxServiceProxy();

        static public DmxServiceProxy GetProxy()
        {
            return _proxy;
        }
    }
}
