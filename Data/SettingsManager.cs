using System;
using System.IO;
using System.Text.Json;

namespace TimeReflector.Data
{
    public class SettingsManager
    {
        public Settings Configuration { get; set; }
        readonly string ConfigFile = default!;
        public static string DefaultAlbumsPath { get; private set; }

        public SettingsManager()
        {
            ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            DefaultAlbumsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images_default");

            Configuration = LoadSettings();
        }
        public void SaveSettings()
        {
            string jsonSettings = JsonSerializer.Serialize(Configuration);
            File.WriteAllText(ConfigFile, jsonSettings);
        }

        public Settings ReLoadSettings()
        {
            Configuration = LoadSettings();
            return Configuration;
        }

        public Settings ResetConfiguration()
        {
            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

            File.Delete(configFile);

            return ReLoadSettings();
        }

        private Settings LoadSettings()
        {
            try
            {
                return LoadFromFile();
            }
            catch (Exception)
            {
                EnsureConfigFileExists();
                return LoadFromFile();
            }

            Settings LoadFromFile()
            {
                string jsonString = File.ReadAllText(ConfigFile);
                var settings = JsonSerializer.Deserialize<Settings>(jsonString);

                return settings!;
            }
        }

        private void EnsureConfigFileExists()
        {
            if (!File.Exists(ConfigFile))
            {
                Configuration = new();
                Configuration.AlbumsPath = DefaultAlbumsPath;
                SaveSettings();
            }

        }
    }
}
