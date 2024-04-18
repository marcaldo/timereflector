using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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
        private DisplayManager displayManager = new();
        private SettingsManager settingsManager = new();

        private TextBlock timeTextBlock = new();
        private TextBlock timeAmPmTextBlock = new();
        private TextBlock dateTextBlock = new();
        private TextBlock tempTextBlock = new();

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

            Cursor = TransparentCursor();

            RunDisplay();
            SetupTimers();
        }

        private Cursor TransparentCursor()
        {
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icons", "transparent.png");
            var transparentCursor = new Cursor(new Bitmap(iconPath), new PixelPoint(0, 0));
            return transparentCursor;
        }
        private void SetupTimers()
        {
            timerDisplay = new Timer();
            timerDisplay.Interval = settingsManager.Configuration.Duration.DisplaySeconds.ToMilisecondsSeconds(TimeConversionType.FromSeconds);
            timerDisplay.AutoReset = true;
            timerDisplay.Elapsed += TimerElapsed!;

            timerTemp = new Timer();
            timerTemp.Interval = settingsManager.Configuration.Duration.WheatherMinutes.ToMilisecondsSeconds(TimeConversionType.FromSeconds);
            timerTemp.AutoReset = true;
            timerTemp.Elapsed += TimerTempElapsed!;

            timerDateTime = new Timer();
            timerDateTime.Interval = 1000;
            timerDateTime.AutoReset = true;
            timerDateTime.Elapsed += TimerDateTimeElapsed!;
        }

        private void ResetTimers()
        {
            timerDisplay.Stop();
            timerTemp.Stop();

            timerDisplay.Interval = settingsManager.Configuration.Duration.DisplaySeconds.ToMilisecondsSeconds(TimeConversionType.FromSeconds);
            timerTemp.Interval = settingsManager.Configuration.Duration.WheatherMinutes.ToMilisecondsSeconds(TimeConversionType.FromMinutes);

            timerDisplay.Start();
            timerTemp.Start();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            timerDisplay.Start();
            timerTemp.Start();
            timerDateTime.Start();

            AttachEvents();
        }

        private void AttachEvents()
        {
            //timeTextBlock.PointerPressed += DateTimeTextBox_Click;
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
                timeTextBlock.Text = displayManager.DateTimeDisplayData.Time;
                timeAmPmTextBlock.Text = displayManager.DateTimeDisplayData.AmPm;
                dateTextBlock.Text = displayManager.DateTimeDisplayData.Date;
            });
        }

        void TimerDateTimeElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                timeTextBlock.Text = displayManager.DateTimeDisplayData.Time;
            });
        }

        private void RunDisplay()
        {
            bool useAppDefaultImagePath = false;

            var displayItem = displayManager.GetNextItem();

            if (displayItem is null) // No items in the album.
            {
                var picNumber = "1";// new Random().Next(1,5);
                displayItem = new()
                {
                    ImageFileName = $"timereflection{picNumber}.jpeg",
                    IsVideo = false
                };

                useAppDefaultImagePath = true;
            }

            Display(displayItem!, useAppDefaultImagePath);
        }

        private void Display(DisplayItem displayItem, bool useAppDefaultImagePath)
        {
            Grid grid = CreateWindowGrid();

            Border imageContainer = GetImageContainer(displayItem, useAppDefaultImagePath);

            // Set the Border as the background of the grid
            grid.Background = new VisualBrush { Visual = imageContainer };

            Grid dateTimeGrid = DateTimeGrid();
            Grid.SetColumn(dateTimeGrid, 0);
            Grid.SetRow(dateTimeGrid, 2);
            grid.Children.Add(dateTimeGrid);

            tempTextBlock = new TextBlock();
            tempTextBlock.Text = "";
            tempTextBlock.FontSize = 100;
            tempTextBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
            tempTextBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Grid.SetColumn(tempTextBlock, 2);
            Grid.SetRow(tempTextBlock, 2);
            grid.Children.Add(tempTextBlock);

            var toolsPanel = ToolsPanel();
            Grid.SetColumn(toolsPanel, 2);
            Grid.SetRow(toolsPanel, 2);
            grid.Children.Add(toolsPanel);

            grid.Cursor = TransparentCursor();

            // Add the grid to the window
            this.Content = grid;
        }


        private Border GetImageContainer(DisplayItem displayItem, bool useAppDefaultImagePath = false)
        {
            double viewPortHeight = this.ClientSize.Height;
            double viewPortWidth = this.ClientSize.Width;

            string albumPath = useAppDefaultImagePath
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images_default")
                : Path.Combine(settingsManager.Configuration.AlbumsPath, settingsManager.Configuration.SelectedAlbum);

            string imagePath = Path.Combine(albumPath, displayItem.ImageFileName);

            Bitmap image = new(imagePath);

            Image backgroundImage = new Image
            {
                Source = image,
                Stretch = Stretch.UniformToFill
            };

            if (displayItem.Rotate > 0)
            {
                RotateTransform rotateTransform = new(displayItem.Rotate);
                backgroundImage.RenderTransform = rotateTransform;
            }

            Border imageContainer = new()
            {
                Child = backgroundImage
            };

            var minRatio = 1.5;
            var imageRatio = image.Size.Width / image.Size.Height;

            if (image.Size.Width > image.Size.Height && imageRatio > minRatio)
            {
                imageContainer.Height = viewPortHeight;
                imageContainer.Width = viewPortWidth;
            }

            return imageContainer;
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

            var timeText = displayManager.DateTimeDisplayData.Time;

            var canvas = new Canvas { Margin = new Thickness(45) };

            var timeTextBlockBckg = new TextBlock
            {
                Text = timeText,
                FontSize = displayManager.DateTimeDisplayData.TimeFontStyle.FontSize,
                Foreground = Brush.Parse("#000000"),
                Margin = Thickness.Parse("2"),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            canvas.Children.Add(timeTextBlockBckg);

            timeTextBlock = new TextBlock
            {
                Text = timeText,
                FontSize = displayManager.DateTimeDisplayData.TimeFontStyle.FontSize,
                Foreground = Brush.Parse(displayManager.DateTimeDisplayData.TimeFontStyle.FontForegroundColor),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            canvas.Children.Add(timeTextBlock);

            Canvas.SetLeft(timeTextBlockBckg, 0);
            Canvas.SetTop(timeTextBlockBckg, 0);
            Canvas.SetLeft(timeTextBlock, 0);
            Canvas.SetTop(timeTextBlock, 0);

            Grid.SetColumn(canvas, 0);
            Grid.SetRow(canvas, 0);
            grid.Children.Add(canvas);

            timeAmPmTextBlock = new TextBlock
            {
                Text = displayManager.DateTimeDisplayData.AmPm,
                FontSize = displayManager.DateTimeDisplayData.TimeFontStyle.FontSize * 0.4,
                Foreground = Brush.Parse(displayManager.DateTimeDisplayData.TimeFontStyle.FontForegroundColor),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            Grid.SetColumn(timeAmPmTextBlock, 1);
            Grid.SetRow(timeAmPmTextBlock, 0);
            grid.Children.Add(timeAmPmTextBlock);

            dateTextBlock = new TextBlock
            {
                Text = displayManager.DateTimeDisplayData.Date,
                FontSize = displayManager.DateTimeDisplayData.DateFontStyle.FontSize,
                Foreground = Brush.Parse(displayManager.DateTimeDisplayData.DateFontStyle.FontForegroundColor),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top
            };
            Grid.SetColumn(dateTextBlock, 0);
            Grid.SetRow(dateTextBlock, 1);
            grid.Children.Add(dateTextBlock);

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

        private StackPanel ToolsPanel()
        {
            var icon = displayManager.Icon("gear.png");
            icon.Cursor = new Cursor(StandardCursorType.Hand);

            // Subscribe to Click event
            icon!.PointerPressed += OpenDialog_Click!;

            // Add to StackPanel or any other container in your UI
            var stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
            stackPanel.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom;
            stackPanel.Children.Add(icon);

            return stackPanel;

        }

        private async void OpenDialog_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            await settingsWindow.ShowDialog(this);

            settingsManager.Configuration = settingsManager.ReLoadSettings();

            displayManager.ClearItemList();
            ResetTimers();
            RunDisplay();
        }

    }
}