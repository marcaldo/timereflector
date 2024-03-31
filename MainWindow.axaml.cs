using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.IO;
using TimeReflector.Data;

namespace TimeReflector
{
    public partial class MainWindow : Window
    {
        private ImageBrush BackgroundBrush = new();
        private DisplayManager displayManager = new ();  
        private SettingsManager settingsManager = new ();   
        public MainWindow()
        {
            InitializeComponent();
            DisplayImages();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void DisplayImages()
        {
            var displayList = displayManager.GetDisplayItems();


            LoadBackgroundImage(displayList[0].ImageFileName);
        }

        private void LoadBackgroundImage(string fileName)
        {
            string albumPath = settingsManager.Configuration.AlbumsPath;
            string imagePath = Path.Combine(albumPath, fileName);

            var backgroundImage = new Bitmap(imagePath);
            BackgroundBrush = new ImageBrush(backgroundImage);
            Resources["BackgroundBrush"] = BackgroundBrush;
        }

        private void Label_OnPointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            OpenSettings();
        }

        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }
    }
}