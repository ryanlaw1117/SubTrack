using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Subscription_Manager.Models;

namespace Subscription_Manager
{
    public static class SubscriptionStorage
    {
        private static readonly string FolderPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SubTrack");

        private static readonly string FilePath =
            Path.Combine(FolderPath, "subscriptions.json");

        public static ObservableCollection<Subscription> Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new ObservableCollection<Subscription>();

                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<ObservableCollection<Subscription>>(json)
                       ?? new ObservableCollection<Subscription>();
            }
            catch
            {
                return new ObservableCollection<Subscription>();
            }
        }

        public static void Save(ObservableCollection<Subscription> subscriptions)
        {
            try
            {
                Directory.CreateDirectory(FolderPath);

                var json = JsonSerializer.Serialize(
                    subscriptions,
                    new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(FilePath, json);
            }
            catch
            {
                // intentionally silent for now
            }
        }
    }
}