using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ExifLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TimeReflector.Data
{
    internal class DisplayManager
    {
        SettingsManager _settingsManager;
        readonly string _displayListFile;

        public DisplayManager()
        {
            _settingsManager = new();
            DisplayItems = [];
            _displayListFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "display-list.json");
        }

        private DateTimeDisplayData dateTimeDisplayDataValue = new DateTimeDisplayData();

        public static List<DisplayItem> DisplayItems { get; set; } = new();
        public DateTimeDisplayData DateTimeDisplayData { get => RefreshDateTimeDisplay(); }
        public List<DisplayItem> GetDisplayItems()
        {
            var storedList = LoadDisplayList();
            if (storedList is not null) return storedList;

            var albumPath = Path.Combine(
                _settingsManager.Configuration.AlbumsPath ?? "",
                _settingsManager.Configuration.SelectedAlbum ?? ""
                );

            var dirInfo = new DirectoryInfo(albumPath);

            if (!dirInfo.Exists) { return []; }


            string fileTypesPattern = "*.jpg|*.jpeg|*.png|*.gif|*.bmp|*.JPG|*.JPEG|*.PNG|*.GIF|*.BMP";
            string[] patterns = fileTypesPattern.Split('|');

            Hashtable displayItemsDictionary = new();

            foreach (string pattern in patterns)
            {
                var files = dirInfo.GetFiles(pattern);

                foreach (var file in files)
                {
                    var rotateValue = GetRotation(file.FullName);
                    var isVideo = file.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase);

                    // Currently nos supporting video
                    if (isVideo) { continue; }

                    var displayItem = new DisplayItem
                    {
                        ImageFileName = file.Name,
                        IsVideo = isVideo,
                        Rotate = rotateValue
                    };

                    var tmpKey = displayItem.ImageFileName.ToLower();

                    if (!displayItemsDictionary.ContainsKey(tmpKey))
                    {
                        displayItemsDictionary.Add(tmpKey, displayItem);
                    }
                }
            }

            List<DisplayItem> displayItems = new();

            foreach (DisplayItem displayItem in displayItemsDictionary.Values)
            {
                displayItems.Add(displayItem);
            }

            var shuffledList = ShuffleDisplayList(displayItems);

            return shuffledList;
        }

        /// <summary>
        /// Clears the current display list.
        /// </summary>
        public void ClearItemList()
        {
            _settingsManager = new();
            DisplayItems.Clear();

            SaveDisplayList(DisplayItems);
        }

        public DisplayItem? GetNextItem()
        {
            var displayItem = DisplayItems.FirstOrDefault();

            if (displayItem is null)
            {
                DisplayItems = GetDisplayItems();
                displayItem = DisplayItems.FirstOrDefault();
            }

            if (displayItem is not null)
            {
                DisplayItems.Remove(displayItem);
                SaveDisplayList(DisplayItems);
            }
            return displayItem;
        }

        public DateTimeDisplayData RefreshDateTimeDisplay()
        {
            var now = DateTime.Now;

            switch (_settingsManager.Configuration.DateTimeFormat.TimeFormat)
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

            dateTimeDisplayDataValue.Date = _settingsManager.Configuration.DateTimeFormat.DateFormat switch
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

        private void SaveDisplayList(List<DisplayItem> displayItems)
        {
            string jsonSettings = JsonSerializer.Serialize(displayItems);
            File.WriteAllText(_displayListFile, jsonSettings);
        }

        private List<DisplayItem>? LoadDisplayList()
        {
            if (!File.Exists(_displayListFile)) return null;

            string displayListJson = File.ReadAllText(_displayListFile);
            var displayList = JsonSerializer.Deserialize<List<DisplayItem>>(displayListJson);

            return displayList is not null && displayList.Any() ? displayList : null;

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
            Random rng = new();

            List<DisplayItem> shuffledList = new List<DisplayItem>(displayItems);

            int n = shuffledList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);

                DisplayItem value = shuffledList[k];
                shuffledList[k] = shuffledList[n];
                shuffledList[n] = value;
            }

            // Remove duplicates
            shuffledList = shuffledList.Distinct().ToList();

            return shuffledList;
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
