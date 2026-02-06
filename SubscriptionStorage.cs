using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Subscription_Manager.Models;

namespace Subscription_Manager
{
    public static class SubscriptionStorage
    {
        private static readonly string FolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SubTrack");

        private static readonly string FilePath =
            Path.Combine(FolderPath, "subscriptions.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public static ObservableCollection<Subscription> Load()
        {
            try
            {
                Directory.CreateDirectory(FolderPath);

                if (!File.Exists(FilePath))
                    return new ObservableCollection<Subscription>();

                var json = File.ReadAllText(FilePath);

                var list = JsonSerializer.Deserialize<ObservableCollection<Subscription>>(json, JsonOptions)
                           ?? new ObservableCollection<Subscription>();

                foreach (var s in list)
                {
                    if (s.FirstBillingDate == DateTime.MinValue)
                        s.FirstBillingDate = DateTime.Today;

                    s.UpdateNextBillingDate();
                }

                return list;
            }
            catch
            {
                return new ObservableCollection<Subscription>();
            }
        }


        public static void Save(ObservableCollection<Subscription> subscriptions)
        {
            Directory.CreateDirectory(FolderPath);

            var json = JsonSerializer.Serialize(subscriptions, JsonOptions);
            File.WriteAllText(FilePath, json);
        }
    }
}
