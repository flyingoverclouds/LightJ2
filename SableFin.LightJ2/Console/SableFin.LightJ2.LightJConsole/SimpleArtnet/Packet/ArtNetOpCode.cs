using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet.Packet
{

    public sealed class ArtNetOpCode
    {
        /// <summary>
        /// ArtPoll Packet. No other data in packet.
        /// </summary>
        public const ushort OpPoll = 0x2000;

        /// <summary>
        /// ArtPollReply packet. Contain device status information
        /// </summary>
        public const ushort OpPollReply = 0x2100;

        /// <summary>
        /// Diagnostic and data logging packet
        /// </summary>
        const ushort OpDiagData = 0x2300;

        /// <summary>
        /// Used to send text based paratemter commands
        /// </summary>
        const ushort OpCommand = 0x2400;

        ///// <summary>
        ///// ArtDmx pcacket. Contain zero start code DMX512 information for a single universe
        ///// Same as OpDmx
        ///// </summary>
        //public const ushort OpOutput = 0x5000;

        /// <summary>
        /// ArtDmx pcacket. Contain zero start code DMX512 information for a signle universe
        /// Same as OpOutput
        /// </summary>
        public const ushort OpDmx = 0x5000;

        /// <summary>
        /// ArtNzs packet. Contains non-zero start code (except RDM) DMX 512 informations for a single universe.
        /// </summary>
        const ushort OpNzs = 0x5100;

        /// <summary>
        /// ArtSync data packet. It is used to force synchronous transfer of ArtDmx packet to a node's output.
        /// </summary>
        const ushort OpSync = 0x5200;

        /// <summary>
        /// This is an ArtAddress packet. It contains remote programming information for a Node.
        /// </summary>
        const ushort OpAddress = 0x6000;

        /// <summary>
        /// This is an ArtInput packet. It contains enable – disable data for DMX inputs
        /// </summary>
        const ushort OpInput = 0x7000;

        /// <summary>
        /// This is an ArtTodRequest packet. It is used to request a Table of Devices (ToD) for RDM discovery. 
        /// </summary>
        const ushort OpToRequest = 0x8000;

        /// <summary>
        /// This is an ArtTodData packet. It is used to send a Table of Devices (ToD) for RDM discovery. 
        /// </summary>
        const ushort OpTodData = 0x0;

        /// <summary>
        /// This is an ArtTodControl packet. It is used to send RDM discovery control messages. 
        /// </summary>
        const ushort OpTodControl = 0x8200;

        /// <summary>
        /// This is an ArtRdm packet. It is used to send all non discovery RDM messages. 
        /// </summary>
        const ushort OpRdm = 0x8300;

        /// <summary>
        /// This is an ArtRdmSub packet. It is used to send compressed, RDM Sub-Device data
        /// </summary>
        const ushort OpRdmSub = 0x8400;

        /// <summary>
        /// This is an ArtVideoSetup packet. It contains video screen setup information for nodes that implement the extended video features. 
        /// </summary>
        const ushort OpVideoSetup = 0xa010;

        /// <summary>
        /// This is an ArtVideoPalette packet. It contains colour palette setup information for nodes that implement the extended video features. 
        /// </summary>
        const ushort OpVideoPalette = 0xa020;

        /// <summary>
        /// This is an ArtVideoData packet. It contains display data for nodes that implement the extended video features
        /// </summary>
        const ushort OpVideoData = 0xa040;

        /// <summary>
        /// This is an ArtMacMaster packet. It is used to program the Node’s MAC address, Oem device type and ESTA manufacturer code. This is for factory initialisation of a Node. It is not to be used by applications
        /// </summary>
        const ushort OpMacMaster = 0xf000;

        /// <summary>
        /// This is an ArtMacSlave packet. It is returned by the node to acknowledge receipt of an ArtMacMaster packet. 
        /// </summary>
        const ushort OpMacSlave = 0x0f100;

        /// <summary>
        /// This is an ArtFirmwareMaster packet. It is used to upload new firmware or firmware extensions to the Node. 
        /// </summary>
        const ushort OpFirmwareMaster = 0xf200;

        /// <summary>
        /// This is an ArtFirmwareReply packet. It is returned by the node to acknowledge receipt of an ArtFirmwareMaster packet or ArtFileTnMaster packet
        /// </summary>
        const ushort OpFirmwareReply = 0xf300;

        /// <summary>
        /// Uploads user file to node
        /// </summary>
        const ushort OpFileTnMAster = 0xf400;

        /// <summary>
        /// Downloads user file from node. 
        /// </summary>
        const ushort OpFileFnMaster = 0xf500;

        /// <summary>
        /// Node acknowledge for downloads. 
        /// </summary>
        const ushort OpFileFnReply = 0xf600;

        /// <summary>
        /// This is an ArtIpProg packet. It is used to reprogramme the IP, Mask and Port address of the Node. 
        /// </summary>
        const ushort OpIpProg = 0xf800;

        /// <summary>
        /// This is an ArtIpProgReply packet. It is returned by the node to acknowledge receipt of an ArtIpProg packet. 
        /// </summary>
        const ushort OpIpProgReply = 0x0;

        /// <summary>
        /// This is an ArtMedia packet. It is Unicast by a Media Server and acted upon by a Controller.
        /// </summary>
        const ushort OpMedia = 0x9000;

        /// <summary>
        /// This is an ArtMediaPatch packet. It is Unicast by a Controller and acted upon by a Media Server
        /// </summary>
        const ushort OpMediaPatch = 0x9100;

        /// <summary>
        /// This is an ArtMediaControl packet. It is Unicast by a Controller and acted upon by a Media Server.
        /// </summary>
        const ushort OpMediaControl = 0x9200;

        /// <summary>
        /// This is an ArtMediaControlReply packet. It is Unicast by a Media Server and acted upon by a Controller
        /// </summary>
        const ushort OpMediaControlReply = 0x0;

        /// <summary>
        /// This is an ArtTimeCode packet. It is used to transport time code over the network
        /// </summary>
        const ushort OpTimeCode = 0x9700;

        /// <summary>
        /// Used to synchronise real time date and clock 
        /// </summary>
        const ushort OpTimeSync = 0x9800;

        /// <summary>
        /// Used to send trigger macros 
        /// </summary>
        const ushort OpTrigger = 0x9900;

        /// <summary>
        /// Requests a node's file list 
        /// </summary>
        const ushort OpDirectory = 0x9a00;

        /// <summary>
        /// Replies to OpDirectory with file list 
        /// </summary>
        const ushort OpDirectoryReply = 0x9b00;

        /// <summary>
        /// REturn the opcode name from the opcode value.
        /// </summary>
        /// <param name="opCode">numerical value of opcode</param>
        /// <returns>opcode name or 'UNKNOW/UNSUPPORTED'</returns>
        public static string ToString(ushort opCode)
        {
            switch(opCode)
            {
                case 0x2000:
                    return "OpPoll";
                    break;
                case 0x2100:
                    return "OpPollReply";
                    break;
            }
            return "UNKNOW/UNSUPPORTED";
        }
    }
}
