namespace Technical_Examination.WebApi_Refactored_.Configurations;

public static class APIConfiguration
{
    public static IServiceCollection AddAPIConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("WeatherApi", client =>
        {
            client.BaseAddress = new Uri("https://api.openweathermap.org/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        return services;
    }
}
