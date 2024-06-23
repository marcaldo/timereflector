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
using TimeReflector.Data;

namespace TimeReflector
{
    public partial class MainWindow : Window
    {
        private readonly DisplayManager _displayManager;

        private TextBlock timeTextBlock = new();
        private TextBlock timeAmPmTextBlock = new();
        private TextBlock dateTextBlock = new();
        private TextBlock tempTextBlock = new();
        private TextBlock? _timeTextBlock;
        private TextBlock? _amPmTextBlock;
        private TextBlock? _dateTextBlock;
        private TextBlock? _infoTextBlock;

        private DispatcherTimer _timerDisplay;
        private DispatcherTimer _timerDateTime;
        private DispatcherTimer _timerWeather;
        private Bitmap _currentBitmap;

        public MainWindow()
        {
            _displayManager = new DisplayManager();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Cursor = TransparentCursor();

            _timeTextBlock = this.FindControl<TextBlock>("TimeTextBlock");
            _amPmTextBlock = this.FindControl<TextBlock>("AmPmTextBlock");
            _dateTextBlock = this.FindControl<TextBlock>("DateTextBlock");
            _infoTextBlock = this.FindControl<TextBlock>("InfoTextBlock");


            RunDisplay();
            SetupTimers();
        }

        private Cursor TransparentCursor()
        {
            if (System.Diagnostics.Debugger.IsAttached) return Cursor.Default;

            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icons", "transparent.png");
            var transparentCursor = new Cursor(new Bitmap(iconPath), new PixelPoint(0, 0));
            return transparentCursor;
        }
        private void SetupTimers()
        {
            _timerDisplay = new DispatcherTimer();
            _timerDisplay.Interval = TimeSpan.FromSeconds(_displayManager.SettingsManager.Configuration.Duration.DisplaySeconds);
            _timerDisplay.Tick += TimerDisplay_Tick!;
            _timerDisplay.Start();

            _timerDateTime = new DispatcherTimer();
            _timerDateTime.Interval = TimeSpan.FromSeconds(1);
            _timerDateTime.Tick += TimerDateTime_Tick!;
            _timerDateTime.Start();

            _timerWeather = new DispatcherTimer();
            _timerWeather.Interval = TimeSpan.FromMinutes(_displayManager.SettingsManager.Configuration.Duration.WheatherMinutes);
            _timerWeather.Tick += TimerDateTime_Tick!;
            _timerWeather.Start();
        }

        private void ResetTimers()
        {
            _timerDisplay.Stop();
            _timerWeather.Stop();

            _timerDisplay.Interval = TimeSpan.FromSeconds(_displayManager.SettingsManager.Configuration.Duration.DisplaySeconds);
            _timerWeather.Interval = TimeSpan.FromMinutes(_displayManager.SettingsManager.Configuration.Duration.WheatherMinutes);

            _timerDisplay.Start();
            _timerWeather.Start();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
        }

        private void TimerDisplay_Tick(object sender, EventArgs e)
        {
            RunDisplay();
        }

        private void TimerDateTime_Tick(object sender, EventArgs e)
        {
            //timeTextBlock.Text = _displayManager.DateTimeDisplayData.Time;
            //timeAmPmTextBlock.Text = _displayManager.DateTimeDisplayData.AmPm;
            //dateTextBlock.Text = _displayManager.DateTimeDisplayData.Date;

            var now = DateTime.Now;
            _timeTextBlock!.Text = now.ToString("hh:mm");
            _amPmTextBlock!.Text = now.ToString("tt");
            _dateTextBlock!.Text = now.ToString("dddd, MMMM dd");
        }

        private void TimerWeather_Tick(object sender, EventArgs e)
        { }

        private void RunDisplay()
        {
            bool useAppDefaultImagePath = false;

            var displayItem = _displayManager.GetNextItem();

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
            //Grid grid = CreateWindowGrid();
            Grid? grid = this.FindControl<Grid>("MainGrid");

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
            //this.Content = grid;

        }

