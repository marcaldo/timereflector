using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.IO;
using Tmds.DBus.Protocol;

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

            var grid = new Grid();
            grid.Children.Add(image);

            var label = new TextBlock
            {
                Text = "Clickable Label",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = 20,
                Foreground = Avalonia.Media.Brushes.White,
                Background = Avalonia.Media.Brushes.Black,
                Opacity = 0.5,
            };

            label.PointerPressed += (sender, e) =>
            {
                Console.WriteLine("Label clicked!");
                // Do different actions here
            };

            grid.Children.Add(label);

            Grid.SetColumn(label, 2); // Adjust column index as needed
            Grid.SetRow(label, 2); // Adjust row index as needed

            Content = grid;
        }

   
    }
}