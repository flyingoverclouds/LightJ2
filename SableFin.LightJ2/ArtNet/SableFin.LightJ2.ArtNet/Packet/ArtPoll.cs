using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNet.Packet
{
    public class ArtPoll : ArtNetPacket
    {

        public ArtPoll(byte[] udpPayload) : base(ArtNetOpCode.OpPoll)
        {
            Unpack(udpPayload);
        }

        void Unpack(byte[] udpPayload)
        {
            int pver = udpPayload[11] + (udpPayload[10] << 8);
            this.ProtocolVersion = (short)pver;
            this.TalkToMe = udpPayload[12];
            this.Priority = udpPayload[13];
        }

        public short ProtocolVersion{ get; private set; }
        public byte TalkToMe { get; private set; }
        public byte Priority { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendFormat("ProtocolVersion : {0}\r\n",ProtocolVersion);
            sb.AppendFormat("TalktoMe: {0} [{1}]\r\n",TalkToMe,TalkToMe.ToTalkToMeString());
            sb.AppendFormat("Priority: {0}\r\n",Priority);
            return sb.ToString();
        }
        
    }
}

