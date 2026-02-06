using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Subscription_Manager.Models
{
    public class Subscription : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private decimal _cost;
        private bool _isYearly;
        private bool _isActive;
        private DateTime _firstBillingDate;
        private DateTime _nextBillingDate;
        private string _description = string.Empty;
        private string _category = string.Empty;
        private string? _accentColor;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public decimal Cost
        {
            get => _cost;
            set { _cost = value; OnPropertyChanged(nameof(Cost)); }
        }

        public bool IsYearly
        {
            get => _isYearly;
            set
            {
                _isYearly = value;
                UpdateNextBillingDate();
                OnPropertyChanged(nameof(IsYearly));
                OnPropertyChanged(nameof(BillingCycle));
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(nameof(IsActive)); }
        }

        public DateTime FirstBillingDate
        {
            get => _firstBillingDate;
            set
            {
                _firstBillingDate = value;
                UpdateNextBillingDate();
                OnPropertyChanged(nameof(FirstBillingDate));
            }
        }

        public DateTime NextBillingDate
        {
            get => _nextBillingDate;
            private set
            {
                _nextBillingDate = value;
                OnPropertyChanged(nameof(NextBillingDate));
                OnPropertyChanged(nameof(DaysUntilBilling));
                OnPropertyChanged(nameof(DaysUntilBillingText));
                OnPropertyChanged(nameof(BillingWarningBrush));
            }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value ?? string.Empty;
                OnPropertyChanged(nameof(Category));
            }
        }

        public string? AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                OnPropertyChanged(nameof(AccentColor));
                OnPropertyChanged(nameof(AccentBrush));
            }
        }

        [JsonIgnore]
        public Brush AccentBrush =>
            string.IsNullOrWhiteSpace(AccentColor)
                ? Brushes.Transparent
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString(AccentColor));

        public string BillingCycle => IsYearly ? "Yearly" : "Monthly";

        public int DaysUntilBilling => (NextBillingDate.Date - DateTime.Today).Days;

        public string DaysUntilBillingText
        {
            get
            {
                if (DaysUntilBilling == 0) return "Due today";
                if (DaysUntilBilling == 1) return "1 day";
                if (DaysUntilBilling < 0) return $"Overdue {Math.Abs(DaysUntilBilling)} days";
                return $"{DaysUntilBilling} days";
            }
        }

        [JsonIgnore]
        public Brush BillingWarningBrush
        {
            get
            {
                if (DaysUntilBilling <= 0) return Brushes.Red;
                if (DaysUntilBilling <= 3) return Brushes.Red;
                if (DaysUntilBilling <= 7) return Brushes.Goldenrod;
                return Brushes.Gray;
            }
        }

        public void UpdateNextBillingDate()
        {
           
            if (FirstBillingDate == DateTime.MinValue)
            {
                NextBillingDate = DateTime.Today;
                return;
            }

            var date = FirstBillingDate.Date;
            var today = DateTime.Today;

            
            while (date < today)
                date = IsYearly ? date.AddYears(1) : date.AddMonths(1);

            NextBillingDate = date;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
