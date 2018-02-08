using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SableFin.LightJ2.CoreCommon.DataModel
{


    internal class LoaderFixturePreset
    {
        /*
       <Preset name="**BLACKOUT**" title="Blackout" description="full blackout : rvbw and dimmer to zero">
          <Set input="3" value="0" description="set dimming to zero" />
          <Set input="4" value="0" description="red" />
          <Set input="5" value="0" description="green" />
          <Set input="6" value="0" description="blue" />
          <Set input="7" value="0" description="white" />
        </Preset>
        */

        public static FixturePreset LoadFromXml(XElement xePreset)
        {
            try
            {
                if (xePreset.Name.LocalName != "Preset")
                {
                    throw new ArgumentException("element is not 'Preset'");
                }

                var preset = new FixturePreset();

                preset.Name = xePreset.Attribute("name").Value;
                preset.Title = xePreset.Attribute("title").Value;
                preset.Description = xePreset.Attribute("description").Value;
                //preset.Num = short.Parse(xeInput.Attribute("num").Value);
                foreach (var xeSet in xePreset.Elements("Set"))
                {
                    int inp = int.Parse(xeSet.Attribute("input").Value);
                    short val = short.Parse(xeSet.Attribute("value").Value);
                    var presetSet = new FixturePresetSet(inp, val);
                    presetSet.Description = xeSet.Attribute("description").Value;
                    preset.Sets.Add(presetSet);
                }
                return preset;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Error while parsing {xePreset.ToString()}");
                return null;
            }
        }
    }
}
