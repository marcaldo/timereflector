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
            albumTextBox.TextChanged += AlbumTextBox_TextChanged;

            albumsComboBox = this.FindControl<ComboBox>("AlbumsComboBox");
            albumsComboBox.SelectionChanged += AlbumsComboBox_SelectionChanged;

            configuration = settingsManager.Configuration;

            LoadAlbums();

            LoadSettings();

        }

        private void AlbumTextBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            LoadAlbums();
        }


        private void AlbumsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)e.AddedItems[0]!;
                string selectedValue = (string)selectedItem!.Content!;

                configuration.SelectedAlbum = selectedValue;
            }
        }
        public void ClickHandler(object sender, RoutedEventArgs args)
        {
            SaveSettings();
        }

        private void LoadAlbums()
        {
            var albumsPath = albumTextBox?.Text;
            if (string.IsNullOrWhiteSpace(albumsPath)) return;

            var selectedAlbum = configuration.SelectedAlbum;
            var dirInfo = new DirectoryInfo(albumsPath);

            if(!dirInfo.Exists) return; 

            var albums = dirInfo.GetDirectories();
            var selectedIndex = 0;

            albumsComboBox.Items.Clear();

            albumsComboBox.IsEnabled = albums.Length > 0;

            for (int i = 0; i < albums.Length; i++)
            {
                bool isSelected = false;
                var directory = albums[i];

                if (directory.Name.Equals(selectedAlbum, StringComparison.OrdinalIgnoreCase))
                {
                    isSelected = true;
                    selectedIndex = i;
                }

                albumsComboBox.Items.Add(new ComboBoxItem { Content = directory.Name, IsSelected = isSelected });

            }

            albumsComboBox.SelectedIndex = selectedIndex;
        }

        private void SaveSettings()
        {
            configuration.AlbumsPath = albumTextBox?.Text ?? "";
            configuration.Weather = new() { TemperatureFormat = TemperatureFormatType.C };

            settingsManager.SaveSettings();

            this.Close();
        }

        private void LoadSettings()
        {
            albumTextBox!.Text = configuration?.AlbumsPath;
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
