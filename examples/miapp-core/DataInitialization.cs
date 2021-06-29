using qckdev.AspNetCore.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace miapp_core
{
    sealed class DataInitialization : IDataInitializer
    {

        WeatherService WeatherService { get; }

        public DataInitialization(WeatherService weatherService)
        {
            this.WeatherService = weatherService;
        }


        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            WeatherService.Summaries.AddRange(new[]{
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            });

            return Task.CompletedTask;
        }
    }
}
