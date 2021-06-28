using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miapp_core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        WeatherService WeatherService { get; }
        ILogger<WeatherForecastController> Logger { get; }
       

        public WeatherForecastController(WeatherService weatherService, ILogger<WeatherForecastController> logger)
        {
            this.WeatherService = weatherService;
            this.Logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = WeatherService.Summaries[rng.Next(WeatherService.Summaries.Count)]
            })
            .ToArray();
        }


        [HttpGet("error")]
        public IEnumerable<WeatherForecast> GetError()
        {
            var summaries = new string[] { };
            var rng = new Random();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = summaries[rng.Next(summaries.Length)]
            })
            .ToArray();
        }

    }
}
