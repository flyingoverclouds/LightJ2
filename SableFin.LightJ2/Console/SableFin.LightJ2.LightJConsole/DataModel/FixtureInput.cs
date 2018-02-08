using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    public class FixtureInput
    {
        public FixtureInput()
        {
            this.InputRanges = new List<InputRange>();
        }

        public short Num { get; set; }

        public string Name { get; set; }

        private List<InputRange> _inputRanges;
        public List<InputRange> InputRanges
        {
            get { return _inputRanges; }
            internal set { _inputRanges = value; }
        }

    }
}
