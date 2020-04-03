using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestODataCore.DbContexts;

namespace TestODataCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // 2.Find the service layer within our scope.
            using (var scope = host.Services.CreateScope())
            {
                //3. Get the instance of DBContext in our services layer
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApiContext>();

                //4. Call the DataGenerator to create sample data
                DbContextSeed.Initialize(services);
            }

            //Continue to run the application
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
