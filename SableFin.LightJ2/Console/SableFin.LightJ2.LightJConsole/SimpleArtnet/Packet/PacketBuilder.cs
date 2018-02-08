using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet.Packet
{
    static class PacketBuilder
    {
        static public bool IsArtNetPacket(byte[] packet)
        {
            if (packet.Length < 10) // at lease "Art-Net\0" header + opCode
                return false;

            if (packet[0] == 'A' &&
              packet[1] == 'r' &&
              packet[2] == 't' &&
              packet[3] == '-' &&
              packet[4] == 'N' &&
              packet[5] == 'e' &&
              packet[6] == 't' &&
              packet[7] == 0x00)
                return true;

            return false;
        }

        static public int GetOpCode(byte[] arnetPacket)
        {
            int opCode = arnetPacket[9] << 8 + arnetPacket[8];
            return opCode;
        }

        static void FillArtNetHeader(byte[] packetToFill)
        {
            packetToFill[0] = (byte)'A';
            packetToFill[1] = (byte)'r';
            packetToFill[2] = (byte)'t';
            packetToFill[3] = (byte)'-';
            packetToFill[4] = (byte)'N';
            packetToFill[5] = (byte)'e';
            packetToFill[6] = (byte)'t';
            packetToFill[7] = 0x00;
        }


        /// <summary>
        /// return a buffer for the PollPacket
        /// </summary>
        /// <param name="talkToMeFlags">Flags for talkToMe byte (default : 0x06 : diagnostice message + artPollReplay)</param>
        /// <returns></returns>
        static public byte[] GetPollPacket(byte talkToMeFlags=0x06)
        {
            byte[] pollPacket = new byte[14];
            FillArtNetHeader(pollPacket);
            pollPacket[8] = 0x00; // opcode low
            pollPacket[9] = 0x20; // opcode high
            pollPacket[10] = 0x00; // prot version low
            pollPacket[11] = 0x0e; // prot version high
            pollPacket[12] = 0x06;  // talk to me : diagnostic message, artPollReply
            pollPacket[13] = 0x00;  // priority
            return pollPacket;
        }

        public const int DmxPacketPayloadOffset = 18;

        static public byte[] GetDmxPacket(byte[] dmxValues)
        {
            // TODO : add parameters for net/subnet/universe

            byte[] pollPacket = new byte[18 + dmxValues.Length]; // artnet header + packet metadata + dmx values
            FillArtNetHeader(pollPacket);
            pollPacket[8] = 0x00; // opcode low OK
            pollPacket[9] = 0x50; // opcode high OK
            pollPacket[10] = 0x00; // prot version low OK
            pollPacket[11] = 0x0e; // prot version high OK

            pollPacket[12] = 0x00;  // sequence : 0x00 = sequence desactivated
            pollPacket[13] = 0x00;  // physical : physical input port. information only.
            pollPacket[14] = 0x00; // SubUni : low byte of th 15bit port-address to which packet is destinated
            pollPacket[15] = 0x00; // Net : the top 7bits of the 15bits port-address to which packet is destinated

            pollPacket[16] = (byte) ((dmxValues.Length >> 8) & 0xFF); // Length Hi : high byte of length of DMX packet
            pollPacket[17] = (byte) (dmxValues.Length & 0xFF); // length low: low byte of length of dmx packet

            Array.Copy(dmxValues, 0, pollPacket, DmxPacketPayloadOffset, dmxValues.Length);

            return pollPacket;
        }

    }
}
