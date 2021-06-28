using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miapp_core
{
    public sealed class WeatherService
    {

        public List<string> Summaries { get; }


        public WeatherService()
        {
            this.Summaries = new List<string>();
        }

    }
}
