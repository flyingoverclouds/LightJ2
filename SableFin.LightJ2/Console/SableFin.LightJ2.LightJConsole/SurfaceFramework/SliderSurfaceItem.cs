using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SableFin.LightJ2.SurfaceFramework
{
    internal class SliderSurfaceItem : SurfaceItem
    {
        private short _sliderMin ;
        private short _sliderMax;

        private List<DmxBinding> dmxChannelTargets;
        public List<DmxBinding> DmxBindings
        {
            get
            {
                return dmxChannelTargets;
            }
        }

        public SliderSurfaceItem(string name,List<DmxBinding> dmxChannelTargets,byte min,byte max,byte defaultValue) : 
            base(name,dmxChannelTargets)
        {
            this.Min = min;
            this.Max = max;
            this.DmxValue = defaultValue;
            this.dmxChannelTargets  = dmxChannelTargets;
            
        }

        private short _value;
        public short DmxValue
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                RaisePropertyChangedEvent("DmxValue");
                if (FastRoutingMatrix.Current!=null)
                    FastRoutingMatrix.Current.SetCells(this.FastRoutingId,(byte)value, this.dmxChannelTargets);
            }
        }

        public short Min
        {
            get { return _sliderMin; }
            set { _sliderMin = value; RaisePropertyChangedEvent("Min");}
        }

        public short Max
        {
            get { return _sliderMax; }
            set { _sliderMax = value; RaisePropertyChangedEvent("Max");}
        }

        public string ImageResourceName { get; set; }

        internal override void SetDmxValue(FastRoutingFrame frame)
        {
            if (!UpdateControlWhenMatrixSendNewValue)
                return;
            if (frame.Value < Min || frame.Value> Max)
                return;
            this.DmxValue = frame.Value;
        }
    }
}
