using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Subscription_Manager.Models;

namespace Subscription_Manager
{
    public partial class AddSubscriptionWindow : Window
    {
        private readonly ObservableCollection<Subscription> _subscriptions;
        private string? _selectedAccentColor;
        private readonly Subscription _subscription;
        public AppSettings AppSettings { get; }

        public AddSubscriptionWindow(
            ObservableCollection<Subscription> subscriptions,
            AppSettings appSettings)
        {
            InitializeComponent();

            _subscriptions = subscriptions;
            AppSettings = appSettings;

            _subscription = new Subscription();
            DataContext = _subscription;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = Color.FromRgb(
                    dialog.Color.R,
                    dialog.Color.G,
                    dialog.Color.B);

                _selectedAccentColor = color.ToString();

                ColorButton.Background = new SolidColorBrush(color);
                ColorButton.Foreground = Brushes.White;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                !decimal.TryParse(CostBox.Text, out var cost) ||
                BillingDatePicker.SelectedDate == null)
            {
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            var firstDate = BillingDatePicker.SelectedDate.Value;

            _subscription.Name = NameBox.Text;
            _subscription.Cost = cost;
            _subscription.FirstBillingDate = firstDate;
            _subscription.IsYearly = YearlyRadio.IsChecked == true;
            _subscription.Description = DescriptionBox.Text;
            _subscription.IsActive = true;
            _subscription.AccentColor = string.IsNullOrWhiteSpace(_selectedAccentColor)
                ? _subscription.AccentColor
                : _selectedAccentColor;

            _subscriptions.Add(_subscription);
            _subscription.IsActive = true;
            _subscription.NotificationsEnabled = AppSettings.NotificationsEnabled;

            Subscription_Manager.SubscriptionStorage.Save(_subscriptions);

            Close();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Add_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void CostBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !decimal.TryParse(
                ((TextBox)sender).Text + e.Text,
                out _);
        }

        private void InputChanged(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;
        }
    }
}