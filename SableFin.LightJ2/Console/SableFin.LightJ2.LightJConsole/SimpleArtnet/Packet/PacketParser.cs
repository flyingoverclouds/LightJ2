using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet.Packet
{
    class PacketParser
    {
        public static ArtPoll ParsePoll(byte[] rawData)
        {
            //throw new NotImplementedException();

            ArtPoll packet = new ArtPoll(rawData);
            return packet;

        }
        public static ArtPollReply ParsePollReply(byte[] rawData)
        {
            //throw new NotImplementedException();

            ArtPollReply packet = new ArtPollReply(rawData);
            return packet;
        }

    }
}
