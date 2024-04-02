using Avalonia;
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
        private DisplayManager displayManager = new();
        private SettingsManager settingsManager = new();
        public MainWindow()
        {
            InitializeComponent();
            //this.WindowState = WindowState.Maximized;
            DisplayImages();
        }

        private void InitializeComponent()
        {
            // Load the XAML file (MainWindow.axaml)
            AvaloniaXamlLoader.Load(this);

            //// Set the window properties
            //this.Width = 800;
            //this.Height = 600;

            // Create a Grid
            Grid grid = new Grid();

            // Load the background image
            Bitmap backgroundImage = new Bitmap("C:/Users/marca/source/repos/TimeReflect/Albums/Album2/JenMar.jpg");

            // Set the background image
            double height = this.ClientSize.Height;
            
            Image background = new Image
            {
                Source = backgroundImage,
                Stretch = Stretch.Uniform,
                Height = height
            };

            RotateTransform rotateTransform = new RotateTransform(270);
            background.RenderTransform = rotateTransform;

            // Add the background image to the Grid
            grid.Children.Add(background);

            // Create a TextBlock
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Hello, Avalonia!";
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            textBlock.FontSize = 24;
            textBlock.Foreground = Brushes.White;

            // Add the TextBlock to the Grid
            grid.Children.Add(textBlock);

            // Set the content of the window to the Grid
            this.Content = grid;
        }

        private void MainWindow_SizeChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            // Access the Bounds property of the window
            var bounds = this.Bounds;
            // Get the height from the bounds
            double height = bounds.Height;
            // Use the height as needed
            // For example, you can update UI elements based on window height
            // Console.WriteLine($"Window Height: {height}");
        }
        private void DisplayImages()
        {
            var displayList = displayManager.GetDisplayItems();


            LoadBackgroundImage(displayList[3]);
        }

        private void LoadBackgroundImage(DisplayItem displayItem)
        {
            string albumPath = settingsManager.Configuration.AlbumsPath;
            string imagePath = Path.Combine(albumPath, displayItem.ImageFileName);

            //var backgroundImage = new Bitmap(imagePath,);

            using Stream stream = File.OpenRead(imagePath);
            Bitmap backgroundImage = Bitmap.DecodeToWidth(stream, 500, BitmapInterpolationMode.HighQuality);

            //Stream ms = default!;
            //using (FileStream file = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            //    file.CopyTo(ms);

            //var backgroundImage = Bitmap.DecodeToWidth(ms, 80);

            backgroundBrush = new ImageBrush(backgroundImage);

            if (displayItem.Rotate > 0)
            {
                backgroundBrush.AlignmentX = AlignmentX.Center;
                backgroundBrush.AlignmentY = AlignmentY.Center;

                var transform = new RotateTransform();
                transform.Angle = displayItem.Rotate;
                transform.CenterX = backgroundImage.Size.Width / 2;
                transform.CenterY = backgroundImage.Size.Height / 2;

                backgroundBrush.Transform = transform;
            }

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