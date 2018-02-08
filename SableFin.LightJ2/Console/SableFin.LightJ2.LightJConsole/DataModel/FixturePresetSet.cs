using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
   
    public class FixturePresetSet
    {
        //  <Set Input="3" value="0" description="set dimming to zero"/>

        public FixturePresetSet(int input,short value)
        {
            this.Input = input;
            this.Value = value;
        }

        public int Input { get; set; }

        public int Value { get; set; }

        public string Description { get; set; }
    }
}
