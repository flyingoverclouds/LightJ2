using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNet.Packet
{
    class ArtNetDmx : ArtNetPacket
    {
        public ArtNetDmx(byte[] udpPayload) : base(ArtNetOpCode.OpDmx)
        {
            Unpack(udpPayload);
        }

        void Unpack(byte[] udpPayload)
        {
            int pver = udpPayload[11] + (udpPayload[10] << 8);
        
        }

    }
}
