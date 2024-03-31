using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.IO;

namespace TimeReflector
{
    public partial class MainWindow : Window
    {
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
            DisplayImageFromFile("superior.jpg");
        }

        private void DisplayImageFromFile(string fileName)
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
            // Create an Image control
            var image = new Image
            {
                // Set the source of the image to the file path
                Source = new Avalonia.Media.Imaging.Bitmap(imagePath)
            };

            // Add the image to the window
            Content = image;
        }
    }
}