using SableFin.LightJ2.SurfaceFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.SurfaceFramework
{
    class ValueReverserSurfaceItem : SurfaceItem
    {
        private DmxBinding[] reverseTargets = new DmxBinding[0]; // list des bindings a inverser
        public ValueReverserSurfaceItem(string name, List<DmxBinding> reverseTargets) : 
            base(name,null) // on ne propage pas la liste des bindings car aucune influence sur les valeurs DMX
        {
            if (reverseTargets != null)
                this.reverseTargets = reverseTargets.ToArray();
        }

        private bool _isReverse;
        public bool IsReverse
        {
            get { return _isReverse; }
            set
            {
                if (_isReverse == value)
                    return;
                _isReverse = value;
                SetReverseTo(value);
                RaisePropertyChangedEvent("IsReverse");
            }
        }

        void SetReverseTo(bool reverse)
        {
            for (int n = 0; n < reverseTargets.Length; n++)
                this.reverseTargets[n].Reverse = reverse;
        }


        public string ImageResourceName { get; set; }
        internal override void SetDmxValue(FastRoutingFrame frame)
        {
            // Nothings to do : this is not DMX channel linked surface item
        }
    }
}
