using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace qckdev.AspNetCore
{

    /// <summary>
    /// Provides helpful methods for application configuration properties.
    /// </summary>
    public static class ConfigurationHelper
    {

        /// <summary>
        /// Searches and replaces environment variables in the application configuration properties. 
        /// Environment variable are defined in the following format: '%VariableName%'.
        /// </summary>
        /// <param name="configuration">Application configuration properties to replace.</param>
        public static void ApplyEnvironmentVariables(IConfiguration configuration)
        {
            // https://regex101.com/r/bCmRKM/1
            var pattern = @"(?:%(?<envar>\w+)%)(?<value>.*)";
            var regex = new Regex(pattern);
            var dictionary = configuration
                .AsEnumerable()
                .ToDictionary(x => x.Key, y => y.Value);

            foreach (var item in dictionary.Where(x => x.Value != null))
            {
                var match = regex.Match(item.Value);

                if (match.Success)
                {
                    var variableName = match.Groups["envar"].Value;

                    if (dictionary.TryGetValue(variableName, out string @envar))
                    {
                        var newValue = match.Result(@$"{@envar}$2");

                        configuration[item.Key] = newValue;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Environment variable not found: {variableName}");
                    }
                }
            }
        }

    }
}
