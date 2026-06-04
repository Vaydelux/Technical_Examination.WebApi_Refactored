using Microsoft.AspNetCore.Mvc;
using Technical_Examination.WebApi_Refactored_.DataContext;

namespace Technical_Examination.WebApi_Refactored_.Controllers;

[ApiController]
[Route("brew-coffee")]
public class BrewCoffeeController : ControllerBase
{
    public int _counter = 1;

    [HttpGet]
    public ActionResult<BrewCoffee> Get(BrewCoffee? cofee_data)
    {
        var current_date = cofee_data?.Prepared ?? DateTime.Now;
        var current_month = current_date.Month;
        var current_day = current_date.Day;

        if (_counter < 5)
        {
            Interlocked.Increment(ref _counter);
            if (DateTime.Equals(current_month, 4) && DateTime.Equals(current_day, 1))
                return StatusCode(418, "418 I’m a teapot");

            var coffee = new BrewCoffee()
            {
                Message = "Your piping hot coffee is ready",
                Prepared = cofee_data?.Prepared ?? DateTime.Parse(DateTime.Now.ToString("O"))
            };
            return Ok(coffee);
        }
        else
        {
            return StatusCode(503, "503 Service Unavailable.");
        }
    }
    public void ResetCounter() => _counter = 1;
}
