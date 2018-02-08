using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    internal class LoaderFixtureInput
    {
        public static FixtureInput LoadFromXml(XElement xeInput)
        {
            if (xeInput.Name.LocalName != "Input")
            {
                throw new ArgumentException("element is not 'Input'");
            }
            var inp = new FixtureInput();
            inp.Name = xeInput.Attribute("name").Value;
            inp.Num = short.Parse(xeInput.Attribute("num").Value);
            foreach (var xeRange in xeInput.Elements("Range"))
            {
                var range = LoaderInputRange.LoadFromXml(xeRange);
                inp.InputRanges.Add(range);
            }
            return inp;
        }
    }


}
