namespace Technical_Examination.WebApi_Refactored_.Service
{
    public interface IWeatherAPIService
    {
        Task<double> GetTemperature(double? latitude, double? longtitude);
    }
}