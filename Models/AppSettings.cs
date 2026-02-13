using System.ComponentModel;

namespace Subscription_Manager.Models
{
    public class AppSettings : INotifyPropertyChanged
    {
        private string _currencySymbol = "$";
        private bool _notificationsEnabled = true;


        public string CurrencySymbol
        {
            get => _currencySymbol;
            set
            {
                if (_currencySymbol == value) return;
                _currencySymbol = value;
                OnPropertyChanged(nameof(CurrencySymbol));
            }
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (_notificationsEnabled == value) return;
                _notificationsEnabled = value;
                OnPropertyChanged(nameof(NotificationsEnabled));
            }
        }

        public AppSettings Clone() => new AppSettings
        {
            CurrencySymbol = CurrencySymbol,
            NotificationsEnabled = NotificationsEnabled
        };

        public void CopyFrom(AppSettings other)
        {
            CurrencySymbol = other.CurrencySymbol;
            NotificationsEnabled = other.NotificationsEnabled;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
