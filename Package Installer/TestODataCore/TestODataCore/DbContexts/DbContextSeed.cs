using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestODataCore.Models;

namespace TestODataCore.DbContexts
{
    public class DbContextSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApiContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApiContext>>()))
            {
                for (int i = 1; i < 6; i++)
                {
                    var forecast = new WeatherForecast()
                    {
                        Id = i,
                        Date = DateTime.Now.AddDays(new Random().Next(1, 5)),
                        TemperatureC = new Random().Next(-20, 55),
                        Summary = "Record " + i.ToString()
                    };

                    context.WeatherForecasts.Add(forecast);
                }

                context.SaveChanges();
            }
        }
    }
}
