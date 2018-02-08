using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    public class FixturePreset
    {
        /*
         <Preset Name="**BLACKOUT**" Title="Blackout" Description="full blackout : rvbw and dimmer to zero">
          <Set Input="3" value="0" description="set dimming to zero"/>
          <Set Input="4" value="0" description="red"/>
          <Set Input="5" value="0" description="green"/>
          <Set Input="6" value="0" description="blue"/>
          <Set Input="7" value="0" description="white"/>
        </Preset>
         */

        public FixturePreset()
        {

        }

        /// <summary>
        /// Name of the preset. Must be unique in a fixture
        /// Preset name staring with * are reserverd and must match a predefined list. (ex : *RED* , *BLUE* , *GREEN*, **BLACKOUT**, ...)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Title of the preset (dusplayed on mouse over or in coinfiguration ui)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// detailed description of the preset (display in configuraiton UI if requested)
        /// </summary>
        public string Description { get; set; }


        private List<FixturePresetSet> _sets=new List<FixturePresetSet>();
        public List<FixturePresetSet> Sets
        {
            get { return _sets; }
            internal set { _sets = value; }
        }

    }
}
