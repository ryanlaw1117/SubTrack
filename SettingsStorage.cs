using System;
using System.IO;
using System.Text.Json;
using Subscription_Manager.Models;

namespace Subscription_Manager
{
    public static class SettingsStorage
    {
        private static readonly string AppFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SubTrack");

        private static readonly string SettingsFilePath =
            Path.Combine(AppFolder, "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                    return new AppSettings();

                var json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                // If JSON is corrupted, don't crash startup
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                Directory.CreateDirectory(AppFolder);
                File.WriteAllText(SettingsFilePath,
                    JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch
            {
                // Don't crash on write failures
            }
        }
    }
}
