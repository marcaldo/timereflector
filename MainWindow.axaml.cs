using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using System;
using System.IO;
using Tmds.DBus.Protocol;

namespace TimeReflector
{
    public partial class MainWindow : Window
    {
        //private Image? _backgroundImage;
        //private TextBlock? _clickableLabel!;
        public Image BkgImage = new();
        private ImageBrush _backgroundBrush;
        public MainWindow()
        {
            InitializeComponent();

            DisplayImages();

            LoadBackgroundImage();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            //_backgroundImage = this.FindControl<Image>("backgroundImage");
            //_clickableLabel = this.FindControl<TextBlock>("clickableLabel");
        }

        private void LoadBackgroundImage()
        {
            var backgroundImage = new Bitmap("C:/Users/marca/source/repos/timereflector/Images/superior.jpg");
            _backgroundBrush = new ImageBrush(backgroundImage);
            Resources["BackgroundBrush"] = _backgroundBrush;
        }

        private void DisplayImages()
        {
            DisplayImageFromFile("superior.jpg");
        }

        private void DisplayImageFromFile(string fileName)
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
            // Create an Image control
            BkgImage = new Image
            {
                Source = new Avalonia.Media.Imaging.Bitmap(imagePath)
            };
        }

        private void Label_OnPointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            Console.WriteLine("Label clicked!");
            // Do different actions here
        }
    }
}