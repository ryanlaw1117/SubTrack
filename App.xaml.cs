using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Windows;

namespace Subscription_Manager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                });
            };

            var window = new MainWindow();
            window.Show();
        }
    }
}