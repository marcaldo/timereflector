using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TimeReflector.Data;

namespace TimeReflector
{
    public partial class SettingsWindow : Window
    {
        private TextBox? albumTextBox;
        SettingsManager settingsManager = new();
        Settings configuration = new();

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            albumTextBox = this.FindControl<TextBox>("AlbumTextBox");

            configuration = settingsManager.Configuration; 

            LoadSettings();
        }


        public void ClickHandler(object sender, RoutedEventArgs args)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            configuration.AlbumsPath = albumTextBox?.Text ?? "";
            configuration.Temperature = new() { Display = TemperatureDisplayTypes.Celsius };

            settingsManager.SaveSettings();
        }

        private void LoadSettings()
        {
            albumTextBox!.Text = configuration?.AlbumsPath;

        }
    }
}
