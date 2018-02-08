using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.SurfaceFramework
{
    class DmxBinding
    {
        private short _channel;

        private short _value;

        private bool _reverse=false;
        public short Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public short Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }

        /// <summary>
        /// minimum DMX value allowed (only used when Reverse=True)
        /// </summary>
        public byte DmxMin { get; set; } = 0;

        /// <summary>
        /// maximum DMX value allowed (only used when Revers=True) 
        /// </summary>
        public byte DmxMax { get; set; } = 255;
    }
}
