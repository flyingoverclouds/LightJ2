using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet.Packet
{
    class ArtPollReply : ArtNetPacket
    {
        public ArtPollReply(byte[] udpPayload) : base(ArtNetOpCode.OpPollReply)
        {
            Unpack(udpPayload);
        }

        /// <summary>
        /// Create a packet for sending
        /// </summary>
        public ArtPollReply() : base(ArtNetOpCode.OpPollReply)
        {

        }

        void Unpack(byte[] udpPayload)
        {
            this.IpNode = string.Format("{0}.{1}.{2}.{3}", udpPayload[10], udpPayload[11], udpPayload[12], udpPayload[13]);
            this.IpPortNode = udpPayload[14] + (udpPayload[15] << 8);
            this.FirmwareVersion = string.Format("{0}.{1}", udpPayload[16], udpPayload[17]);
            this.NetSwitch = udpPayload[18];
            this.SubSwitch = udpPayload[19];
            this.OemCode = (ushort)(udpPayload[21] + (udpPayload[20] << 8));
            this.UbeaVersion = udpPayload[22];
            this.Status1 = udpPayload[23];
            this.EstaManufacturer = (ushort)(udpPayload[24] + (udpPayload[25] << 8));
            this.ShortName = ExtractZStringFromPayload(udpPayload, 26, 18);
            this.LongName = ExtractZStringFromPayload(udpPayload, 44, 64);
            this.NodeReport = ExtractZStringFromPayload(udpPayload, 108, 64);
            this.NumPorts = (ushort)(udpPayload[173] + (udpPayload[172] << 8));

            var portTypes = new byte[4];
            portTypes[0] = udpPayload[174];
            portTypes[1] = udpPayload[175];
            portTypes[2] = udpPayload[176];
            portTypes[3] = udpPayload[177];

            var portInput = new byte[4];
            portInput[0] = udpPayload[178];
            portInput[1] = udpPayload[179];
            portInput[2] = udpPayload[180];
            portInput[3] = udpPayload[181];

            var portOutput = new byte[4];
            portOutput[0] = udpPayload[182];
            portOutput[1] = udpPayload[183];
            portOutput[2] = udpPayload[184];
            portOutput[3] = udpPayload[185];

            var portSwitchInput = new byte[4];
            portSwitchInput[0] = udpPayload[186];
            portSwitchInput[1] = udpPayload[187];
            portSwitchInput[2] = udpPayload[188];
            portSwitchInput[3] = udpPayload[189];

            var portSwitchOutput = new byte[4];
            portSwitchOutput[0] = udpPayload[190];
            portSwitchOutput[1] = udpPayload[191];
            portSwitchOutput[2] = udpPayload[192];
            portSwitchOutput[3] = udpPayload[193];


            this.SwVideo = udpPayload[194];
            this.SwMacro = udpPayload[195];
            this.SwRemote = udpPayload[196];


            this.Ports = new ArtPollReplyPort[this.NumPorts];
            if (NumPorts >= 1) this.Ports[0] = new ArtPollReplyPort(portTypes[0], portInput[0], portOutput[0], portSwitchInput[0], portSwitchOutput[0]);
            if (NumPorts >= 2) this.Ports[1] = new ArtPollReplyPort(portTypes[1], portInput[1], portOutput[1], portSwitchInput[1], portSwitchOutput[1]);
            if (NumPorts >= 3) this.Ports[2] = new ArtPollReplyPort(portTypes[2], portInput[2], portOutput[2], portSwitchInput[2], portSwitchOutput[2]);
            if (NumPorts >= 4) this.Ports[3] = new ArtPollReplyPort(portTypes[3], portInput[3], portOutput[3], portSwitchInput[3], portSwitchOutput[3]);
        }

        public string IpNode { get; private set; }
        public int IpPortNode { get; private set; }
        public string FirmwareVersion { get; private set; }
        public byte NetSwitch { get; private set; }
        public byte SubSwitch { get; private set; }
        public ushort OemCode { get; private set; }
        public byte UbeaVersion { get; private set; }
        public byte Status1 { get; private set; }
        public ushort EstaManufacturer { get; private set; }
        public string ShortName { get; private set; }
        public string LongName { get; private set; }
        public string NodeReport { get; private set; }
        public ushort NumPorts { get; private set; }

        public ArtPollReplyPort[] Ports {get; private set;}

        public byte SwVideo { get; private set; }
        public byte SwMacro { get; private set; }
        public byte SwRemote { get; private set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine("Ip: " + IpNode);
            sb.AppendLine("Port: " + IpPortNode);
            sb.AppendFormat("Oem: 0x{0} [{1}]", OemCode.ToString("x"),/*OemCode.GetOemName()*/"OEM_NAME").AppendLine();
            sb.AppendFormat("Firmware: {0}", FirmwareVersion).AppendLine();
            sb.AppendFormat("NetSwitch: {0}", NetSwitch).AppendLine();
            sb.AppendFormat("SubSwitch: {0}", SubSwitch).AppendLine();
            sb.AppendFormat("UbeaVersion: 0x{0}", this.UbeaVersion.ToString("x")).AppendLine();
            sb.AppendFormat("Status1: 0x{0} [{1}]", this.Status1.ToString("x"),StatusToString( this.Status1)).AppendLine();
            sb.AppendFormat("EstaManufacturer: 0x{0}", this.EstaManufacturer.ToString("x")).AppendLine();
            sb.AppendFormat("ShortName: [{0}]",this.ShortName).AppendLine();
            sb.AppendFormat("LongName: [{0}]",this.LongName).AppendLine();
            sb.AppendFormat("NodeReport: [{0}]",this.NodeReport).AppendLine();
            sb.AppendFormat("NumPorts: {0}", this.NumPorts).AppendLine();
            //for(int n=0;n<4;n++)
            //{
            //    sb.AppendFormat("Port #{0} Type: 0x{1}  [{2}]", n, PortTypes[n].ToString("x"), PortTypeToString(PortTypes[n])).AppendLine();
            //    sb.AppendFormat("Port #{0} Input: 0x{1}  [{2}]", n, PortInput[n].ToString("x"), "NOT DECODED").AppendLine();
            //    sb.AppendFormat("Port #{0} Output: 0x{1}  [{2}]", n, PortOutput[n].ToString("x"), "NOT DECODED").AppendLine();
            //    sb.AppendFormat("Port #{0} SwInput: 0x{1}  [{2}]", n, PortSwitchInput[n].ToString("x"), "NOT DECODED").AppendLine();
            //    sb.AppendFormat("Port #{0} SwOutput: 0x{1}  [{2}]", n, PortSwitchOutput[n].ToString("x"), "NOT DECODED").AppendLine();
            //}
            sb.AppendFormat("SwVideo: 0x{0}", SwVideo.ToString("x"));
            sb.AppendFormat("SwMacro: 0x{0}", SwMacro.ToString("x"));
            sb.AppendFormat("SwRemote: 0x{0}", SwRemote.ToString("x"));
            return sb.ToString();
        }

        string PortTypeToString(byte portType)
        {
            StringBuilder sb = new StringBuilder();
            switch(portType & 0x00111111)
            {
                case 0x0:
                    sb.Append("DMX512,");
                    break;
                case 0x000001:
                    sb.Append("MIDI,");
                    break;
                case 0x000010:
                    sb.Append("Avab,");
                    break;
                case 0x000011:
                    sb.Append("ColortranCMX,");
                    break;
                case 0x000100:
                    sb.Append("ADB62.5,");
                    break;
                case 0x000101:
                    sb.Append("Art-Net,");
                    break;
            }
            if ((portType & 0x10000000) != 0)
                sb.Append("ArtNet_OUT,");
            if ((portType & 0x01000000) != 0)
                sb.Append("ArtNet_IN,");
            return sb.ToString().Trim(',');
        }

        string PortInputToString(byte portInput)
        {
            StringBuilder sb = new StringBuilder();

            return sb.ToString().Trim(',');
        }

        string StatusToString(byte status)
        {
            StringBuilder sb = new StringBuilder();
            switch (status & 0x01100000)
            {
                case 0x00000000:
                    sb.Append("Indicator:Unknow,");
                    break;
                case 0x00100000:
                    sb.Append("Indicator:Locate,");
                    break;
                case 0x01000000:
                    sb.Append("Indicator:Mute,");
                    break;
                case 0x01100000:
                    sb.Append("Indicator:Normal,");
                    break;
            }
            switch (status & 0x00011000)
            {
                case 0x00000000:
                    sb.Append("ProgAuth:Unknow,");
                    break;
                case 0x00001000:
                    sb.Append("ProgAuth:FrontPanel,");
                    break;
                case 0x00010000:
                    sb.Append("ProgAuth:Network,");
                    break;
                case 0x00011000:
                    sb.Append("ProgAuth:NotUsed,");
                    break;
            }
            if ((status & 0x00000010) == 0)
                sb.Append("Boot:Flash,");
            else
                sb.Append("Boot:ROM,");
            if ((status & 0x00000001) == 0)
                sb.Append("RDM:No,");
            else
                sb.Append("RDM:Yes,");
            return sb.ToString().Trim(',');
        }
    }
}
