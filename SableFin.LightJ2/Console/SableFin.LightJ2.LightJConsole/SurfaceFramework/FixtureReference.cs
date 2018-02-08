using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.SurfaceFramework
{
    /// <summary>
    /// Classe mémorisant les informations d'un fixture utilisé dans une configuration
    /// </summary>
    class FixtureReference
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string DbFixtureId { get; set; }
        public short DmxChannel { get; set; }
    }
}
