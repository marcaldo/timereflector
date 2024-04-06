using ExifLib;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TimeReflector.Data
{
    internal class DisplayManager
    {
        SettingsManager settingsManager = new();

        private DateTimeDisplayData dateTimeDisplayDataValue = new DateTimeDisplayData();

        public static List<DisplayItem> DisplayItems { get; set; } = new();
        public DateTimeDisplayData DateTimeDisplayData { get => RefreshDateTimeDisplay(); }
        public List<DisplayItem> GetDisplayItems()
        {
            var albumPath = settingsManager.Configuration.AlbumsPath;

            var dirInfo = new DirectoryInfo(albumPath);

            if (!dirInfo.Exists) { return new List<DisplayItem>(); }

            List<DisplayItem> displayItems = new();


            foreach (var file in dirInfo.GetFiles())
            {
                var rotateValue = GetRotation(file.FullName);
                displayItems.Add(new DisplayItem
                {
                    ImageFileName = file.Name,
                    IsVideo = !file.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase),
                    Rotate = rotateValue
                });
            }

            return displayItems;
        }

        public DisplayItem? GetNextItem()
        {
            var displayItem = DisplayItems.FirstOrDefault();

            if (displayItem is null)
            {
                DisplayItems = GetDisplayItems();
                displayItem = DisplayItems.FirstOrDefault();
            }

            if (displayItem is not null) DisplayItems.Remove(displayItem);

            return displayItem;
        }

        public DateTimeDisplayData RefreshDateTimeDisplay()
        {
            var now = DateTime.Now;

            switch (settingsManager.Configuration.DateTimeFormat.TimeFormat)
            {
                case TimeFormatType.None:
                    break;
                case TimeFormatType.T12hs:
                    dateTimeDisplayDataValue.Time = now.ToString("hh:mm");
                    dateTimeDisplayDataValue.AmPm = now.ToString("tt");
                    break;
                case TimeFormatType.T24hs:
                default:
                    dateTimeDisplayDataValue.Time = DateTime.Now.ToString("HH:mm");
                    dateTimeDisplayDataValue.AmPm = "";
                    break;
            }

            dateTimeDisplayDataValue.Date = settingsManager.Configuration.DateTimeFormat.DateFormat switch
            {
                DateFormatType.None => "",
                // Date1: TUE, Set 23
                DateFormatType.Date1_xWD_M_D => $"{now.DayOfWeek.ToString()[..3].ToUpper()}, {now:MMM} {now:dd}",
                // Date2: TUESDAY, Set 23
                DateFormatType.Date2_xWDDD_M_D => $"{now.DayOfWeek.ToString().ToUpper()}, {now:MMM} {now:dd}",
                // Date3: Tuesday 23
                DateFormatType.Date3_WD_D => $"{now.DayOfWeek} {now:dd}",
                // Date4: Tuesday
                DateFormatType.Date4_WD => $"{now.DayOfWeek}",
                // Date5: 23 SEP 2021
                DateFormatType.Date5_DD_MMM_YY => $"{now:dd} {now:MMM} {now:yyyy}",
                // Date6: SEP 23 2021
                DateFormatType.Date6_MMM_DD_YY => $"{now:MMM} {now:dd} {now:yyyy}",
                // Date7: 23/09/21
                DateFormatType.Date7_DD_MM_YY => $"{now:dd}/{now:MM}/{now:yyyy}",
                // Date8: 09/23/21
                DateFormatType.Date8_MM_DD_YY => $"{now:MM}/{now:dd}/{now:yyyy}"
            };


            return dateTimeDisplayDataValue;
        }

        private static int GetRotation(string imagePath)
        {
            using ExifReader reader = new ExifReader(imagePath);
            reader.GetTagValue(ExifTags.Orientation, out object orientationValue);

            int.TryParse(orientationValue.ToString(), out int orientation);

            // 1 - Normal - No rotation needed.
            // 2 - Flipped horizontally.
            // 3 - Rotated 180 degrees.
            // 4 - Flipped vertically.
            // 5 - Flipped horizontally and rotated 270 degrees clockwise.
            // 6 - Rotated 90 degrees clockwise.
            // 7 - Flipped horizontally and rotated 90 degrees clockwise.
            // 8 - Rotated 270 degrees clockwise.

            return orientation switch
            {
                3 => 180,
                5 => 270,
                6 => 90,
                7 => 90,
                8 => 270,
                _ => 0
            };
        }
    }
}
