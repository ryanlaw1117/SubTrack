using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Subscription_Manager.Models;

namespace Subscription_Manager
{
    public partial class EditSubscriptionWindow : Window
    {
        private readonly Subscription _subscription;
        private readonly ObservableCollection<Subscription> _subscriptions;

        private Subscription _draft;
        private string? _selectedAccentColor;
        public AppSettings AppSettings { get; }

        public EditSubscriptionWindow(Subscription subscription, ObservableCollection<Subscription> subscriptions, AppSettings appSettings)
        {
            InitializeComponent();
            _subscription = subscription;
            _subscriptions = subscriptions;
            AppSettings = appSettings;

            _draft = new Subscription
            {
                Name = _subscription.Name,
                Cost = _subscription.Cost,
                IsYearly = _subscription.IsYearly,
                IsActive = _subscription.IsActive,
                NotificationsEnabled = _subscription.NotificationsEnabled,
                FirstBillingDate = _subscription.FirstBillingDate,
                Description = _subscription.Description ?? string.Empty,
                Category = _subscription.Category ?? string.Empty,
                AccentColor = _subscription.AccentColor ?? string.Empty
            };

            _selectedAccentColor = _draft.AccentColor;

            DataContext = _draft;

            NameBox.Text = _draft.Name;
            CostBox.Text = _draft.Cost.ToString("0.##");
            DescriptionBox.Text = _draft.Description ?? "";

            BillingDatePicker.SelectedDate =
                _draft.FirstBillingDate == DateTime.MinValue
                    ? DateTime.Today
                    : _draft.FirstBillingDate;

            MonthlyRadio.IsChecked = !_draft.IsYearly;
            YearlyRadio.IsChecked = _draft.IsYearly;
            ActiveCheck.IsChecked = _draft.IsActive;

            if (!string.IsNullOrWhiteSpace(_draft.Category))
                CategoryBox.SelectedItem = _draft.Category;

            if (!string.IsNullOrWhiteSpace(_draft.AccentColor))
            {
                var color = (Color)ColorConverter.ConvertFromString(_draft.AccentColor);
                ColorButton.Background = new SolidColorBrush(color);
                ColorButton.Foreground = Brushes.White;
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = Color.FromRgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);
                _selectedAccentColor = color.ToString();

                ColorButton.Background = new SolidColorBrush(color);
                ColorButton.Foreground = Brushes.White;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                !decimal.TryParse(CostBox.Text, out var cost) ||
                BillingDatePicker.SelectedDate == null)
            {
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            _draft.Name = NameBox.Text.Trim();
            _draft.Cost = cost;
            _draft.IsYearly = YearlyRadio.IsChecked == true;
            _draft.IsActive = ActiveCheck.IsChecked == true;
            _draft.FirstBillingDate = BillingDatePicker.SelectedDate.Value;
            _draft.Description = DescriptionBox.Text ?? "";
            _draft.Category = CategoryBox.SelectedItem as string ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(_selectedAccentColor))
                _draft.AccentColor = _selectedAccentColor;

            _subscription.Name = _draft.Name;
            _subscription.Cost = _draft.Cost;
            _subscription.IsYearly = _draft.IsYearly;
            _subscription.IsActive = _draft.IsActive;
            _subscription.NotificationsEnabled = _draft.NotificationsEnabled;
            _subscription.FirstBillingDate = _draft.FirstBillingDate;
            _subscription.Description = _draft.Description;
            _subscription.Category = _draft.Category;
            _subscription.AccentColor = _draft.AccentColor;

            _subscription.UpdateNextBillingDate();

            Subscription_Manager.SubscriptionStorage.Save(_subscriptions);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Delete this subscription?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            _subscriptions.Remove(_subscription);
            Subscription_Manager.SubscriptionStorage.Save(_subscriptions);
            DialogResult = true;
            Close();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Cancel_Click(this, new RoutedEventArgs());
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                Save_Click(this, new RoutedEventArgs());
                e.Handled = true;
                return;
            }

            base.OnPreviewKeyDown(e);
        }

        private void CostBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !decimal.TryParse(((TextBox)sender).Text + e.Text, out _);
        }

        private void InputChanged(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;
        }
    }
}