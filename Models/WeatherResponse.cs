namespace MyAspNetCoreApp.Models
{
    // DTO for Weather API Response
    public class WeatherResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public CurrentWeather? Current { get; set; }
    }

    public class CurrentWeather
    {
        public string? Time { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public int WeatherCode { get; set; }
    }

    // DTO for Exchange Rate API Response
    public class ExchangeRateResponse
    {
        public string? Base { get; set; }
        public string? Date { get; set; }
        public Dictionary<string, double>? Rates { get; set; }
    }
}
