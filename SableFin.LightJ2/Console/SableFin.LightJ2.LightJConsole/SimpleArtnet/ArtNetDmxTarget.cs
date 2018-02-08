using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet
{
    /// <summary>
    /// Describe a single universe target
    /// </summary>
    class ArtNetDmxTarget
    {
        /// <summary>
        /// Name of ArtNet device
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Shortname of ArnetDmxTarget
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// IpAdress returned by the device for unicast addressing
        /// </summary>
        public IPAddress IpAddress { get; set; }

        /// <summary>
        /// MAC Ethernet Adress of node
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// Artnet NET
        /// </summary>
        public ushort Net { get; set; }

        /// <summary>
        /// Artnet SUBNET
        /// </summary>
        public ushort Subnet{ get; set; }

        /// <summary>
        /// Artnet UNIVERSE
        /// </summary>
        public ushort Universe { get; set; }

        public DateTime LastPollReply { get; set; }


    }
}
