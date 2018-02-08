using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace SableFin.LightJ2.SurfaceFramework
{
    /// <summary>
    /// Classe de base pour un item d'interface avec un controle de UI
    /// </summary>
    abstract class SurfaceItem : INotifyPropertyChanged
    {
        private bool _updateControlWhenMatrixSendNewValue = true;
        protected List<DmxBinding> dmxChannelsTargeted;
        public SurfaceItem(string text, List<DmxBinding> dmxChannelTargets)
        {
            this.FastRoutingId = -1;
            this.Text = text;
            this.dmxChannelsTargeted = dmxChannelTargets;
            if (dmxChannelTargets == null)
                this.dmxChannelsTargeted = new List<DmxBinding>();
            if (this.dmxChannelsTargeted.Count > 1)
                _updateControlWhenMatrixSendNewValue = false;
        }

        protected bool UpdateControlWhenMatrixSendNewValue
        {
            get { return _updateControlWhenMatrixSendNewValue;  }
        }
        
        /// <summary>
        /// routing ID used by the FastRoutingMethod
        /// NEVER CHANGE IT !!! OR ROUTING WILL FAIL
        /// </summary>
        public short FastRoutingId { get; internal set; }



        internal List<short> GetSubscribedDmxChannel()
        {
            return dmxChannelsTargeted.Select( dm => dm.Channel ).ToList();
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; RaisePropertyChangedEvent("Text");}
        }

        internal abstract void SetDmxValue(FastRoutingFrame frame);

        #region INotifyPropertyChanged

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged!=null)
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion

     
    }
}
