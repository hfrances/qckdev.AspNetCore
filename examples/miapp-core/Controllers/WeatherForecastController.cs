using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using qckdev.AspNetCore;
using qckdev.AspNetCore.Exceptions;
using qckdev.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miapp_core.Controllers
{
    [ApiController, Route("[controller]")]
    public class WeatherForecastController : ApiControllerBase
    {
        WeatherService WeatherService { get; }

        public WeatherForecastController(WeatherService weatherService, ILogger<WeatherForecastController> logger)
        {
            this.WeatherService = weatherService;
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

        [HttpGet("errorext")]
        public IEnumerable<WeatherForecast> GetErrorExt()
        {
            throw new HttpHandledException(System.Net.HttpStatusCode.InternalServerError, "Some HttpHandledException with details.")
            {
                Content = new
                {
                    Property1 = "Value1",
                    Property2 = "Value2"
                }
            };
        }

    }
}
