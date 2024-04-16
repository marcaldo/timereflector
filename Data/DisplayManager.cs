﻿using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ExifLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TimeReflector.Data
{
    internal class DisplayManager
    {
        SettingsManager settingsManager;

        public DisplayManager()
        {
            settingsManager = new();
            DisplayItems = [];
        }

        private DateTimeDisplayData dateTimeDisplayDataValue = new DateTimeDisplayData();

        public static List<DisplayItem> DisplayItems { get; set; } = new();
        public DateTimeDisplayData DateTimeDisplayData { get => RefreshDateTimeDisplay(); }
        public List<DisplayItem> GetDisplayItems()
        {
            var albumPath = Path.Combine(
                settingsManager.Configuration.AlbumsPath ?? "",
                settingsManager.Configuration.SelectedAlbum ?? ""
                );

            var dirInfo = new DirectoryInfo(albumPath);

            if (!dirInfo.Exists) { return []; }

            List<DisplayItem> displayItems = new();

            string fileTypesPattern = "*.jpg|*.jpeg|*.png|*.gif|*.bmp|*.JPG|*.JPEG|*.PNG|*.GIF|*.BMP";
            string[] patterns = fileTypesPattern.Split('|');

            foreach (string pattern in patterns)
            {
                foreach (var file in dirInfo.GetFiles(pattern))
                {
                    var rotateValue = GetRotation(file.FullName);
                    var isVideo = file.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase);

                    // Currently nos supporting video
                    if (isVideo) { continue; }

                    displayItems.Add(new DisplayItem
                    {
                        ImageFileName = file.Name,
                        IsVideo = isVideo,
                        Rotate = rotateValue
                    });
                }
            }

            return ShuffleDisplayList(displayItems);
        }

        /// <summary>
        /// Clears the current display list.
        /// </summary>
        public void ClearItemList()
        {
            settingsManager = new();
            DisplayItems.Clear();
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

        /// <summary>
        /// Image to use as icon.
        /// </summary>
        /// <param name="iconFileName">Image file name.</param>
        /// <param name="width">Width in pixels. If this is defined and <paramref name="height"/> 
        /// is not, it will set the image as squared. Default is value 30</param>
        /// <param name="height">Height in pixels. If this is defined and <paramref name="width"/> 
        /// is not, it will set the image as squared. Default is value 30</param>
        /// <returns></returns>
        public Image Icon(string iconFileName, double? width = null, double? height = null)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Combine the current directory with the relative path to the image directory
            string imagePath = Path.Combine(currentDirectory, "icons", iconFileName);

            // Create Image control
            var icon = new Image();

            // Set properties
            double iconWidth = width ?? height ?? 30;
            double iconHeight = height ?? width ?? 30;

            icon.Source = new Bitmap(imagePath); // Set the source to the image path
            icon.Width = iconWidth;
            icon.Height = iconHeight;

            return icon;
        }

        private static List<DisplayItem> ShuffleDisplayList(List<DisplayItem> displayItems)
        {
            // Implementing Fisher-Yates algorithm.
            Random rng = new();

            int count = displayItems.Count;
            while (count > 1)
            {
                count--;
                int k = rng.Next(count + 1);
                (displayItems[count], displayItems[k]) = (displayItems[k], displayItems[count]);
            }

            return displayItems;
        }
        private static int GetRotation(string imagePath)
        {
            try
            {
                using ExifReader reader = new ExifReader(imagePath);
                reader.GetTagValue(ExifTags.Orientation, out object orientationValue);

                int.TryParse(orientationValue?.ToString(), out int orientation);

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
            catch
            {
                return 0;
            }
        }
    }
}
