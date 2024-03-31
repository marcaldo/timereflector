namespace TimeReflector.Data
{
    public sealed class Settings
    {
        public string AlbumsPath { get; set; } = default!;
        public Temperature Temperature { get; set; } = new();

    }

    public sealed class Temperature
    {
        public TemperatureDisplayTypes Display { get; set; } = TemperatureDisplayTypes.FahrenheitCelsius;
    }

    public enum TemperatureDisplayTypes
    {
        None = 0,
        Celsius = 1,
        Fahrenheit = 2,
        CelsiusFahrenheit = 3,
        FahrenheitCelsius = 4
    }
}
