using Subscription_Manager.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Subscription_Manager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _isYearly;
        private bool _showDisabled;
        private decimal _yearlyTotal;
        private decimal _monthlyTotal;

        public string EmptyMessage =>
            ShowDisabled
                ? "You have no Disabled Subscriptions"
                : "You have no Active Subscriptions";

        public Visibility EmptyMessageVisibility =>
            CurrentSubscriptions.Any()
                ? Visibility.Collapsed
                : Visibility.Visible;

        public bool IsYearly
        {
            get => _isYearly;
            set
            {
                if (_isYearly == value) return;

                _isYearly = value;
                RecalculateTotals();
                OnPropertyChanged(nameof(IsYearly));
                OnPropertyChanged(nameof(TotalLabel));
                OnPropertyChanged(nameof(DisplayedTotal));
            }
        }

        private void RecalculateTotals()
        {
            decimal yearlyTotal = 0m;

            foreach (var sub in ActiveSubscriptions)
            {
                yearlyTotal += sub.IsYearly ? sub.Cost : sub.Cost * 12;
            }

            _yearlyTotal = yearlyTotal;
            _monthlyTotal = yearlyTotal / 12;

            OnPropertyChanged(nameof(YearlyTotal));
            OnPropertyChanged(nameof(MonthlyTotal));
            OnPropertyChanged(nameof(DisplayedTotal));
        }

        public decimal YearlyTotal => _yearlyTotal;
        public decimal MonthlyTotal => _monthlyTotal;
        public decimal DisplayedTotal => IsYearly ? YearlyTotal : MonthlyTotal;
        public string TotalLabel => IsYearly ? "Yearly Total" : "Monthly Total";

        public bool ShowDisabled
        {
            get => _showDisabled;
            set
            {
                if (_showDisabled == value) return;

                _showDisabled = value;

                OnPropertyChanged(nameof(CurrentSubscriptions));
                OnPropertyChanged(nameof(SubscriptionCount));
                OnPropertyChanged(nameof(SubscriptionCountText));
                OnPropertyChanged(nameof(SubscriptionsHeader));
                OnPropertyChanged(nameof(CalculatorVisibility));
                OnPropertyChanged(nameof(EmptyMessage));
                OnPropertyChanged(nameof(EmptyMessageVisibility));

                if (!ShowDisabled)
                    RecalculateTotals();
            }
        }

        public int SubscriptionCount =>
            CurrentSubscriptions?.Count() ?? 0;

        public Visibility CalculatorVisibility =>
            ShowDisabled ? Visibility.Collapsed : Visibility.Visible;

        public ObservableCollection<Subscription> Subscriptions { get; set; }

        public IEnumerable<Subscription> ActiveSubscriptions =>
            Subscriptions?.Where(s => s.IsActive) ?? Enumerable.Empty<Subscription>();

        public IEnumerable<Subscription> DisabledSubscriptions =>
            Subscriptions?.Where(s => !s.IsActive) ?? Enumerable.Empty<Subscription>();

        public IEnumerable<Subscription> CurrentSubscriptions =>
            ShowDisabled ? DisabledSubscriptions : ActiveSubscriptions;

        public string SubscriptionsHeader =>
            ShowDisabled ? "Disabled Subscriptions" : "Active Subscriptions";

        // ---------- OPTIONAL 2 (Pluralization) ----------

        private static string Pluralize(int count, string word)
        {
            return count == 1 ? word : word + "s";
        }

        public string SubscriptionCountText
        {
            get
            {
                int count = SubscriptionCount;
                string state = ShowDisabled ? "disabled" : "active";
                string noun = Pluralize(count, "subscription");

                return $"You have {count} {state} {noun}";
            }
        }
        public string AppVersion
        {
            get
            {
                var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                if (v == null) return "v1.0.0";
                return $"v{v.Major}.{v.Minor}.{v.Build}";
            }
        }


        // ---------- UI EVENTS ----------

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddSubscriptionWindow(Subscriptions)
            {
                Owner = this
            };

            window.ShowDialog();

            OnPropertyChanged(nameof(CurrentSubscriptions));
            OnPropertyChanged(nameof(SubscriptionCount));
            OnPropertyChanged(nameof(SubscriptionCountText));
            RecalculateTotals();
        }

        private void Subscription_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SubscriptionStorage.Save(Subscriptions);
            OnPropertyChanged(nameof(EmptyMessageVisibility));
            OnPropertyChanged(nameof(SubscriptionCountText));
        }

        public MainWindow()
        {
            Subscriptions = SubscriptionStorage.Load();

            InitializeComponent();
            DataContext = this;

            foreach (var sub in Subscriptions)
            {
                sub.PropertyChanged += Subscription_PropertyChanged;
            }

            Subscriptions.CollectionChanged += (_, e) =>
            {
                if (e.NewItems != null)
                    foreach (Subscription sub in e.NewItems)
                        sub.PropertyChanged += Subscription_PropertyChanged;

                if (e.OldItems != null)
                    foreach (Subscription sub in e.OldItems)
                        sub.PropertyChanged -= Subscription_PropertyChanged;

                SubscriptionStorage.Save(Subscriptions);

                RecalculateTotals();
                OnPropertyChanged(nameof(CurrentSubscriptions));
                OnPropertyChanged(nameof(SubscriptionCount));
                OnPropertyChanged(nameof(SubscriptionCountText));
                OnPropertyChanged(nameof(EmptyMessageVisibility));
            };

            RecalculateTotals();
        }

        private void Monthly_Checked(object sender, RoutedEventArgs e) =>
            IsYearly = false;

        private void Yearly_Checked(object sender, RoutedEventArgs e) =>
            IsYearly = true;

        private void ShowActive_Click(object sender, RoutedEventArgs e) =>
            ShowDisabled = false;

        private void ShowDisabled_Click(object sender, RoutedEventArgs e) =>
            ShowDisabled = true;

        private void Subscriptions_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListView listView)
                return;

            if (listView.SelectedItem is not Subscription selectedSubscription)
                return;

            var window = new EditSubscriptionWindow(selectedSubscription, Subscriptions)
            {
                Owner = this
            };

            window.ShowDialog();

            OnPropertyChanged(nameof(CurrentSubscriptions));
            OnPropertyChanged(nameof(SubscriptionCount));
            OnPropertyChanged(nameof(SubscriptionCountText));
            RecalculateTotals();
            OnPropertyChanged(nameof(EmptyMessageVisibility));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}