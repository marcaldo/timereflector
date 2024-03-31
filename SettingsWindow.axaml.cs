using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.IO;
using System.Text.Json;
using TimeReflector.Data;

namespace TimeReflector
{
    public partial class SettingsWindow : Window
    {
        private TextBox? albumTextBox;
        string ConfigFile = default!;
        public SettingsWindow()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            albumTextBox = this.FindControl<TextBox>("AlbumTextBox");

            ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "config.json");

            LoadSettings();

        }


        public void ClickHandler(object sender, RoutedEventArgs args)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            Settings settings = new()
            {
                AlbumsPath = albumTextBox?.Text ?? "",
                Temperature = new() { Display = TemperatureDisplayTypes.Celsius }

            };

            string jsonSettings = JsonSerializer.Serialize(settings);

            File.WriteAllText(ConfigFile, jsonSettings);

        }

        private void LoadSettings()
        {
            string jsonString = File.ReadAllText(ConfigFile);

            var settings = JsonSerializer.Deserialize<Settings>(jsonString);

            albumTextBox!.Text = settings?.AlbumsPath;

        }
    }
}
