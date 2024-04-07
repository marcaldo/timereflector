using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
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
            configuration.Weather = new() { TemperatureFormat = TemperatureFormatType.C };

            settingsManager.SaveSettings();
        }

        private void LoadSettings()
        {
            albumTextBox!.Text = configuration?.AlbumsPath;

        }

        public string Result { get; private set; }
        public event EventHandler<string> DialogClosed;
        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            // Set the result before closing the dialog
            Result = "Value from dialog";
            DialogClosed?.Invoke(this, Result);
            Close();
        }


    }
}
