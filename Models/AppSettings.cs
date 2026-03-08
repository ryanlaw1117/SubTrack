using System;
using System.ComponentModel;

namespace Subscription_Manager.Models
{
    public class AppSettings : INotifyPropertyChanged
    {
        private string _currencySymbol = "$";
        private bool _notificationsEnabled = true;
        private int _notificationDaysBefore = 0;
        private bool _playSoundOnNotification = false;
        private TimeSpan _quietStart = new TimeSpan(22, 0, 0);
        private TimeSpan _quietEnd = new TimeSpan(7, 0, 0);
        private string _dateFormat = "MMM d, yyyy";

        public bool ShowNotificationPreferences => NotificationsEnabled;

        public bool CompactMode { get; set; }

        public string DateFormat
        {
            get => _dateFormat;
            set
            {
                if (_dateFormat == value) return;
                _dateFormat = value;
                OnPropertyChanged(nameof(DateFormat));
            }
        }

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
                OnPropertyChanged(nameof(ShowNotificationPreferences));
            }
        }

        public int NotificationDaysBefore
        {
            get => _notificationDaysBefore;
            set
            {
                if (_notificationDaysBefore == value) return;
                _notificationDaysBefore = value;
                OnPropertyChanged(nameof(NotificationDaysBefore));
            }
        }

        public bool PlaySoundOnNotification
        {
            get => _playSoundOnNotification;
            set
            {
                if (_playSoundOnNotification == value) return;
                _playSoundOnNotification = value;
                OnPropertyChanged(nameof(PlaySoundOnNotification));
            }
        }

        public TimeSpan QuietStart
        {
            get => _quietStart;
            set
            {
                if (_quietStart == value) return;
                _quietStart = value;
                OnPropertyChanged(nameof(QuietStart));
            }
        }

        public TimeSpan QuietEnd
        {
            get => _quietEnd;
            set
            {
                if (_quietEnd == value) return;
                _quietEnd = value;
                OnPropertyChanged(nameof(QuietEnd));
            }
        }

        public AppSettings Clone() => new AppSettings
        {
            CurrencySymbol = this.CurrencySymbol,
            NotificationsEnabled = this.NotificationsEnabled,
            NotificationDaysBefore = this.NotificationDaysBefore,
            PlaySoundOnNotification = this.PlaySoundOnNotification,
            QuietStart = this.QuietStart,
            QuietEnd = this.QuietEnd,
            CompactMode = this.CompactMode,
            DateFormat = this.DateFormat
        };

        public void CopyFrom(AppSettings other)
        {
            CurrencySymbol = other.CurrencySymbol;
            NotificationsEnabled = other.NotificationsEnabled;
            NotificationDaysBefore = other.NotificationDaysBefore;
            PlaySoundOnNotification = other.PlaySoundOnNotification;
            QuietStart = other.QuietStart;
            QuietEnd = other.QuietEnd;
            CompactMode = other.CompactMode;
            DateFormat = other.DateFormat;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}