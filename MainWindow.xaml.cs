using Subscription_Manager.Models;
using Subscription_Manager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Subscription_Manager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _isYearly;
        private bool _showDisabled;
        private decimal _yearlyTotal;
        private decimal _monthlyTotal;
        private string _searchText = "";
        private readonly DispatcherTimer _notificationTimer = new();

        public ICommand NewSubscriptionCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public AppSettings AppSettings { get; }

        public string FormattedDisplayedTotal =>
            $"{AppSettings.CurrencySymbol}{DisplayedTotal:F2}";

        public string EmptyMessage
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                    return "No results match your search";

                return ShowDisabled
                    ? "You have no Disabled Subscriptions"
                    : "You have no Active Subscriptions";
            }
        }

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
                OnPropertyChanged(nameof(FormattedDisplayedTotal));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;

                _searchText = value;

                OnPropertyChanged(nameof(SearchText));
                OnPropertyChanged(nameof(CurrentSubscriptions));
                OnPropertyChanged(nameof(SubscriptionCount));
                OnPropertyChanged(nameof(SubscriptionCountText));
                OnPropertyChanged(nameof(EmptyMessage));
                OnPropertyChanged(nameof(EmptyMessageVisibility));
            }
        }

        private bool MatchesSearch(Subscription s)
        {
            if (s == null) return false;

            var q = (SearchText ?? "").Trim();
            if (q.Length == 0) return true;

            var name = s.Name ?? "";
            var desc = s.Description ?? "";

            return name.Contains(q, StringComparison.OrdinalIgnoreCase)
                || desc.Contains(q, StringComparison.OrdinalIgnoreCase);
        }

        private void RecalculateTotals()
        {
            decimal yearlyTotal = 0m;

            foreach (var sub in ActiveSubscriptions)
                yearlyTotal += sub.IsYearly ? sub.Cost : sub.Cost * 12;

            _yearlyTotal = yearlyTotal;
            _monthlyTotal = yearlyTotal / 12;

            OnPropertyChanged(nameof(YearlyTotal));
            OnPropertyChanged(nameof(MonthlyTotal));
            OnPropertyChanged(nameof(DisplayedTotal));
            OnPropertyChanged(nameof(FormattedDisplayedTotal));
        }

        private void RootGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SearchBox == null)
                return;

            if (SortCombo != null && SortCombo.IsDropDownOpen)
                return;

            var src = e.OriginalSource as DependencyObject;
            if (src == null)
                return;

            if (IsDescendantOf(src, SearchBox))
                return;

            if (SortCombo != null && IsDescendantOf(src, SortCombo))
                return;

            Keyboard.ClearFocus();
        }

        private static bool IsDescendantOf(DependencyObject child, DependencyObject parent)
        {
            while (child != null)
            {
                if (child == parent) return true;
                child = VisualTreeHelper.GetParent(child);
            }
            return false;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.N)
            {
                Add_Click(this, new RoutedEventArgs());
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchText = string.Empty;
                    FocusManager.SetFocusedElement(this, this);
                    Keyboard.ClearFocus();
                    e.Handled = true;
                }
            }

            if (Keyboard.Modifiers == ModifierKeys.None && (e.Key == Key.Oem2 || e.Key == Key.Divide))
            {
                if (SearchBox != null && !SearchBox.IsKeyboardFocusWithin)
                {
                    SearchBox.Focus();
                    SearchBox.SelectAll();
                    e.Handled = true;
                }
                return;
            }

            if (e.Key == Key.Oem2 && Keyboard.Modifiers == ModifierKeys.None)
            {
                SearchBox.Focus();
                SearchBox.SelectAll();
                e.Handled = true;
            }
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
                _searchText = "";
                OnPropertyChanged(nameof(SearchText));
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

        public int SubscriptionCount => CurrentSubscriptions?.Count() ?? 0;

        public Visibility CalculatorVisibility =>
            ShowDisabled ? Visibility.Collapsed : Visibility.Visible;

        public ObservableCollection<Subscription> Subscriptions { get; set; }

        public IEnumerable<Subscription> ActiveSubscriptions =>
            ApplySorting(
                Subscriptions?
                    .Where(s => s.IsActive)
                    .Where(MatchesSearch)
                ?? Enumerable.Empty<Subscription>()
            );

        public IEnumerable<Subscription> DisabledSubscriptions =>
            ApplySorting(
                Subscriptions?
                    .Where(s => !s.IsActive)
                    .Where(MatchesSearch)
                ?? Enumerable.Empty<Subscription>()
            );

        private IEnumerable<Subscription> ApplySorting(IEnumerable<Subscription> source)
        {
            return CurrentSort switch
            {
                SortMode.Cost => source.OrderByDescending(s => s.Cost),
                SortMode.DaysUntilBilling => source.OrderBy(s => s.DaysUntilBilling),
                _ => source.OrderBy(s => s.Name?.Trim(), StringComparer.OrdinalIgnoreCase),
            };
        }

        public IEnumerable<Subscription> CurrentSubscriptions =>
            ShowDisabled ? DisabledSubscriptions : ActiveSubscriptions;

        public string SubscriptionsHeader =>
            ShowDisabled ? "Disabled Subscriptions" : "Active Subscriptions";

        private static string Pluralize(int count, string word) =>
            count == 1 ? word : word + "s";

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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddSubscriptionWindow(Subscriptions, AppSettings)
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

            RecalculateTotals();

            OnPropertyChanged(nameof(CurrentSubscriptions));
            OnPropertyChanged(nameof(EmptyMessageVisibility));
            OnPropertyChanged(nameof(SubscriptionCountText));
        }

        private void OpenAddSubscription()
        {
            var window = new AddSubscriptionWindow(Subscriptions, AppSettings)
            {
                Owner = this
            };

            window.ShowDialog();

            OnPropertyChanged(nameof(CurrentSubscriptions));
            OnPropertyChanged(nameof(SubscriptionCount));
            OnPropertyChanged(nameof(SubscriptionCountText));
            RecalculateTotals();
        }
        private void ApplyNotificationSettings()
        {
            if (AppSettings.NotificationsEnabled)
            {
                if (!_notificationTimer.IsEnabled)
                    _notificationTimer.Start();

                CheckForDueNotifications();
            }
            else
            {
                if (_notificationTimer.IsEnabled)
                    _notificationTimer.Stop();
            }
        }
        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow(AppSettings) { Owner = this };

            if (window.ShowDialog() == true)
            {
                SettingsStorage.Save(AppSettings);

                RecalculateTotals();

                OnPropertyChanged(nameof(FormattedDisplayedTotal));
                OnPropertyChanged(nameof(CurrentSubscriptions));
                OnPropertyChanged(nameof(SubscriptionCountText));

                CheckForDueNotifications();
            }
        }
        public IEnumerable<SortMode> SortModes { get; } =
            new[] { SortMode.Name, SortMode.Cost, SortMode.DaysUntilBilling };

        public MainWindow()
        {
            Subscriptions = SubscriptionStorage.Load();
            AppSettings = SettingsStorage.Load();

            InitializeComponent();
            CheckForDueNotifications();

            DataContext = this;
            NewSubscriptionCommand = new RelayCommand(OpenAddSubscription);
            ClearSearchCommand = new RelayCommand(
                () => SearchText = string.Empty,
                () => !string.IsNullOrWhiteSpace(SearchText)
            );

            
            if (Enum.TryParse(Properties.Settings.Default.LastSortMode, out SortMode savedSort))
                _currentSort = savedSort;
            else
                _currentSort = SortMode.Name;

            
            foreach (var sub in Subscriptions)
                sub.PropertyChanged += Subscription_PropertyChanged;

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

            CheckForDueNotifications();

            _notificationTimer.Interval = TimeSpan.FromHours(1);
            _notificationTimer.Tick += (_, __) => CheckForDueNotifications();

            ApplyNotificationSettings();
        }
        private void CheckForDueNotifications()
        {
            if (!AppSettings.NotificationsEnabled)
                return;

            foreach (var sub in Subscriptions)
            {
                if (!sub.IsActive)
                    continue;

                if (!sub.NotificationsEnabled)
                    continue;

                if (sub.DaysUntilBilling != 0)
                    continue;


                if (sub.LastNotifiedDate?.Date == DateTime.Today)
                    continue;

                NotificationService.ShowBillingReminder(sub, AppSettings);

                sub.LastNotifiedDate = DateTime.Today;
            }

            SubscriptionStorage.Save(Subscriptions);
        }

        private void Monthly_Checked(object sender, RoutedEventArgs e) => IsYearly = false;
        private void Yearly_Checked(object sender, RoutedEventArgs e) => IsYearly = true;
        private void ShowActive_Click(object sender, RoutedEventArgs e) => ShowDisabled = false;
        private void ShowDisabled_Click(object sender, RoutedEventArgs e) => ShowDisabled = true;

        private void Subscriptions_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListView listView)
                return;

            if (listView.SelectedItem is not Subscription selectedSubscription)
                return;

            var window = new EditSubscriptionWindow(selectedSubscription, Subscriptions, AppSettings)
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

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return;

            SearchText = string.Empty;
        }

        private SortMode _currentSort;

        public SortMode CurrentSort
        {
            get => _currentSort;
            set
            {
                if (_currentSort == value)
                    return;

                _currentSort = value;

                Properties.Settings.Default.LastSortMode = _currentSort.ToString();
                Properties.Settings.Default.Save();

                OnPropertyChanged(nameof(CurrentSort));
                OnPropertyChanged(nameof(CurrentSubscriptions));
                OnPropertyChanged(nameof(EmptyMessageVisibility));
                OnPropertyChanged(nameof(SubscriptionCountText));
            }
        }

        public int CurrentSortIndex
        {
            get => (int)CurrentSort;
            set
            {
                if ((int)CurrentSort == value)
                    return;

                CurrentSort = (SortMode)value;
                OnPropertyChanged(nameof(CurrentSortIndex));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
