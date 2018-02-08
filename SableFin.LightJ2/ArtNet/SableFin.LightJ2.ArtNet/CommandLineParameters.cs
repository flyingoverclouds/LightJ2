using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNet
{
    class CommandLineParameters
    {
        public byte VersionMajor { get; set; } = 0;

        public byte VersionMinor { get; set; } = 111;

        public short Universe { get; set; } = 123;

        public ushort OemCode { get; set; } = (ushort)ArtNetOemCodes.OemEnttecf;

        public string OemShortName { get; set; } = "ArtNetToOpenDmx-NC";
        public string OemLongName { get; set; } = "ArtNetToOpenDmx-Nicolas Clerc-Sablefin-2016";


    }
}
