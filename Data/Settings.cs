using Avalonia.Media;

namespace TimeReflector.Data
{
    public sealed class Settings
    {
        public string AlbumsPath { get; set; } = default!;
        public string Album { get; set; } = default!;
        public UnitSystemType UnitSystem { get; set; } = UnitSystemType.Metric;
        public DateTimeFormat DateTimeFormat { get; set; } = new();
        public Weather Weather { get; set; } = new();
        public Duration Duration { get; set; } = new();
    }

    public sealed class Weather
    {
        public TemperatureFormatType TemperatureFormat { get; set; } = TemperatureFormatType.CF;
        public string City { get; set; } = default!;
        public bool UseCity { get; set; } = false;
    }

    public sealed class Duration()
    {
        /// <summary>
        /// Seconds a display item is displayed.
        /// </summary>
        public int DisplaySeconds { get; set; } = 5;

        /// <summary>
        /// Minutes between weather API calls to retireve the weather.
        /// </summary>
        public int WheatherMinutes { get; set; } = 30;
    }


    public class DateTimeFormat
    {
        public TimeFormatType TimeFormat { get; set; } = TimeFormatType.T12hs;
        public DateFormatType DateFormat { get; set; } = DateFormatType.Date1_xWD_M_D;
        public FontStyle TimeFontStyle { get; set; } = new FontStyle { FontSize = 100 };
        public FontStyle DateFontStyle { get; set; } = new FontStyle { FontSize = 45 };

    }

    public class FontStyle
    {
        public int FontSize { get; set; } = 100;
        public string FontForegroundColor { get; set; } = "#ffffff";
    }
    public enum UnitSystemType
    {
        Metric,
        Imperial
    }

    public enum TemperatureFormatType
    {
        None = 0,
        C = 1,
        F = 2,
        CF = 3,
        FC = 4
    }

    public enum TimeFormatType
    {
        None = 0,
        T12hs = 1,
        T24hs = 2
    }
    public enum DateFormatType
    {
        None = 0,
        Date1_xWD_M_D = 1,    // Date1: TUE, Set 23
        Date2_xWDDD_M_D = 2,  // Date2: TUESDAY, Set 23
        Date3_WD_D = 3,       // Date2: Tuesday 23
        Date4_WD = 4,         // Date3: Tuesday
        Date5_DD_MMM_YY = 5,  // Date4: 23 SEP 2021
        Date6_MMM_DD_YY = 6,  // Date5: SEP 23 2021
        Date7_DD_MM_YY = 7,   // Date6: 23/09/21
        Date8_MM_DD_YY = 8,   // Date7: 09/23/21
    }

}
