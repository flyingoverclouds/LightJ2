using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    public class InputRange
    {
        public InputRange()
        {
        }

        public string Num { get; internal set; }
        
        public string Name { get; internal set; }

        public RangeType Type { get; internal set; }

        public byte Minimum { get; internal set; }

        public byte Maximum { get; internal set; }

        public byte Default { get; internal set; }
    }
}
