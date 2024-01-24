using Microsoft.AspNetCore.Mvc;

namespace Northwind.WebApi.Controllers;

//enables REST-specific behavior for controllers, like automatic HTTP 400 responses for invalid models
[ApiController]
//leavit it like this with controller between square brackets will use a API route path 
//based on the name of the class without the controller part from the end
//[Route("[controller]")] 

// Best practice to differentiate in larger projects the MVC controllers from web API
[Route(Routes.WeatherForecast)]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecastFiveDays")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Get(5);
    }

    [HttpGet(template: "days:int", Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get(int days)
    {
        return Enumerable.Range(1, days).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