        private Border GetImageContainer(DisplayItem displayItem, bool useAppDefaultImagePath = false)
        {    
            double viewPortHeight = this.ClientSize.Height;
            double viewPortWidth = this.ClientSize.Width;

            _infoTextBlock!.Text = string.Empty;


            if (displayItem?.AlbumPath is null)
            {
                _displayManager.SettingsManager.ResetConfiguration();
                displayItem = _displayManager.GetNextItem()!;
            }

            string imagePath = Path.Combine(displayItem.AlbumPath, displayItem.ImageFileName);
            _currentBitmap?.Dispose();

            try
            {
                _currentBitmap = new Bitmap(imagePath);
            }
            catch (Exception ex)
            {
                _infoTextBlock!.Text =$"{ex.Message} - {imagePath}";
                return new Border();
            }

            Image backgroundImage = new Image
            {
                Source = _currentBitmap,
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
            var imageRatio = _currentBitmap.Size.Width / _currentBitmap.Size.Height;

            if (_currentBitmap.Size.Width > _currentBitmap.Size.Height && imageRatio > minRatio)
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

            var timeText = _displayManager.DateTimeDisplayData.Time;

            var canvas = new Canvas { Margin = new Thickness(45) };

            //var timeTextBlockBckg = new TextBlock
            //{
            //    Text = timeText,
            //    FontSize = _displayManager.DateTimeDisplayData.TimeFontStyle.FontSize,
            //    Foreground = Brush.Parse("#000000"),
            //    Margin = Thickness.Parse("2"),
            //    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            //    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            //};

            //canvas.Children.Add(timeTextBlockBckg);

            //timeTextBlock = new TextBlock
            //{
            //    Text = timeText,
            //    FontSize = _displayManager.DateTimeDisplayData.TimeFontStyle.FontSize,
            //    Foreground = Brush.Parse(_displayManager.DateTimeDisplayData.TimeFontStyle.FontForegroundColor),
            //    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            //    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            //};

            //canvas.Children.Add(timeTextBlock);

            //Canvas.SetLeft(timeTextBlockBckg, 0);
            //Canvas.SetTop(timeTextBlockBckg, 0);
            //Canvas.SetLeft(timeTextBlock, 0);
            //Canvas.SetTop(timeTextBlock, 0);

            //Grid.SetColumn(canvas, 0);
            //Grid.SetRow(canvas, 0);
            //grid.Children.Add(canvas);

            //timeAmPmTextBlock = new TextBlock
            //{
            //    Text = _displayManager.DateTimeDisplayData.AmPm,
            //    FontSize = _displayManager.DateTimeDisplayData.TimeFontStyle.FontSize * 0.4,
            //    Foreground = Brush.Parse(_displayManager.DateTimeDisplayData.TimeFontStyle.FontForegroundColor),
            //    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            //    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            //};
            //Grid.SetColumn(timeAmPmTextBlock, 1);
            //Grid.SetRow(timeAmPmTextBlock, 0);
            //grid.Children.Add(timeAmPmTextBlock);

            //dateTextBlock = new TextBlock
            //{
            //    Text = _displayManager.DateTimeDisplayData.Date,
            //    FontSize = _displayManager.DateTimeDisplayData.DateFontStyle.FontSize,
            //    Foreground = Brush.Parse(_displayManager.DateTimeDisplayData.DateFontStyle.FontForegroundColor),
            //    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            //    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top
            //};
            //Grid.SetColumn(dateTextBlock, 0);
            //Grid.SetRow(dateTextBlock, 1);
            //grid.Children.Add(dateTextBlock);

            return grid;
        }

        private static Grid CreateWindowGrid()
        {
            // Create a new Grid
            Grid grid = new();

            // Set the grid to fill the whole screen
            grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

            //// Define ColumnDefinitions
            //grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            //// Define RowDefinitions
            //grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });
            //grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            //grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });

            return grid;
        }

        private StackPanel ToolsPanel()
        {
            var icon = _displayManager.Icon("gear.png");
            icon.Cursor = new Cursor(StandardCursorType.Hand);

            // Subscribe to Click event
            icon!.PointerPressed += OpenDialog_Click!;

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom
            };
            stackPanel.Children.Add(icon);

            return stackPanel;

        }

        private async void OpenDialog_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            await settingsWindow.ShowDialog(this);

            _displayManager.SettingsManager.Configuration = _displayManager.SettingsManager.ReLoadSettings();

            _displayManager.ClearItemList();
            ResetTimers();
            RunDisplay();
        }

        protected override void OnClosed(EventArgs e)
        {
            _currentBitmap?.Dispose();
            base.OnClosed(e);
        }

    }
}