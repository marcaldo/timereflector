using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.IO;
using System.Timers;
using TimeReflector.Data;

namespace TimeReflector
{
    public partial class MainWindow : Window
    {
        private readonly DisplayManager displayManager = new();
        private readonly SettingsManager settingsManager = new();

        private TextBlock dateTimeTextBlock;
        private TextBlock tempTextBlock;

        private Timer timerDisplay;
        private Timer timerTemp;
        private Timer timerDateTime;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            tempTextBlock = new TextBlock() { Name = "TempTextBlock" };
            dateTimeTextBlock = new TextBlock() { Name = "DateTimeTextBlock" };


            RunDisplay();
            SetupTimer();
        }

        private void AttachEvents()
        {
            dateTimeTextBlock.PointerPressed += DateTimeTextBox_Click;
        }

        private void SetupTimer()
        {
            timerDisplay = new Timer();
            timerDisplay.Interval = 5000;
            timerDisplay.AutoReset = true;
            timerDisplay.Elapsed += TimerElapsed;

            timerTemp = new Timer();
            timerTemp.Interval = 10000;
            timerTemp.AutoReset = true;
            timerTemp.Elapsed += TimerTempElapsed;

            timerDateTime = new Timer();
            timerDateTime.Interval = 1000;
            timerDateTime.AutoReset = true;
            timerDateTime.Elapsed += TimerDateTimeElapsed;
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            timerDisplay.Start();
            timerTemp.Start();
            timerDateTime.Start();

            AttachEvents();

        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                RunDisplay();
            });
        }

        void TimerTempElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                dateTimeTextBlock.Text = DateTime.Now.ToString("t");
            });
        }

        void TimerDateTimeElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                dateTimeTextBlock.Text = displayManager.DateTimeDisplayData.Time;
            });
        }

        private void RunDisplay()
        {
            var displayItem = displayManager.GetNextItem();

            if (displayItem is null) // No items in the album.
            {
                timerDisplay.Stop();
            }

            Display(displayItem!);
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

            Grid dateTimeGrid = DateTimeGrid();
            Grid.SetColumn(dateTimeGrid, 0);
            Grid.SetRow(dateTimeGrid, 2);
            grid.Children.Add(dateTimeGrid);


            tempTextBlock = new TextBlock();
            tempTextBlock.Text = "41°F 5°C  ";
            tempTextBlock.FontSize = 100;
            tempTextBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
            tempTextBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(tempTextBlock, 2);
            Grid.SetRow(tempTextBlock, 2);
            grid.Children.Add(tempTextBlock);

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
                Stretch = Stretch.UniformToFill,
                Height = height
            };

            if (displayItem.Rotate > 0)
            {
                RotateTransform rotateTransform = new RotateTransform(displayItem.Rotate);
                backgroundImage.RenderTransform = rotateTransform;
            }


            return backgroundImage;
        }

        private Grid DateTimeGrid()
        {
            Grid grid = new();

            grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

            // Define ColumnDefinitions
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });

            // Define RowDefinitions
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });

            dateTimeTextBlock = new TextBlock();
            dateTimeTextBlock.Text = displayManager.DateTimeDisplayData.Time;
            dateTimeTextBlock.FontSize = 100;
            dateTimeTextBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            dateTimeTextBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(dateTimeTextBlock, 0);
            Grid.SetRow(dateTimeTextBlock, 2);
            grid.Children.Add(dateTimeTextBlock);

            return grid;
        }

        private static Grid CreateWindowGrid()
        {
            // Create a new Grid
            Grid grid = new();

            // Set the grid to fill the whole screen
            grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

            // Define ColumnDefinitions
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            // Define RowDefinitions
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });

            return grid;
        }

        private void DateTimeTextBox_Click(object? sender, Avalonia.Input.PointerPressedEventArgs e)
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