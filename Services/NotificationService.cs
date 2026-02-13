using Microsoft.Toolkit.Uwp.Notifications;
using Subscription_Manager.Models;

namespace Subscription_Manager.Services
{
    public static class NotificationService
    {
        public static void ShowBillingReminder(Subscription sub, AppSettings settings)
        {
            if (settings == null || !settings.NotificationsEnabled)
                return;

            int days = sub.DaysUntilBilling;

            string message = days switch
            {
                0 => $"{sub.Name} is due today.",
                1 => $"{sub.Name} is due tomorrow.",
                _ => $"{sub.Name} is due in {days} days."
            };

            new ToastContentBuilder()
                .AddText("Subscription Reminder")
                .AddText(message)
                .Show();
        }
    }
}