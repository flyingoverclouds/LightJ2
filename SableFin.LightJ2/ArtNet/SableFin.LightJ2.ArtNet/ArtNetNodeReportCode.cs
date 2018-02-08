using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNet
{
    public sealed class ArtNetNodeReportCode
    {
        /// <summary>
        /// Booted in debug mode (Only used in development) 
        /// </summary>
        const ushort RcDebug = 0x0;

        /// <summary>
        /// Power on tests successful
        /// </summary>
        const ushort RcPowerOk = 0x0001;

        /// <summary>
        /// Hardware tests failed at Power On 
        /// </summary>
        const ushort RcPowerFail = 0x0002;

        /// <summary>
        /// Last UDP from Node failed due to truncated length,  Most likely caused by a collision. 
        /// </summary>
        const ushort RcSocketWr1 = 0x0003;

        /// <summary>
        /// Unable to identify last UDP transmission. Check OpCode and packet length. 
        /// </summary>
        const ushort RcParseFail = 0x0004;

        /// <summary>
        /// Unable to open Udp Socket in last transmission attempt 
        /// </summary>
        const ushort RcUdpFail = 0x0005;

        /// <summary>
        /// Confirms that Short Name programming via ArtAddress, was successful. 
        /// </summary>
        const ushort RcShNameOk = 0x0006;

        /// <summary>
        /// Confirms that Long Name programming via ArtAddress, was successful. 
        /// </summary>
        const ushort RcLoNameOk = 0x0007;

        /// <summary>
        /// DMX512 receive errors detected. 
        /// </summary>
        const ushort RcDmxError = 0x0008;

        /// <summary>
        /// Ran out of internal DMX transmit buffers. 
        /// </summary>
        const ushort RcDmxUdpFull = 0x0009;

        /// <summary>
        /// Ran out of internal DMX Rx buffers
        /// </summary>
        const ushort RcDmxRxFull = 0x000a;

        /// <summary>
        /// Rx Universe switches conflict
        /// </summary>
        const ushort RcSwitchErr = 0x000b;

        /// <summary>
        /// Product configuration does not match firmware. 
        /// </summary>
        const ushort RcConfigErr = 0x000c;

        /// <summary>
        /// DMX output short detected. See GoodOutput field
        /// </summary>
        const ushort RcDmxShort = 0x000d;

        /// <summary>
        /// Last attempt to upload new firmware failed
        /// </summary>
        const ushort RcFirmwareFail = 0x000e;

        /// <summary>
        /// User changed switch settings when address locked by remote programming. User changes ignored. 
        /// </summary>
        const ushort RcUserFail = 0x000f;

        /// <summary>
        /// 
        /// </summary>
        const ushort Rc = 0x0;



    }
}
