using Microsoft.Toolkit.Uwp.Notifications;
using Subscription_Manager.Properties;
using Subscription_Manager.Services;
using System;
using System.Linq;
using System.Windows;


namespace Subscription_Manager
{
    public partial class App : Application
    {

        private void RunNotificationMode()
        {
           
            try
            {
                var subscriptions = SubscriptionStorage.Load();
                var settings = SettingsStorage.Load();

                if (subscriptions == null || subscriptions.Count == 0)
                    return;

                if (settings == null)
                    return;

                bool updated = false;

                foreach (var sub in subscriptions)
                {
                    if (!sub.IsActive)
                        continue;

                    if (!sub.NotificationsEnabled)
                        continue;

                    int days = sub.DaysUntilBilling;

                    if (days != 0 && days != 1)
                        continue;

                    if (sub.LastNotifiedDate.HasValue &&
                        sub.LastNotifiedDate.Value.Date == DateTime.Today)
                        continue;

                    NotificationService.ShowBillingReminder(sub, settings);

                    sub.LastNotifiedDate = DateTime.Now;
                    updated = true;
                }

                if (updated)
                    SubscriptionStorage.Save(subscriptions);
            }
            catch
            {
               
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Contains("--notify"))
            {
                RunNotificationMode();
                Shutdown();
                return;
            }

            base.OnStartup(e);

            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var args = ToastArguments.Parse(toastArgs.Argument);

                    if (args.Contains("action") && args["action"] == "open")
                    {
                        if (Application.Current.MainWindow == null)
                        {
                            var window = new MainWindow();
                            Application.Current.MainWindow = window;
                            window.Show();
                        }
                        else
                        {
                            Application.Current.MainWindow.Show();
                            Application.Current.MainWindow.Activate();
                        }
                    }
                });
            };

            var window = new MainWindow();
            window.Show();
        }
    }
}