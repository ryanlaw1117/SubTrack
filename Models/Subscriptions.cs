using System;
using System.ComponentModel;

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
                OnPropertyChanged(nameof(DaysUntilBilling));
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
                OnPropertyChanged(nameof(DaysUntilBilling));
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
            }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public string BillingCycle => IsYearly ? "Yearly" : "Monthly";

        public int DaysUntilBilling =>
            (NextBillingDate.Date - DateTime.Today).Days;

        public void UpdateNextBillingDate()
        {
            var date = FirstBillingDate;

            while (date.Date < DateTime.Today)
            {
                date = IsYearly
                    ? date.AddYears(1)
                    : date.AddMonths(1);
            }

            NextBillingDate = date;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}