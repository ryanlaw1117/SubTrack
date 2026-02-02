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

        public AddSubscriptionWindow(ObservableCollection<Subscription> subscriptions)
        {
            InitializeComponent();
            _subscriptions = subscriptions;
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

            var subscription = new Subscription
            {
                Name = NameBox.Text,
                Cost = cost,
                FirstBillingDate = firstDate,
                IsYearly = YearlyRadio.IsChecked == true,
                Description = DescriptionBox.Text,
                IsActive = true,
                AccentColor = _selectedAccentColor
            };

            _subscriptions.Add(subscription);
            SubscriptionStorage.Save(_subscriptions);
            Close();
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