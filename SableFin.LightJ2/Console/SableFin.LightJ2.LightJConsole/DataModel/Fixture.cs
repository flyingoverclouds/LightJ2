using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    public class Fixture
    {
        public Fixture()
        {
            this.FixtureInputs=new List<FixtureInput>();
        }
        public string Id { get; internal set; }

        public string Name { get; internal set; }

        public string Manufacturer { get; internal set; }

        public string Ref { get; internal set; }

        public string Language { get; internal set; }

        public string Version { get; internal set; }

        private List<FixtureInput> _fixtureInputs;
        public List<FixtureInput> FixtureInputs
        {
            get { return _fixtureInputs; }
            internal set { _fixtureInputs = value; }
        }

        private Dictionary<string,FixturePreset> _presets;
        public Dictionary<string, FixturePreset> Presets
        {
            get { return _presets; }
            internal set { _presets = value; }
        }

        public InputRange GetInputRangeByNum(string numRange)
        {
            foreach (var fixtureInput in FixtureInputs)
            {
                var r = fixtureInput.InputRanges.SingleOrDefault(fi => fi.Num == numRange);
                if (r != null)
                    return r;
            }
            return null;
        }
    }
}
