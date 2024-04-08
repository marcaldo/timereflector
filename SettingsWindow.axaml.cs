using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using TimeReflector.Data;

namespace TimeReflector
{
    public partial class SettingsWindow : Window
    {
        private TextBox? albumTextBox;
        private ComboBox albumsComboBox;


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

            albumsComboBox = this.FindControl<ComboBox>("AlbumsComboBox");
            albumsComboBox.SelectionChanged += AlbumsComboBox_SelectionChanged;

            configuration = settingsManager.Configuration;

            LoadAlbums();

            LoadSettings();


        }

        private void AlbumsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)e.AddedItems[0]!;
                string selectedValue = (string)selectedItem!.Content!;

                settingsManager.Configuration.SelectedAlbum = selectedValue;
            }
        }
        public void ClickHandler(object sender, RoutedEventArgs args)
        {
            SaveSettings();
        }

        private void LoadAlbums()
        {
            var albumPath = settingsManager.Configuration.AlbumsPath;
            var dirInfo = new DirectoryInfo(albumPath);
            var albumes = dirInfo.GetDirectories();

            albumsComboBox.IsEnabled = albumes.Length > 0;

            albumsComboBox.Items.Clear();
            foreach (var directory in albumes)
            {
                albumsComboBox.Items.Add(new ComboBoxItem { Content = directory.Name });
            }

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
            albumsComboBox.SelectedItem = "Album";// configuration?.SelectedAlbum;
            //albumsComboBox.SelectedIndex = 1;

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
