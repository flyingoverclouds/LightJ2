using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNet.Packet
{
    abstract public class ArtNetPacket
    {
        protected ArtNetPacket(ushort opCode)
        {
            this.OpCode = opCode;
        }

        /// <summary>
        /// unpack UDP payload
        /// </summary>
        /// <param name="udpPayLoad">udp binary payload</param>
        /// <returns>true if ArtNet packet, false othervise</returns>
        async public static Task<ArtNetPacket> Unpack(IPEndPoint src,byte[] udpPayLoad)
        {
            long tick = DateTime.UtcNow.Ticks;

            if (udpPayLoad.Length < 14)
                return null;

            if (!(udpPayLoad[0] == 'A' &&
                udpPayLoad[1] == 'r' &&
                udpPayLoad[2] == 't' &&
                udpPayLoad[3] == '-' &&
                udpPayLoad[4] == 'N' &&
                udpPayLoad[5] == 'e' &&
                udpPayLoad[6] == 't' &&
                udpPayLoad[7] == 0x00))
                return null;

            int opCode = udpPayLoad[9] << 8 + udpPayLoad[8];
            ArtNetPacket packet;
            // create ArtNetPacket based on opcode
            switch(opCode)
            {
                case ArtNetOpCode.OpPoll:
                    packet = new Packet.ArtPoll(udpPayLoad);
                    break;
                case ArtNetOpCode.OpPollReply:
                    packet = new Packet.ArtPollReply(udpPayLoad);
                    break;
                case ArtNetOpCode.OpDmx:
                    packet = new Packet.ArtNetDmx(udpPayLoad);
                    break;
                default:
                    packet = null;
                    break;
            }
            packet.ReceiveTime = tick;
            packet.IpFrom = src.Address.ToString();

            return packet;
        }

        /// <summary>
        /// Add the default information to the packet buffer
        /// DOES NOT CHECK BUFFER SIZE !!
        /// </summary>
        /// <param name="packetBuffer">packet buffer to fill</param>
        void Pack(byte[] packetBuffer)
        {

            // TODO : add Correct IpAdress

            // set the "Art-Net" Header
            packetBuffer[0] = 0x41; // 'A'
            packetBuffer[1] = 0x72; // 'r'
            packetBuffer[2] = 0x72; // 't'
            packetBuffer[3] = 0x2D; // '-'
            packetBuffer[4] = 0x4E; // 'N'
            packetBuffer[5] = 0x65; // 'e'
            packetBuffer[6] = 0x74; // 't'
            packetBuffer[7] = 0x00;

            // set the opcode
            packetBuffer[8] = (byte)(this.OpCode | 0xFF);
            packetBuffer[9] = (byte)((this.OpCode>>8) | 0xFF); ;            

        }

        /// <summary>
        /// Extracte a zerp termnated string from a byte buffer. 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected string ExtractZStringFromPayload(byte[] buffer, int start,int size)
        {
            
            StringBuilder sb = new StringBuilder();
            for(int n=start; n<start+ size;n++)
            {
                if (buffer[n] == 0x00)
                    break;
                sb.Append(Convert.ToChar(buffer[n]));
            }
            return sb.ToString();
        }

        public ushort OpCode { get; private set; }
      
        public string IpFrom { get; private set; }
   
        public long ReceiveTime { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ReceiveTime: {0} [ticks:{1}]\r\n", DateTime.FromBinary(this.ReceiveTime).ToString("u"),this.ReceiveTime);
            sb.AppendFormat("ArtNet: {0} (0x{1})\r\n" ,ArtNetOpCode.ToString(OpCode),OpCode.ToString("x"));
            sb.AppendFormat("IP Source: {0}", IpFrom).AppendLine();
            return sb.ToString();            
        }
    }
}
