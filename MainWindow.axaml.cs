using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.IO;

namespace TimeReflector
{
    public partial class MainWindow : Window
    {
        private ImageBrush BackgroundBrush = new();
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
            LoadBackgroundImage("vertical.jpg");
        }

        private void LoadBackgroundImage(string fileName)
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

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