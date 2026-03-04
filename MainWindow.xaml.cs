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
using System.Diagnostics;

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


        private ICollectionView _subscriptionsView;
        public ICollectionView SubscriptionsView => _subscriptionsView;

        private SortMode _currentSort;

        public ICommand NewSubscriptionCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public AppSettings AppSettings { get; }

        public MainWindow()
        {
            Subscriptions = SubscriptionStorage.Load();
            AppSettings = SettingsStorage.Load();

            InitializeComponent();

            DataContext = this;
  
            if (Enum.TryParse(Properties.Settings.Default.LastSortMode, out SortMode savedSort))
                _currentSort = savedSort;
            else
                _currentSort = SortMode.Name;

            _subscriptionsView = CollectionViewSource.GetDefaultView(Subscriptions);
            _subscriptionsView.Filter = FilterSubscriptions;
            ApplySortingToView();

            NewSubscriptionCommand = new RelayCommand(OpenAddSubscription);
            ClearSearchCommand = new RelayCommand(
                () => SearchText = string.Empty,
                () => !string.IsNullOrWhiteSpace(SearchText)
            );

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

                _subscriptionsView.Refresh();
                OnPropertyChanged(nameof(SubscriptionCount));
                OnPropertyChanged(nameof(SubscriptionCountText));
                OnPropertyChanged(nameof(EmptyMessage));
                OnPropertyChanged(nameof(EmptyMessageVisibility));
            };

            RecalculateTotals();

            _notificationTimer.Interval = TimeSpan.FromHours(1);
            _notificationTimer.Tick += (_, __) => CheckForDueNotifications();

            ApplyNotificationSettings();
            CheckForDueNotifications();
        }

        public ObservableCollection<Subscription> Subscriptions { get; set; }

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

        public bool ShowDisabled
        {
            get => _showDisabled;
            set
            {
                if (_showDisabled == value) return;

                _showDisabled = value;
                _searchText = "";

                OnPropertyChanged(nameof(SearchText));
                OnPropertyChanged(nameof(SubscriptionsHeader));
                OnPropertyChanged(nameof(CalculatorVisibility));

                _subscriptionsView.Refresh();

                OnPropertyChanged(nameof(SubscriptionCount));
                OnPropertyChanged(nameof(SubscriptionCountText));
                OnPropertyChanged(nameof(EmptyMessage));
                OnPropertyChanged(nameof(EmptyMessageVisibility));

                if (!ShowDisabled)
                    RecalculateTotals();
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

                _subscriptionsView.Refresh();

                OnPropertyChanged(nameof(SubscriptionCount));
                OnPropertyChanged(nameof(SubscriptionCountText));
                OnPropertyChanged(nameof(EmptyMessage));
                OnPropertyChanged(nameof(EmptyMessageVisibility));
            }
        }

        public IEnumerable<SortMode> SortModes { get; } =
            new[] { SortMode.Name, SortMode.Cost, SortMode.DaysUntilBilling };

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

                ApplySortingToView();
                _subscriptionsView.Refresh();

                OnPropertyChanged(nameof(CurrentSort));
                OnPropertyChanged(nameof(CurrentSortIndex));
                OnPropertyChanged(nameof(EmptyMessage));
                OnPropertyChanged(nameof(EmptyMessageVisibility));
                OnPropertyChanged(nameof(SubscriptionCountText));
            }
        }
        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedSubs = SubscriptionsListView.SelectedItems
                .Cast<Subscription>()
                .ToList();

            if (!selectedSubs.Any())
                return;

            decimal yearlySavings = selectedSubs
                .Where(s => s.IsActive)
                .Sum(s => s.IsYearly ? s.Cost : s.Cost * 12);

            int count = selectedSubs.Count;

            var confirmWindow = new DeleteConfirmationWindow(count, yearlySavings)
            {
                Owner = this
            };

            bool? result = confirmWindow.ShowDialog();

            if (result != true)
                return;

            foreach (var sub in selectedSubs)
                Subscriptions.Remove(sub);

            SubscriptionStorage.Save(Subscriptions);
            _subscriptionsView.Refresh();
            RecalculateTotals();
        }
        private void SubscriptionsContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (SubscriptionsListView.SelectedItems.Count == 0)
            {
                DeleteMenuItem.Visibility = Visibility.Collapsed;
                DisableMenuItem.Visibility = Visibility.Collapsed;
                ActivateMenuItem.Visibility = Visibility.Collapsed;
                return;
            }

            var selectedSubs = SubscriptionsListView.SelectedItems
                .Cast<Subscription>()
                .ToList();

            bool anyActive = selectedSubs.Any(s => s.IsActive);
            bool anyDisabled = selectedSubs.Any(s => !s.IsActive);

            DeleteMenuItem.Visibility = Visibility.Visible;

            DisableMenuItem.Visibility = anyActive
                ? Visibility.Visible
                : Visibility.Collapsed;

            ActivateMenuItem.Visibility = anyDisabled
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        private void DisableSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedSubs = SubscriptionsListView.SelectedItems
                .Cast<Subscription>()
                .Where(s => s.IsActive)
                .ToList();

            if (!selectedSubs.Any())
                return;

            foreach (var sub in selectedSubs)
                sub.IsActive = false;

            SubscriptionStorage.Save(Subscriptions);
            _subscriptionsView.Refresh();
            RecalculateTotals();
        }
        private void ActivateSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedSubs = SubscriptionsListView.SelectedItems
                .Cast<Subscription>()
                .Where(s => !s.IsActive)
                .ToList();

            if (!selectedSubs.Any())
                return;

            foreach (var sub in selectedSubs)
                sub.IsActive = true;

            SubscriptionStorage.Save(Subscriptions);
            _subscriptionsView.Refresh();
            RecalculateTotals();
        }
        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://subtrackapp.net/support",
                UseShellExecute = true
            });
        }

        public int CurrentSortIndex
        {
            get => (int)CurrentSort;
            set
            {
                if ((int)CurrentSort == value)
                    return;

                CurrentSort = (SortMode)value;
            }
        }

        public decimal YearlyTotal => _yearlyTotal;
        public decimal MonthlyTotal => _monthlyTotal;
        public decimal DisplayedTotal => IsYearly ? YearlyTotal : MonthlyTotal;
        public string TotalLabel => IsYearly ? "Yearly Total" : "Monthly Total";

        public string FormattedDisplayedTotal => $"{AppSettings.CurrencySymbol}{DisplayedTotal:F2}";

        public Visibility CalculatorVisibility => ShowDisabled ? Visibility.Collapsed : Visibility.Visible;

        public string SubscriptionsHeader => ShowDisabled ? "Disabled Subscriptions" : "Active Subscriptions";

        public int SubscriptionCount => _subscriptionsView?.Cast<object>().Count() ?? 0;

        public string SubscriptionCountText
        {
            get
            {
                int count = SubscriptionCount;
                string state = ShowDisabled ? "disabled" : "active";
                string noun = count == 1 ? "subscription" : "subscriptions";
                return $"You have {count} {state} {noun}";
            }
        }

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
            (_subscriptionsView?.Cast<object>().Any() == true)
                ? Visibility.Collapsed
                : Visibility.Visible;

        public string AppVersion
        {
            get
            {
                var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                if (v == null) return "v1.0.0";
                return $"v{v.Major}.{v.Minor}.{v.Build}";
            }
        }

        private bool FilterSubscriptions(object obj)
        {
            if (obj is not Subscription s)
                return false;

            // Active vs Disabled view
            if (ShowDisabled && s.IsActive)
                return false;

            if (!ShowDisabled && !s.IsActive)
                return false;

            // Search
            return MatchesSearch(s);
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

        private void ApplySortingToView()
        {
            _subscriptionsView.SortDescriptions.Clear();

            switch (CurrentSort)
            {
                case SortMode.Cost:
                    _subscriptionsView.SortDescriptions.Add(
                        new SortDescription(nameof(Subscription.Cost), ListSortDirection.Descending));
                    break;

                case SortMode.DaysUntilBilling:
                    _subscriptionsView.SortDescriptions.Add(
                        new SortDescription(nameof(Subscription.DaysUntilBilling), ListSortDirection.Ascending));
                    break;

                default:
                    _subscriptionsView.SortDescriptions.Add(
                        new SortDescription(nameof(Subscription.Name), ListSortDirection.Ascending));
                    break;
            }
        }

        private void RecalculateTotals()
        {
            decimal yearlyTotal = 0m;

            foreach (var sub in Subscriptions.Where(s => s.IsActive))
                yearlyTotal += sub.IsYearly ? sub.Cost : sub.Cost * 12;

            _yearlyTotal = yearlyTotal;
            _monthlyTotal = yearlyTotal / 12;

            OnPropertyChanged(nameof(YearlyTotal));
            OnPropertyChanged(nameof(MonthlyTotal));
            OnPropertyChanged(nameof(DisplayedTotal));
            OnPropertyChanged(nameof(FormattedDisplayedTotal));
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddSubscriptionWindow(Subscriptions, AppSettings) { Owner = this };
            window.ShowDialog();

            _subscriptionsView.Refresh();
            OnPropertyChanged(nameof(SubscriptionCount));
            OnPropertyChanged(nameof(SubscriptionCountText));
            OnPropertyChanged(nameof(EmptyMessage));
            OnPropertyChanged(nameof(EmptyMessageVisibility));

            RecalculateTotals();
        }

        private void OpenAddSubscription()
        {
            var window = new AddSubscriptionWindow(Subscriptions, AppSettings) { Owner = this };
            window.ShowDialog();

            _subscriptionsView.Refresh();
            OnPropertyChanged(nameof(SubscriptionCount));
            OnPropertyChanged(nameof(SubscriptionCountText));
            OnPropertyChanged(nameof(EmptyMessage));
            OnPropertyChanged(nameof(EmptyMessageVisibility));

            RecalculateTotals();
        }

        private void Subscription_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SubscriptionStorage.Save(Subscriptions);

            // Only totals depend on active subs.
            RecalculateTotals();

            // Refresh view for search/sort and active/disabled switches.
            _subscriptionsView.Refresh();

            OnPropertyChanged(nameof(SubscriptionCount));
            OnPropertyChanged(nameof(SubscriptionCountText));
            OnPropertyChanged(nameof(EmptyMessage));
            OnPropertyChanged(nameof(EmptyMessageVisibility));
        }

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

            _subscriptionsView.Refresh();
            OnPropertyChanged(nameof(SubscriptionCount));
            OnPropertyChanged(nameof(SubscriptionCountText));
            OnPropertyChanged(nameof(EmptyMessage));
            OnPropertyChanged(nameof(EmptyMessageVisibility));

            RecalculateTotals();
        }

        private void ShowActive_Click(object sender, RoutedEventArgs e) => ShowDisabled = false;
        private void ShowDisabled_Click(object sender, RoutedEventArgs e) => ShowDisabled = true;

        private void Monthly_Checked(object sender, RoutedEventArgs e) => IsYearly = false;
        private void Yearly_Checked(object sender, RoutedEventArgs e) => IsYearly = true;

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow(AppSettings) { Owner = this };

            if (window.ShowDialog() == true)
            {
                SettingsStorage.Save(AppSettings);

                OnPropertyChanged(nameof(AppSettings));
                _subscriptionsView.Refresh();

                RecalculateTotals();

                OnPropertyChanged(nameof(FormattedDisplayedTotal));

                _subscriptionsView.Refresh();
                OnPropertyChanged(nameof(AppSettings));
                OnPropertyChanged(nameof(SubscriptionCount));
                OnPropertyChanged(nameof(SubscriptionCountText));
                OnPropertyChanged(nameof(EmptyMessage));
                OnPropertyChanged(nameof(EmptyMessageVisibility));

                CheckForDueNotifications();
                ApplyNotificationSettings();
            }
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
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Subscription sub)
            {
                var window = new EditSubscriptionWindow(sub, Subscriptions, AppSettings)
                {
                    Owner = this
                };

                window.ShowDialog();

                _subscriptionsView.Refresh();
                RecalculateTotals();
                OnPropertyChanged(nameof(SubscriptionCount));
                OnPropertyChanged(nameof(SubscriptionCountText));
            }

            e.Handled = true;
        }

        private bool IsWithinQuietHours()
        {
            var now = DateTime.Now.TimeOfDay;
            var start = AppSettings.QuietStart;
            var end = AppSettings.QuietEnd;

            if (start < end)
                return now >= start && now < end;

            return now >= start || now < end;
        }

        private void CheckForDueNotifications()
        {
            if (!AppSettings.NotificationsEnabled)
                return;

            if (IsWithinQuietHours())
                return;

            foreach (var sub in Subscriptions)
            {
                if (!sub.IsActive)
                    continue;

                if (!sub.NotificationsEnabled)
                    continue;

                if (sub.DaysUntilBilling > AppSettings.NotificationDaysBefore)
                    continue;

                if (sub.LastNotifiedDate?.Date == DateTime.Today)
                    continue;

                NotificationService.ShowBillingReminder(sub, AppSettings);

                if (AppSettings.PlaySoundOnNotification)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                }

                sub.LastNotifiedDate = DateTime.Today;
            }

            SubscriptionStorage.Save(Subscriptions);
        }


        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return;

            SearchText = string.Empty;
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
                SearchBox?.Focus();
                SearchBox?.SelectAll();
                e.Handled = true;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}