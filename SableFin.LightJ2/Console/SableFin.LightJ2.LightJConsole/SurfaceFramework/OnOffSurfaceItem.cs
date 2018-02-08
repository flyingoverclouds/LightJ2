using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.SurfaceFramework
{
    class OnOffSurfaceItem : SurfaceItem
    {
       
        private DmxBinding[] bindingsOn = new DmxBinding[0]; // list des valeur à emettre quand le bouton pass à ON
        private DmxBinding[] bindingsOff = new DmxBinding[0]; // liste des valeurs a émettre quand le bouton pass a OFF
        public OnOffSurfaceItem(string name, List<DmxBinding> bindingsOn, List<DmxBinding> bindingsOff) : 
            base(name,bindingsOn)
        {
            if (bindingsOn!=null)
                this.bindingsOn = bindingsOn.ToArray();
            if (bindingsOff!=null)
                this.bindingsOff = bindingsOff.ToArray();
        }

        private bool _isOn;
        public bool IsOn
        {
            get { return _isOn; }
            set
            {
                if (_isOn == value)
                    return;
                _isOn = value;
                RaisePropertyChangedEvent("IsOn");
                UpdateMatrixForState(value);
            }
        }

        void UpdateMatrixForState(bool isOn)
        {
            if (FastRoutingMatrix.Current == null)
                return;
            if (isOn)
            {
                // on emet les trame pour l'etat On
                for (int n = 0; n < this.bindingsOn.Length;n++)
                {
                    FastRoutingMatrix.Current.SetCell(this.FastRoutingId, (byte)bindingsOn[n].Value, bindingsOn[n].Channel);
                }
            }
            else
            {
                // on emet les trame correspondant à un etat Off
                for (int n = 0; n < this.bindingsOff.Length;n++)
                {
                    FastRoutingMatrix.Current.SetCell(this.FastRoutingId, (byte)bindingsOff[n].Value, bindingsOff[n].Channel);
                }
            }
            

        }

        public string ImageResourceName { get; set; }

        internal override void SetDmxValue(FastRoutingFrame frame)
        {
            if (!UpdateControlWhenMatrixSendNewValue)
                return;
        }

    }
}
