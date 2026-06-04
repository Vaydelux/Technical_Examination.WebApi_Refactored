using Microsoft.AspNetCore.Mvc;
using Technical_Examination.WebApi_Refactored_.Controllers;
using Technical_Examination.WebApi_Refactored_.DataContext;

namespace TechnicalExam.Tests_Refactored_;

public class BrewCoffeeTests
{

    [Fact]
    public void Return503_After5Endpoints()
    {
        var (counter, controller) = LoadController();
        IEnumerable<CoffeeTestCase> brewobj = BrewCasesData() ?? [];
        if (!brewobj.Any() & !(brewobj.Count() > 0 ))
            return;
        LoopEndpoints(brewobj, controller, counter);
    }

    [Fact]
    public void Return418_After5Endpoints()
    {
        var (counter, controller) = LoadController();
        IEnumerable<CoffeeTestCase> brewobj = AprilBrewCasesData() ?? [];
        if (!brewobj.Any())
            return;
        LoopEndpoints(brewobj, controller, counter);
    }

    public static IEnumerable<CoffeeTestCase> BrewCasesData()
    {
        return
        [
            new CoffeeTestCase { Date = "2025-12-17T12:46:00Z", ExpectedStatusCode = 200 },
            new CoffeeTestCase { Date = "2021-11-10T20:43:00+08:00", ExpectedStatusCode = 200 },
            new CoffeeTestCase { Date = "2022-02-21T14:33:00+02:00", ExpectedStatusCode = 200 },
            new CoffeeTestCase { Date = "2024-07-23T08:36:00-04:00", ExpectedStatusCode = 200 },
            new CoffeeTestCase { Date = "2026-06-06T12:06:00Z", ExpectedStatusCode = 503 }
        ];
    }
    public static IEnumerable<CoffeeTestCase> AprilBrewCasesData()
    {
        return
        [
            new CoffeeTestCase { Date = "2025-04-01T12:46:00Z", ExpectedStatusCode = 418 },
            new CoffeeTestCase { Date = "2021-04-01T20:43:00+08:00", ExpectedStatusCode = 418 },
            new CoffeeTestCase { Date = "2022-04-01T14:33:00+02:00", ExpectedStatusCode = 418 },
            new CoffeeTestCase { Date = "2024-04-01T08:36:00-04:00", ExpectedStatusCode = 418 },
            new CoffeeTestCase { Date = "2026-04-01T12:06:00Z", ExpectedStatusCode = 503 }
        ];
    }
    private static (int counter, BrewCoffeeController controller) LoadController()
    {
        var controller = new BrewCoffeeController();
        controller.ResetCounter();
        int counter = 1;
        return (counter, controller);
    }
    private void LoopEndpoints(IEnumerable<CoffeeTestCase> sample_object, BrewCoffeeController controller, int counter)
    {
        foreach (var caseData in sample_object)
        {
            var date = caseData.Date;
            var status_code = caseData.ExpectedStatusCode;

            var new_coffee = new BrewCoffee
            {
                Prepared = DateTime.Parse(date)
            };
            var result = controller.Get(new_coffee);

            if (result.Result is ObjectResult objresult)
            {
                Assert.Equal(status_code, objresult.StatusCode);

                if (status_code == 200 && counter < 5)
                {
                    var coffee = objresult.Value as BrewCoffee;
                    Assert.NotNull(coffee);
                    Assert.Equal("Your piping hot coffee is ready", coffee.Message);
                    Assert.Equal(DateTime.Parse(date), coffee.Prepared);
                }
                else if (status_code == 418 && counter < 5)
                {
                    Assert.Equal("418 I’m a teapot", objresult.Value?.ToString());
                }
                else if (status_code == 503 && counter == 5)
                {
                    Assert.Equal("503 Service Unavailable.", objresult.Value?.ToString());
                }
            }
            else if (result.Result is StatusCodeResult statusResult)
            {
                Assert.Equal(status_code, statusResult.StatusCode);
            }
            counter++;
        }
    }

    public class CoffeeTestCase
    {
        public string Date { get; set; }
        public int ExpectedStatusCode { get; set; }
    }
}
