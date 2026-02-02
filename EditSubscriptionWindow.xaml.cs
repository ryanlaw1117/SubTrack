using System;
using System.Collections.ObjectModel;
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
        private string? _selectedAccentColor;

        public EditSubscriptionWindow(Subscription subscription, ObservableCollection<Subscription> subscriptions)
        {
            InitializeComponent();

            _subscription = subscription;
            _subscriptions = subscriptions;

            NameBox.Text = _subscription.Name;
            CostBox.Text = _subscription.Cost.ToString("0.##");
            DescriptionBox.Text = _subscription.Description ?? "";

            BillingDatePicker.SelectedDate =
                _subscription.FirstBillingDate == DateTime.MinValue
                    ? DateTime.Today
                    : _subscription.FirstBillingDate;

            MonthlyRadio.IsChecked = !_subscription.IsYearly;
            YearlyRadio.IsChecked = _subscription.IsYearly;
            ActiveCheck.IsChecked = _subscription.IsActive;

            _selectedAccentColor = _subscription.AccentColor;

            if (!string.IsNullOrWhiteSpace(_selectedAccentColor))
            {
                var color = (Color)ColorConverter.ConvertFromString(_selectedAccentColor);
                ColorButton.Background = new SolidColorBrush(color);
                ColorButton.Foreground = Brushes.White;
            }
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

            _subscription.Name = NameBox.Text;
            _subscription.Cost = cost;
            _subscription.IsYearly = YearlyRadio.IsChecked == true;
            _subscription.FirstBillingDate = BillingDatePicker.SelectedDate.Value;
            _subscription.Description = DescriptionBox.Text;
            _subscription.IsActive = ActiveCheck.IsChecked == true;
            _subscription.AccentColor = _selectedAccentColor;

            _subscription.UpdateNextBillingDate();
            SubscriptionStorage.Save(_subscriptions);

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

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
            SubscriptionStorage.Save(_subscriptions);
            Close();
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