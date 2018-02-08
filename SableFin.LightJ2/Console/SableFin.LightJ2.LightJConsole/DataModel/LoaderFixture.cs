using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    internal class LoaderFixture
    {
        public static Fixture LoadFromXml(XElement xeFixture)
        {
            if (xeFixture.Name.LocalName != "Fixture")
            {
                throw new ArgumentException("element is not 'Fixture'");
            }

            var fixt = new Fixture();
            XAttribute xa;
            xa = xeFixture.Attribute("name");
            if (xa != null)
                fixt.Name = xa.Value;
            else
                throw new ArgumentException("Missing [name] in fixture");


            xa = xeFixture.Attribute("manufacturer");
            if (xa != null)
                fixt.Manufacturer = xa.Value;
            else
                throw new ArgumentException("Missing [manufacturer] for fixture [" + fixt.Name+"]");

            xa = xeFixture.Attribute("id");
            if (xa != null)
                fixt.Id = xa.Value;
            else
                throw new ArgumentException("Missing [ref] for fixture [" + fixt.Name + "]");

            xa = xeFixture.Attribute("ref");
            if (xa != null)
                fixt.Ref = xa.Value;
            else
                throw new ArgumentException("Missing [ref] for fixture [" + fixt.Name + "]");

            xa = xeFixture.Attribute("language");
            if (xa != null)
                fixt.Language = xa.Value;
            else
                throw new ArgumentException("Missing [language] for fixture [" + fixt.Name + "]");

            int fixtureInputPos = 0;

            // Loading INPUTs
            foreach (var xeInp in xeFixture.Elements("Input"))
            {
                fixtureInputPos++;
                try {
                    var inp = LoaderFixtureInput.LoadFromXml(xeInp);
                    fixt.FixtureInputs.Add(inp);
                }
                catch(Exception)
                {
                    Debug.WriteLine(" Fixture [{0}] : Error loading FixtureInput number {1}",fixt.Name,fixtureInputPos);
                }
            }

            int presetPos = 0;
            try
            {
                presetPos++;
                // Loading PRESETs
                foreach (var xePre in xeFixture.Elements("Preset"))
                {
                    var fixPre = LoaderFixturePreset.LoadFromXml(xePre);
                    if (fixPre != null)
                        fixt.Presets.Add(fixPre.Name,fixPre);
                }
            }
            catch(Exception)
            {
                Debug.WriteLine(" Fixture [{0}] : Error loadin fixture Preset number {1}", fixt.Name, presetPos);
            }
            return fixt;
        }
    }
}
