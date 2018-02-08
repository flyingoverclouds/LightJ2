using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet
{
    class Universe : INotifyPropertyChanged
    {
        private int universeId;

        private int subnetId;

        public int UniverseId
        {
            get
            {
                return universeId;
            }

            set
            {
                if (universeId != value)
                    RaisePropertyChanged(nameof(UniverseId));
                universeId = value;
            }
        }

        public int SubnetId
        {
            get
            {
                return subnetId;
            }

            set
            {
                if (subnetId!= value)
                    RaisePropertyChanged(nameof(SubnetId));
                subnetId = value;
            }
        }


        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
