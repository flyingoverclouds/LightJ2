using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet
{
    public class Device : INotifyPropertyChanged
    {
        string _ip;
        int _port;
        string _name;

        public string Ip
        {
            get
            {
                return _ip;
            }

            set
            {
                if (_ip != value)
                    RaisePropertyChanged(nameof(Ip));
                _ip = value;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                if (_port != value)
                    RaisePropertyChanged(nameof(Port));
                _port = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name != value)
                    RaisePropertyChanged(nameof(Name));
                _name = value;
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
