using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Subscription_Manager.Models;
using Subscription_Manager.Services;


namespace Subscription_Manager
{
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _target;
        private readonly AppSettings _draft;

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();

            _target = settings;
            _draft = settings.Clone();

            DataContext = _draft;

            PreviewKeyDown += SettingsWindow_PreviewKeyDown;
        }

        private void SettingsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                e.Handled = true;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (_draft.QuietStart == _draft.QuietEnd)
            {
                MessageBox.Show(
                    "Quiet hours start and end time cannot be the same.",
                    "Invalid Quiet Hours",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (_draft.NotificationDaysBefore < 0)
            {
                MessageBox.Show(
                    "Notification days before billing cannot be negative.",
                    "Invalid Notification Setting",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            _target.CopyFrom(_draft);
            SettingsStorage.Save(_target);

            if (_target.NotificationsEnabled)
                TaskSchedulerService.EnsureTaskExists();
            else
                TaskSchedulerService.RemoveTask();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CurrencySymbol_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[\p{L}\p{Nd}\s]");
        }
    }
}
