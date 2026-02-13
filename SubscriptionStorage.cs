using Subscription_Manager.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Subscription_Manager
{
    public static class SubscriptionStorage
    {
        private static readonly string FilePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SubTrack",
                "subscriptions.json"
            );

        public static ObservableCollection<Subscription> Load()
        {
            if (!File.Exists(FilePath))
                return new ObservableCollection<Subscription>();

            return JsonSerializer.Deserialize<ObservableCollection<Subscription>>(
                       File.ReadAllText(FilePath))
                   ?? new ObservableCollection<Subscription>();
        }

        public static void Save(ObservableCollection<Subscription> subscriptions)
        {
            var json = JsonSerializer.Serialize(subscriptions, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }

    }
}
