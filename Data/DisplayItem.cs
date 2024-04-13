namespace TimeReflector.Data
{
    internal sealed class DisplayItem
    {   /// <summary>
        /// The file name with its extension. E.g. "picture1.jpg"
        /// </summary>
        public string ImageFileName { get; set; } = default!;
        /// <summary>
        /// Current supported format is mp4.
        /// </summary>
        public bool IsVideo { get; set; } = false;
        /// <summary>
        /// Angle to rotate the image.
        /// </summary>
        public int Rotate { get; set; }
    }
}
