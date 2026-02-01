using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Subscription_Manager.Models;

namespace Subscription_Manager
{
    public partial class EditSubscriptionWindow : Window
    {
        private readonly Subscription _subscription;
        private readonly ObservableCollection<Subscription> _subscriptions;

        public EditSubscriptionWindow(Subscription subscription, ObservableCollection<Subscription> subscriptions)
        {
            InitializeComponent();

            _subscription = subscription;
            _subscriptions = subscriptions;

            NameBox.Text = _subscription.Name;
            CostBox.Text = _subscription.Cost.ToString("0.##");
            DescriptionBox.Text = _subscription.Description ?? "";
            BillingDatePicker.SelectedDate = _subscription.FirstBillingDate == DateTime.MinValue ? DateTime.Today : _subscription.FirstBillingDate;

            MonthlyRadio.IsChecked = !_subscription.IsYearly;
            YearlyRadio.IsChecked = _subscription.IsYearly;

            ActiveCheck.IsChecked = _subscription.IsActive;
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

            _subscription.UpdateNextBillingDate();

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var cycle = _subscription.IsYearly ? "per year" : "per month";
            var savings = _subscription.Cost.ToString("0.00");

            var result = MessageBox.Show(
                $"Are you sure you want to delete this subscription?\nDeleting this will save you ${savings} {cycle}.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            _subscriptions.Remove(_subscription);
            Close();
        }

        private void CostBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !decimal.TryParse(((TextBox)sender).Text + e.Text, out _);
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