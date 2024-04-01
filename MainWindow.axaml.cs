using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ExifLib;
using SkiaSharp;
using System;
using System.IO;
using TimeReflector.Data;

namespace TimeReflector
{
    public partial class MainWindow : Window
    {
        private ImageBrush backgroundBrush = new();
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

            using (ExifReader reader = new ExifReader(imagePath))
            {
                reader.GetTagValue(ExifTags.Orientation, out object orientation);
   
            }


            backgroundBrush = new ImageBrush(backgroundImage);

            var transform = new RotateTransform();
            transform.Angle = 45;
            backgroundBrush.Transform =transform;

            Resources["BackgroundBrush"] = backgroundBrush;
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