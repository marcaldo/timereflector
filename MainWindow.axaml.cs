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
        private ImageBrush _backgroundBrush = new();
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
            LoadBackgroundImage("superior.jpg");
        }

        private void LoadBackgroundImage(string fileName)
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

            var backgroundImage = new Bitmap(imagePath);
            _backgroundBrush = new ImageBrush(backgroundImage);
            Resources["BackgroundBrush"] = _backgroundBrush;
        }

        private void Label_OnPointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            //Console.WriteLine("Label clicked!");
            // Do different actions here
        }
    }
}