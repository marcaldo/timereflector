using Avalonia.Media;

namespace TimeReflector.Data
{
    public class DateTimeDisplayData
    {
        public string Time { get; set; } = default!;
        public string AmPm { get; set; } = default!;
        public string Date { get; set; } = default!;
        public int TimeFontSize { get; set; } = 100;
        public int DateFontSize { get; set; } = 100;
        public IImmutableBrush TimeForegroundColor { get; set; } = Brushes.White;
        public IImmutableBrush DateForegroundColor { get; set; } = Brushes.Yellow;
    }
}
