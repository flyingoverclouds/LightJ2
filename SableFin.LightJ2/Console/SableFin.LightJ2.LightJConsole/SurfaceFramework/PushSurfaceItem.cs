using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.SurfaceFramework
{
    class PushSurfaceItem : SurfaceItem
    {

        private DmxBinding[] bindingsPressed = new DmxBinding[0]; // list des valeur à emettre quand on press le bouton
        private DmxBinding[] bindingsReleased = new DmxBinding[0]; // liste des valeurs a émettre quand on relache le bouton
        public PushSurfaceItem(string name, List<DmxBinding> bindingsOnPress,List<DmxBinding> bindingsOnRelease) : 
            base(name,bindingsOnPress)
        {
            if (bindingsOnPress!=null)
                this.bindingsPressed = bindingsOnPress.ToArray();
            if (bindingsOnRelease!=null)
                this.bindingsReleased = bindingsOnRelease.ToArray();
        }

        private bool _ispressed;
        public bool IsPressed
        {
            get { return _ispressed; }
            set
            {
                if (_ispressed == value)
                    return;
                _ispressed = value;
                RaisePropertyChangedEvent("IsPressed");
                UpdateMatrixForState(value);
            }
        }

        public string ImageResourceName { get; set; }

        void UpdateMatrixForState(bool isPressed)
        {
            if (FastRoutingMatrix.Current == null)
                return;
            if (isPressed)
            {
                // on emet les trame pour l'etat préssé
                for (int n = 0; n < this.bindingsPressed.Length;n++)
                {
                    FastRoutingMatrix.Current.SetCell(this.FastRoutingId, (byte)bindingsPressed[n].Value, bindingsPressed[n].Channel);
                }
            }
            else
            {
                // on emet les trame correspondant à un relachement du bouton
                for (int n = 0; n < this.bindingsReleased.Length;n++)
                {
                    FastRoutingMatrix.Current.SetCell(this.FastRoutingId, (byte)bindingsReleased[n].Value, bindingsReleased[n].Channel);
                }
            }
            

        }
        internal override void SetDmxValue(FastRoutingFrame frame)
        {
            if (!UpdateControlWhenMatrixSendNewValue)
                return;
        }

    }
}
