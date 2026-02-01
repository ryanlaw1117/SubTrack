using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Subscription_Manager.Models;

namespace Subscription_Manager
{
    public partial class AddSubscriptionWindow : Window
    {
        private readonly ObservableCollection<Subscription> _subscriptions;

        public AddSubscriptionWindow(ObservableCollection<Subscription> subscriptions)
        {
            InitializeComponent();
            _subscriptions = subscriptions;
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
                IsActive = true
            };

            _subscriptions.Add(subscription);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CostBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !decimal.TryParse(
                ((TextBox)sender).Text + e.Text,
                out _
            );
        }

        private void CostBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CostBox.Text == "")
                return;

            if (!decimal.TryParse(CostBox.Text, out _))
                CostBox.Text = CostBox.Text.Remove(CostBox.Text.Length - 1);
        }
        private void InputChanged(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;
        }

    }

}