using System;
using System.IO;
using System.Text.Json;

namespace TimeReflector.Data
{
    public class SettingsManager
    {
        public Settings Configuration { get; set; }
        readonly string ConfigFile = default!;
        public static string DefaultAlbumsPath { get; private set; } = default!;

        private const string  _configFileName = "config.json";

        public SettingsManager()
        {
            ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configFileName);
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
            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configFileName);

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
            if (!File.Exists(ConfigFile) || Configuration is null)
            {
                Configuration = new();
                Configuration.AlbumsPath = DefaultAlbumsPath;
                SaveSettings();
            }

        }
    }
}
