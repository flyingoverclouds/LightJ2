using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNetToOpenDmxUsxbServer.MiniArtNet
{
    class ArtnetListener : IDisposable
    {
        public const int ArtnetDefaultPort = 6454;

        string longName;
        string shortName;
        byte netSwitch, subSwitch, universe;


        byte[] artPollReplyPacket;

        public ArtnetListener(string shortName, string longName, byte netswitch, byte subswitch, byte universe)
        {
            this.longName = (longName + "\0                                                                      ").Substring(0,63);
            this.shortName = (shortName + "\0                    ").Substring(0, 17); ;
            this.netSwitch = netswitch;
            this.subSwitch = subswitch;
            this.universe = universe;

            PreBuildingPackets();
        }


        void PreBuildingPackets()
        {
            // opPollReply packet
            // http://art-net.org.uk/?page_id=570
            artPollReplyPacket = new byte[239];
            FillArtNetHeader(artPollReplyPacket);
            artPollReplyPacket[8] = 0x00; // opcode LO
            artPollReplyPacket[9] = 0x21; // opcode HI
            artPollReplyPacket[10] = 127; // ip address of the node
            artPollReplyPacket[11] = 0;  // ip address of the node
            artPollReplyPacket[12] = 0;  // ip address of the node
            artPollReplyPacket[13] = 1;  // ip address of the node
            artPollReplyPacket[14] = 0x36; // IP port LO
            artPollReplyPacket[15] = 0x19; // IP port HI
            artPollReplyPacket[16] = 0;     // version LO
            artPollReplyPacket[17] = 14; // version HI
            artPollReplyPacket[18] = netSwitch;
            artPollReplyPacket[19] = subSwitch;
            artPollReplyPacket[20] = 0; // OEM HI - unknow oem
            artPollReplyPacket[21] = 0xff; // OEM LO - unknow oem
            artPollReplyPacket[22] = 0; // UbeaVersion
            artPollReplyPacket[23] = 0xC4; // Status1  // TO CHECK !!!!
            artPollReplyPacket[24] = Convert.ToByte('N'); // ESTA manuf code HI
            artPollReplyPacket[25] = Convert.ToByte('C'); // ESTA manuf code LO
            System.Text.Encoding.ASCII.GetBytes(shortName + "\0").CopyTo(artPollReplyPacket, 26);
            System.Text.Encoding.ASCII.GetBytes(longName + "\0").CopyTo(artPollReplyPacket, 44);
            System.Text.Encoding.ASCII.GetBytes("NO INFO.\0                                                                      ".Substring(0, 64))
                .CopyTo(artPollReplyPacket,108);
            artPollReplyPacket[172] = 0; // nb port HI
            artPollReplyPacket[173] = 1; // nb port LO (1 in this version)

            artPollReplyPacket[174] = 0x80;    // Port Type 1 -  0b1000 0000 can output data from ArtNet network
            artPollReplyPacket[175] = 0;    // Port Type 2
            artPollReplyPacket[176] = 0;    // Port Type 3
            artPollReplyPacket[177] = 0;    // Port Type 4

            artPollReplyPacket[178] = 0x80;    // good input 1  0b1000 0000
            artPollReplyPacket[179] = 0;    // good input 2
            artPollReplyPacket[180] = 0;    // good input 3
            artPollReplyPacket[181] = 0;    // good input 4

            artPollReplyPacket[182] = 0x80;    // good output 1  0b1000 0000
            artPollReplyPacket[183] = 0;    // good output 2
            artPollReplyPacket[184] = 0;    // good output 3
            artPollReplyPacket[185] = 0;    // good output 4

            artPollReplyPacket[186] = 0;    // SwIn 1       // TO CHECK !!!
            artPollReplyPacket[187] = 0;    // SwIn 2
            artPollReplyPacket[188] = 0;    // SwIn 3
            artPollReplyPacket[189] = 0;    // SwIn 4

            artPollReplyPacket[190] = 2;    // SwOut 1      // TO CHECK !!!
            artPollReplyPacket[191] = 0;    // SwOut 2
            artPollReplyPacket[192] = 0;    // SwOut 3
            artPollReplyPacket[193] = 0;    // SwOut 4

            artPollReplyPacket[194] = 0;    // SwVideo // Deprecated
            artPollReplyPacket[195] = 0;    // sWMacro // deprecated
            artPollReplyPacket[196] = 0;    // SwRemote // deprecated

            artPollReplyPacket[197] = 0;    // spare1
            artPollReplyPacket[198] = 0;    // spare2
            artPollReplyPacket[199] = 0;    // spare3

            artPollReplyPacket[200] = 0x00; // StNode

            artPollReplyPacket[201] = 0x00; // MAC adress of node
            artPollReplyPacket[202] = 0x00;
            artPollReplyPacket[203] = 0x00;
            artPollReplyPacket[204] = 0x00;
            artPollReplyPacket[205] = 0x00;
            artPollReplyPacket[206] = 0x00;

            artPollReplyPacket[207] = 0x00; // IP address du device
            artPollReplyPacket[208] = 0x00;
            artPollReplyPacket[209] = 0x00;
            artPollReplyPacket[210] = 0x00;

            artPollReplyPacket[211] = 0x00; // BindIndex 

            artPollReplyPacket[212] = 0x06; // Status 2  - 0b0000 0110

            for(int n=0;n<26;n++)
            {
                artPollReplyPacket[213 + n] = 0x00;
            }
        }




        public async Task StartAsync()
        {
            UdpClient udpClient = new UdpClient();
            udpClient.ExclusiveAddressUse = false;
            IPEndPoint brodcastIp = new IPEndPoint(IPAddress.Any, ArtnetDefaultPort);
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.ExclusiveAddressUse = false;

            udpClient.Client.Bind(brodcastIp);

            while (true)
            {
                var receiveResult = await udpClient.ReceiveAsync();
                ParseReceivedPacketAsync(receiveResult.Buffer, receiveResult.RemoteEndPoint);
            }

        }

        #region ArtNet packet helper function
        bool IsArtNetPacket(byte[] packet)
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

        int GetOpCode(byte[] arnetPacket)
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
        #endregion

        async Task ParseReceivedPacketAsync(byte[] packet,IPEndPoint from)
        {
            await Task.Run(async () =>
            {
                //Console.WriteLine($"Packet receive from {from.Address.ToString()}:{from.Port}");
                if (!IsArtNetPacket(packet))
                {
                    Console.WriteLine("   NOT ARTNET. ignoring packet.");
                    return;
                }
                var opCode = GetOpCode(packet);
                switch (opCode)
                {
                    case 0x2000: // OpPoll
                        Console.WriteLine($"ArtNet OpPoll from {from.Address}");
                        await SendOpPollReplyAsync(packet,from);
                        break;
                    case 0x2100: // OpPollReply
                        Console.WriteLine("ArtNet OpPollReply (Ignoring it)");
                        break;
                    case 0x5000: // OpOutput / OpDmx
                        Console.WriteLine($"ArtNet OpDmx from {from.Address}");
                        break;
                    case 0x5200: // OpSync
                        Console.WriteLine($"ArtNet OpSync from {from.Address}");
                        break;
                    default:
                        Console.WriteLine($"ArtNet {opCode.ToString("x")} from {from.Address}");
                        break;
                }
            });
        }



        object lockForOpPollReplyPatching = new object();

        /// <summary>
        /// Preparing and sending the OpPollReply 
        /// </summary>
        /// <returns></returns>
        async Task SendOpPollReplyAsync(byte[] opPoolPacket,IPEndPoint from)
        {
            lock (lockForOpPollReplyPatching)
            {
                // patching the default OpPoolReply prebuilt packet    
                UpdateNodeIp(artPollReplyPacket,from);

                // sending it to the requester
                using (UdpClient udpclient = new UdpClient())
                {
                    IPEndPoint remoteep = new IPEndPoint(from.Address, ArtnetDefaultPort);
                    udpclient.Send(artPollReplyPacket, artPollReplyPacket.Length, remoteep);
                }
            }

        }

        void UpdateNodeIp(byte[] opPollReplyPacket,IPEndPoint from)
        {
            // TODO
            // find the unicast ip of the node based on the source ip of the opPool packet

            // Hack : hardcoded unicast ip
            opPollReplyPacket[10] = 192;
            opPollReplyPacket[11] = 168;
            opPollReplyPacket[12] = 92;
            opPollReplyPacket[13] = 227;


            // Hack : hardcoded MAC adresse 5C-51-4F-56-F0-81
            opPollReplyPacket[201] = 0x5C;
            opPollReplyPacket[202] = 0x51;
            opPollReplyPacket[203] = 0x4F;
            opPollReplyPacket[204] = 0x56;
            opPollReplyPacket[205] = 0xF0;
            opPollReplyPacket[206] = 0x81;

            // Hacll : hard coded node IP
            opPollReplyPacket[207] = 0x4F;
            opPollReplyPacket[208] = 0x56;
            opPollReplyPacket[209] = 0xF0;
            opPollReplyPacket[210] = 0x81;


        }

        public void Stop()
        {
        }

        public void Dispose()
        {
            this.Stop();
            GC.SuppressFinalize(this);
        }
    }
}
