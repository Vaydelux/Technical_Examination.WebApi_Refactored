using System.Text.Json.Nodes;

namespace Technical_Examination.WebApi_Refactored_.Service;

public class WeatherAPIService : IWeatherAPIService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    public WeatherAPIService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<double> GetTemperature(double? latitude, double? longtitude)
    {
        try
        {
            latitude ??= 40.7128;
            longtitude ??= -74.0060;
            string? weather_api = _configuration["WEATHER_API_KEY"];

            if (string.IsNullOrEmpty(weather_api))
            {
                throw new Exception("WEATHER_API_KEY is not configured.");
            }

            var client = _httpClientFactory.CreateClient("WeatherApi");

            string endpoint = $"data/2.5/weather?lat={latitude}&lon={longtitude}&units=metric&appid={weather_api}";
            var weather = await client.GetFromJsonAsync<JsonObject>(endpoint) ?? throw new Exception("Weather data not found in the response.");
            var main = weather["main"];
            return main?["temp"]?.GetValue<double>() ?? throw new Exception("Temperature data not found in the response.");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
