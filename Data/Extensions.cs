namespace TimeReflector.Data
{
    internal static class Extensions
    {
        public static int ToMilisecondsSeconds(this int interval, TimeConversionType timeConversionType)
        {
            return timeConversionType switch
            {
                TimeConversionType.FromSeconds => interval * 1000,
                TimeConversionType.FromMinutes => interval * 60000,
                _ => throw new System.ArgumentException(null, nameof(timeConversionType))
            };
        }
    }

    public enum TimeConversionType
    {
        FromSeconds,
        FromMinutes
    }
}
