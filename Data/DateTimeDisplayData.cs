using Avalonia.Media;

namespace TimeReflector.Data
{
    public class DateTimeDisplayData : DateTimeFormat
    {
        public string Time { get; set; } = default!;
        public string AmPm { get; set; } = default!;
        public string Date { get; set; } = default!;
    }
}
