using System;
using System.Collections.Generic;
using System.IO;

namespace TimeReflector.Data
{
    internal class DisplayManager
    {
        SettingsManager settingsManager = new();

        public IList<DisplayItem> GetDisplayItems()
        {
            var albumPath = settingsManager.Configuration.AlbumsPath;

            var dirInfo = new DirectoryInfo(albumPath);

            if (!dirInfo.Exists ) { return new  List<DisplayItem>(); }

            List<DisplayItem> displayItems = new();


            foreach (var file in dirInfo.GetFiles())
            {
                displayItems.Add(new DisplayItem
                {
                    ImageFileName = file.Name,
                    IsVideo = !file.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
                });
            }

            return displayItems;
        }
    }
}
