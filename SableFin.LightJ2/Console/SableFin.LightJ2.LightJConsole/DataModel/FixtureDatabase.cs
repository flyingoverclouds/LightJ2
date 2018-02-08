using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.CoreCommon.DataModel
{
    public class FixtureDatabase
    {
        public FixtureDatabase()
        {
            this._fixtures=new Dictionary<string, Fixture>();
        }

        public string Name { get; internal set; }
        public string Version { get; internal set; }
        public string Author { get; internal set; }
        public string Origin { get; internal set; }

        public string Date { get; internal set; }

        private Dictionary<string, Fixture> _fixtures;
        public Dictionary<string, Fixture> Fixtures
        {
            get { return _fixtures; }
            internal set { _fixtures = value; }
        }

        
    }
}
