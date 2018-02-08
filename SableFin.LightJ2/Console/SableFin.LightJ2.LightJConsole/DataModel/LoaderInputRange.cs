using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    internal class LoaderInputRange
    {
        public static InputRange LoadFromXml(XElement xeRange)
        {
            try
            {

                if (xeRange.Name.LocalName != "Range")
                {
                    throw new ArgumentException("element is not 'Range'");
                }
                var range = new InputRange();
                range.Num = xeRange.Attribute("num").Value;
                range.Name = xeRange.Attribute("name").Value;
                range.Type = (RangeType) Enum.Parse(typeof (RangeType), xeRange.Attribute("type").Value);
                range.Minimum = byte.Parse(xeRange.Attribute("minimum").Value);
                range.Maximum = byte.Parse(xeRange.Attribute("maximum").Value);
                if (xeRange.Attribute("default") != null)
                    range.Default = byte.Parse(xeRange.Attribute("default").Value);
                else
                    range.Default = range.Minimum;
                return range;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
