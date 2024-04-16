using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using TimeReflector.Data;

namespace TimeReflector
{
    public partial class SettingsWindow : Window
    {
        private TextBox? albumTextBox;
        private ComboBox albumsComboBox;
        private ComboBox displayTimeComboBox;

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
            albumTextBox!.TextChanged += AlbumTextBox_TextChanged;

            albumsComboBox = this.FindControl<ComboBox>("AlbumsComboBox")!;
            albumsComboBox!.SelectionChanged += AlbumsComboBox_SelectionChanged!;

            displayTimeComboBox = this.FindControl<ComboBox>("DisplayTimeComboBox")!;
            displayTimeComboBox!.SelectionChanged += DisplayTimeComboBox_SelectionChanged;


            configuration = settingsManager.Configuration;

            LoadAlbumsList();
            PopulateDisplayTimeComboBox();

            LoadSettings();

        }

        private void AlbumTextBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            LoadAlbumsList();
        }

        private void DisplayTimeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)e.AddedItems[0]!;
            int selectedValue = (int)selectedItem!.DataContext!;

            configuration.Duration.DisplaySeconds = selectedValue;
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

        private void LoadAlbumsList()
        {
            var albumsPath = albumTextBox?.Text;
            if (string.IsNullOrWhiteSpace(albumsPath)) return;

            var selectedAlbum = configuration.SelectedAlbum;
            var dirInfo = new DirectoryInfo(albumsPath);

            if (!dirInfo.Exists) return;

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

        private void PopulateDisplayTimeComboBox()
        {
            displayTimeComboBox.Items.Clear();

            int currentInterval = configuration.Duration.DisplaySeconds;


            displayTimeComboBox.SelectedIndex = 1;

            List<ComboBoxItem> displayTimeComboBoxItems = new List<ComboBoxItem>()
            {
                    new() { DataContext = 5, Content = "5 seconds" },
                    new() { DataContext = 10, Content = "10 seconds" },
                    new() { DataContext = 15, Content = "15 seconds" },
                    new() { DataContext = 20, Content = "20 seconds" },
                    new() { DataContext = 30, Content = "30 seconds" },
                    new() { DataContext = 45, Content = "45 seconds" },
                    new() { DataContext = 60, Content = "1 minute" },
                    new() { DataContext = 90, Content = "1.5 minutes" },
                    new() { DataContext = 120, Content = "2 minutes" },
                    new() { DataContext = 180, Content = "3 minutes" },
                    new() { DataContext = 240, Content = "4 minutes" },
                    new() { DataContext = 300, Content = "5 minutes" },
                    new() { DataContext = 420, Content = "7 minutes" },
                    new() { DataContext = 600, Content = "10 minutes" },
                    new() { DataContext = 900, Content = "15 minutes" },
                    new() { DataContext = 1200, Content = "20 minutes" },
                    new() { DataContext = 1800, Content = "30 minutes" },
                    new() { DataContext = 2700, Content = "45 minutes" },
                    new() { DataContext = 3600, Content = "1 hour" },
                    new() { DataContext = 7200, Content = "2 hours" },
                    new() { DataContext = 10800, Content = "3 hours" },
                    new() { DataContext = 21600, Content = "6 hours" },
                    new() { DataContext = 43200, Content = "12 hours" },
                    new() { DataContext = 86400, Content = "24 hours" }
            };


            int selectedIndex = 0;

            for (int i = 0; i < displayTimeComboBoxItems.Count; i++)
            {
                var displayTimeItem = displayTimeComboBoxItems[i];
                displayTimeComboBox.Items.Add(displayTimeItem);

                if (((int)displayTimeItem.DataContext!) == configuration.Duration.DisplaySeconds)
                    selectedIndex = i;
            }

            displayTimeComboBox.SelectedIndex = selectedIndex;
        }

        private void SaveSettings()
        {
            settingsManager.Configuration.AlbumsPath = albumTextBox?.Text ?? "";
            settingsManager.Configuration.SelectedAlbum = albumsComboBox.SelectionBoxItem?.ToString() ?? "";
            settingsManager.Configuration.Weather = new() { TemperatureFormat = TemperatureFormatType.None };

            settingsManager.SaveSettings();

            this.Close();
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

        //public sealed class ComboBoxItem : ListBoxItem
        //{
        //    public int Value { get; set; }
        //    public string DisplayText { get; set; } = "";
        //}

    }
}
