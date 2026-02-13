using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Subscription_Manager.Models;

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
            _target.CopyFrom(_draft);
            SettingsStorage.Save(_target);
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
