using System;
using System.IO;
using System.Text.Json;

namespace TimeReflector.Data
{
    public class SettingsManager
    {
        public Settings Configuration { get; set; }
        readonly string ConfigFile = default!;

        public SettingsManager()
        {
            ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "config.json");
            Configuration = LoadSettings();
        }
        public void SaveSettings()
        {
            string jsonSettings = JsonSerializer.Serialize(Configuration);
            File.WriteAllText(ConfigFile, jsonSettings);
        }

        private Settings LoadSettings()
        {
            string jsonString = File.ReadAllText(ConfigFile);
            var settings = JsonSerializer.Deserialize<Settings>(jsonString);

            return settings ?? new Settings();
        }
    }
}
