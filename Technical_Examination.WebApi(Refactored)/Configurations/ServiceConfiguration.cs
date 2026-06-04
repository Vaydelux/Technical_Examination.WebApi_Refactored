using Technical_Examination.WebApi_Refactored_.Service;

namespace Technical_Examination.WebApi_Refactored_.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddServiceConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IWeatherAPIService, WeatherAPIService>();
        return services;
    }
}
