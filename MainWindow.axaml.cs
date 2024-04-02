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
        private DisplayManager displayManager = new();
        private SettingsManager settingsManager = new();
        private double windowHeight = 0;

        public MainWindow()
        {
            InitializeComponent();
            //this.WindowState = WindowState.Maximized;


        }

        private void InitializeComponent()
        {
            // Load the XAML file (MainWindow.axaml)
            AvaloniaXamlLoader.Load(this);

            //this.Activated += (sender, e) =>
            //{
            //    this.windowHeight = this.Height;

            //    // Do something with windowHeight
            //};

            ////// Set the window properties
            ////this.Width = 800;
            ////this.Height = 600;

            //// Create a Grid
            //Grid grid = new Grid();
            //grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            //grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            //grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            //grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            //grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            //grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));




            //// Load the background image
            //Bitmap backgroundImage = new Bitmap("C:/Users/marca/OneDrive/Dev/TimeReflector/Album/ice.jpg");

            //// Set the background image
            ////double height = this.ClientSize.Height;

            //Image background = new Image
            //{
            //    Source = backgroundImage,
            //    Stretch = Stretch.UniformToFill,
            //    //Height = height
            //};

            //RotateTransform rotateTransform = new RotateTransform(0);
            //background.RenderTransform = rotateTransform;

            //// Add the background image to the Grid
            //grid.Children.Add(background);

            //// Create a TextBlock
            ////TextBlock textBlock = new TextBlock();
            ////textBlock.Text = "Hello, Avalonia!";
            ////textBlock.TextAlignment = TextAlignment.Center;
            ////textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            ////textBlock.FontSize = 24;
            ////textBlock.Foreground = Brushes.White;

            //// Add the TextBlock to the Grid
            ////grid.Children.Add(textBlock);

            //// Set the content of the window to the Grid
            //this.Content = grid;

            var displayList = displayManager.GetDisplayItems();


            Display(displayList[0]);

        }

        private void Display(DisplayItem displayItem)
        {
            Grid grid = CreateWindowGrid();

            Image image = GetImage(displayItem);

            // Set the Image control as the content of a Border
            Border imageContainer = new()
            {
                Child = image
            };

            // Set the Border as the background of the grid
            grid.Background = new VisualBrush { Visual = imageContainer };

            // Add label
            Label weatherText = WeatherText();
            grid.Children.Add(weatherText);

            // Add the grid to the window
            this.Content = grid;
        }


        private Image GetImage(DisplayItem displayItem)
        {
            var height = this.ClientSize.Height;



            string albumPath = settingsManager.Configuration.AlbumsPath;
            string imagePath = Path.Combine(albumPath, displayItem.ImageFileName);

            // Load the image
            Bitmap image = new(imagePath);

            Image backgroundImage = new Image
            {
                Source = image,
                //Stretch = Stretch.UniformToFill,
                Height = height// this.windowHeight,
            };

            if (displayItem.Rotate > 0)
            {
                RotateTransform rotateTransform = new RotateTransform(displayItem.Rotate);
                backgroundImage.RenderTransform = rotateTransform;
            }


            return backgroundImage;
        }

        private static Label WeatherText()
        {
            // Add the label to the third column and third row
            Label weatherText = new()
            {
                Content = "41°F 5°C  ",
                FontSize = 50,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top
            };

            Grid.SetColumn(weatherText, 2);
            Grid.SetRow(weatherText, 2);
            return weatherText;
        }

        private static Grid CreateWindowGrid()
        {
            // Create a new Grid
            Grid grid = new();

            // Set the grid to fill the whole screen
            grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

            // Define ColumnDefinitions
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });

            // Define RowDefinitions
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });

            return grid;
        }



        //private void RefreshScreen(DisplayItem displayItem)
        //{
        //    string albumPath = settingsManager.Configuration.AlbumsPath;
        //    string imagePath = Path.Combine(albumPath, displayItem.ImageFileName);

        //    Bitmap backgroundImage = new(imagePath);


        //    //backgroundBrush = new ImageBrush(backgroundImage);

        //    //if (displayItem.Rotate > 0)
        //    //{
        //    //    backgroundBrush.AlignmentX = AlignmentX.Center;
        //    //    backgroundBrush.AlignmentY = AlignmentY.Center;

        //    //    var transform = new RotateTransform();
        //    //    transform.Angle = displayItem.Rotate;
        //    //    transform.CenterX = backgroundImage.Size.Width / 2;
        //    //    transform.CenterY = backgroundImage.Size.Height / 2;

        //    //    backgroundBrush.Transform = transform;
        //    //}

        //    //Resources["BackgroundBrush"] = backgroundBrush;
        //}



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