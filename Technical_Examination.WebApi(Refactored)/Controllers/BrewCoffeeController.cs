using Microsoft.AspNetCore.Mvc;
using Technical_Examination.WebApi_Refactored_.DataContext;
using Technical_Examination.WebApi_Refactored_.Service;

namespace Technical_Examination.WebApi_Refactored_.Controllers;

[ApiController]
[Route("brew-coffee")]
public class BrewCoffeeController : ControllerBase
{
    public static int _counter = 1;
    private readonly IWeatherAPIService _weatherService;

    public BrewCoffeeController(IWeatherAPIService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<ActionResult<BrewCoffee>> Get([FromQuery] BrewCoffee? cofee_data, double? lat, double? lon)
    {
        try
        {
            var temperature = await _weatherService.GetTemperature(lat, lon);
            var current_date = cofee_data?.Prepared ?? DateTime.Now;
            var current_month = current_date.Month;
            var current_day = current_date.Day;

            if (_counter < 5)
            {
                Interlocked.Increment(ref _counter);
                if (current_month == 4 && current_day == 1)
                    return StatusCode(418, "418 I’m a teapot");

                var coffee = new BrewCoffee()
                {
                    Message = temperature > 30.00 ? "Your refreshing iced coffee is ready" : "Your piping hot coffee is ready",
                    Prepared = cofee_data?.Prepared ?? DateTime.Parse(DateTime.Now.ToString("O"))
                };
                return Ok(coffee);
            }
            else
            {
                return StatusCode(503, "503 Service Unavailable.");
            }
        }
        catch (Exception)
        {

            throw;
        }
        
        
    }
    public void ResetCounter() => _counter = 1;
}
