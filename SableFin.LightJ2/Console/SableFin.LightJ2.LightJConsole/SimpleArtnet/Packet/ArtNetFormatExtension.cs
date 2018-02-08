using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet.Packet
{
    static class ArtNetFormatExtension
    {
        public static string ToTalkToMeString(this byte talkToMeValue)
        {
            StringBuilder sb = new StringBuilder();
            if ((talkToMeValue & 0x01) == 0)
                sb.Append("ArtPollReplyOnNodeChange,");
            else
                sb.Append("ArtPollReplyOnArtPoll,");

            if ((talkToMeValue & 0x02) == 0)
                sb.Append("DontSendMeDiagMsg,");
            else
                sb.Append("SendMeDiagMsg,");

            if ((talkToMeValue & 0x04) == 0)
                sb.Append("BroadcastDiagMsg,");
            else
                sb.Append("UnicastDiagMsg,");
            return sb.ToString().Trim(',');
        }
    }
}
