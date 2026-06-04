using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Technical_Examination.WebApi_Refactored_.Controllers;
using Technical_Examination.WebApi_Refactored_.DataContext;
using Technical_Examination.WebApi_Refactored_.Service;

namespace TechnicalExam.Tests_Refactored_;

public class BrewCoffeeTests
{
    private static readonly Random _random = new Random();

    [Fact]
    public void Return503_After5Endpoints()
    {
        var (counter, controller) = LoadController();
        var brewobj = BrewCasesData().ToList();

        if (!brewobj.Any())
            return;

        LoopEndpoints(brewobj, controller, counter);
    }

    [Fact]
    public void Return418_After5Endpoints()
    {
        var (counter, controller) = LoadController();
        var brewobj = AprilBrewCasesData().ToList();

        if (!brewobj.Any())
            return;

        LoopEndpoints(brewobj, controller, counter);
    }

    public static IEnumerable<CoffeeTestCase> BrewCasesData()
    {
        return new[]
        {
            new CoffeeTestCase { Date = "2025-12-17T12:46:00Z", ExpectedStatusCode = 200, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() },
            new CoffeeTestCase { Date = "2021-11-10T20:43:00+08:00", ExpectedStatusCode = 200, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() },
            new CoffeeTestCase { Date = "2022-02-21T14:33:00+02:00", ExpectedStatusCode = 200, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() },
            new CoffeeTestCase { Date = "2024-07-23T08:36:00-04:00", ExpectedStatusCode = 200, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() },
            new CoffeeTestCase { Date = "2026-06-06T12:06:00Z", ExpectedStatusCode = 503, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() }
        };
    }

    public static IEnumerable<CoffeeTestCase> AprilBrewCasesData()
    {
        return new[]
        {
            new CoffeeTestCase { Date = "2025-04-01T12:46:00Z", ExpectedStatusCode = 418, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() },
            new CoffeeTestCase { Date = "2021-04-01T20:43:00+08:00", ExpectedStatusCode = 418, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() },
            new CoffeeTestCase { Date = "2022-04-01T14:33:00+02:00", ExpectedStatusCode = 418, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() },
            new CoffeeTestCase { Date = "2024-04-01T08:36:00-04:00", ExpectedStatusCode = 418, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() },
            new CoffeeTestCase { Date = "2026-04-01T12:06:00Z", ExpectedStatusCode = 503, Latitude = GenerateLatitude(), Longtitude = GenerateLongtitude() }
        };
    }

    private static double GenerateLatitude()
    {
        var latitude = _random.NextDouble() * 180 - 90;

        return Math.Round(latitude, 2);
    }

    private static double GenerateLongtitude()
    {
        var longtitude = _random.NextDouble() * 360 - 180;

        return Math.Round(longtitude, 2);
    }

    private static (int counter, BrewCoffeeController controller) LoadController()
    {
        var controller = new BrewCoffeeController(new FakeWeatherService());
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
            var lat = caseData.Latitude;
            var lon = caseData.Longtitude;

            var new_coffee = new BrewCoffee
            {
                Prepared = DateTime.Parse(date)
            };

            var response = controller.Get(new_coffee, lat, lon).Result;
            var action = response.Result;

            if (action is ObjectResult objresult)
            {
                var actualStatus = objresult.StatusCode ?? 200;

                Assert.Equal(status_code, actualStatus);

                if (status_code == 200 && counter < 5)
                {
                    var coffee = objresult.Value as BrewCoffee ?? response.Value as BrewCoffee;

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
            else if (action is StatusCodeResult statusResult)
            {
                Assert.Equal(status_code, statusResult.StatusCode);
            }
            else if (response.Value is BrewCoffee coffeeVal)
            {
                Assert.Equal(200, status_code);
                Assert.Equal("Your piping hot coffee is ready", coffeeVal.Message);
                Assert.Equal(DateTime.Parse(date), coffeeVal.Prepared);
            }

            counter++;
        }
    }

    public class CoffeeTestCase
    {
        public string Date { get; set; }
        public int ExpectedStatusCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longtitude { get; set; }
    }
}

internal class FakeWeatherService : IWeatherAPIService
{
    public Task<double> GetTemperature(double? latitude, double? longtitude)
    {
        return Task.FromResult(25.00);
    }
}