using System.ComponentModel;
using System.Runtime.CompilerServices;
using DivineShopMonitor.Annotations;

namespace DivineShopMonitor.Model
{
    public class MainModel : INotifyPropertyChanged
    {
        private bool _garena2500 = true;
        private bool _garena2400 = true;
        private bool _garena520;
        private bool _garena500 = true;
        private bool _garena200 = true;
        private bool _garena100 = true;
        private bool _garena87;

        public bool Garena2500
        {
            get => _garena2500;
            set
            {
                if (value == _garena2500) return;
                _garena2500 = value;
                OnPropertyChanged();
            }
        }

        public bool Garena2400
        {
            get => _garena2400;
            set
            {
                if (value == _garena2400) return;
                _garena2400 = value;
                OnPropertyChanged();
            }
        }

        public bool Garena520
        {
            get => _garena520;
            set
            {
                if (value == _garena520) return;
                _garena520 = value;
                OnPropertyChanged();
            }
        }

        public bool Garena500
        {
            get => _garena500;
            set
            {
                if (value == _garena500) return;
                _garena500 = value;
                OnPropertyChanged();
            }
        }

        public bool Garena200
        {
            get => _garena200;
            set
            {
                if (value == _garena200) return;
                _garena200 = value;
                OnPropertyChanged();
            }
        }

        public bool Garena100
        {
            get => _garena100;
            set
            {
                if (value == _garena100) return;
                _garena100 = value;
                OnPropertyChanged();
            }
        }

        public bool Garena87
        {
            get => _garena87;
            set
            {
                if (value == _garena87) return;
                _garena87 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
