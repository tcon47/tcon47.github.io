using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Default;
//using TestODataCore.Models;
using TestODataCore;

namespace TestClient
{
    class Program
    {
        // TODO: Find out why this is in "DEFAULT" NameSpace
        private static Container _odataContext;

        static void Main(string[] args)
        {
            // set the odata context
            _odataContext = new Container(new Uri("http://localhost:18316/odata/"));

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }

        private static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Get all records from the service");
            Console.WriteLine("2) Get a single record from the service");
            Console.WriteLine("3) Add a record to the service");
            Console.WriteLine("4) Update a record on the service letting the context set the entity state ** NOTE ** This IS NOT working!!!");
            Console.WriteLine("5) Update a record on the service by manually changing the entity state ** NOTE ** This IS working!!!");
            Console.WriteLine("6) Deleta a record from the service");
            Console.WriteLine("7) Exit");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    // get all the data in the database
                    GetAllRecords();
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "2":
                    // get a single record from the database
                    GetSingleRecord();
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "3":
                    // add a record to the database
                    AddRecord();
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "4":
                    // update a record from the database
                    UpdateRecord();
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "5":
                    // update a record from the database
                    UpdateRecord(true);
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "6":
                    // delete a record in the database
                    DeleteRecord();
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "7":
                    return false;
                default:
                    return true;
            }
        }

        static void GetAllRecords()
        {
            try
            {
                var data = _odataContext.WeatherForecasts.ToList();

                if (data?.Count > 0)
                {
                    Console.WriteLine($"Downloaded {data.Count.ToString()} records.");

                    foreach (var item in data)
                    {
                        Console.WriteLine($"ID = {item.Id.ToString()}");
                        Console.WriteLine($"Date = {item.Date.ToString()}");
                        Console.WriteLine($"TemperatureC = {item.TemperatureC.ToString()}");
                        Console.WriteLine($"Summary = {item.Summary}");
                        Console.WriteLine("");
                    }
                }
                else
                {
                    Console.WriteLine("No records were downloaded.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                Console.WriteLine(ex.ToString());
            }
        }

        static void GetSingleRecord()
        {
            try
            {
                var data = _odataContext.WeatherForecasts.FirstOrDefault();

                if (data != null)
                {
                    Console.WriteLine("Downloaded 1 record.");

                    Console.WriteLine($"ID = {data.Id.ToString()}");
                    Console.WriteLine($"Date = {data.Date.ToString()}");
                    Console.WriteLine($"TemperatureC = {data.TemperatureC.ToString()}");
                    Console.WriteLine($"Summary = {data.Summary}");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("No record was downloaded.");
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                Console.WriteLine(ex.ToString());
            }
        }

        static void AddRecord()
        {
            try
            {
                WeatherForecast weatherForecast = new WeatherForecast()
                {
                    Date = DateTimeOffset.Now,
                    TemperatureC = 32,
                    Summary = "NEW RECORD"
                };

                _odataContext.AddToWeatherForecasts(weatherForecast);

                // TODO: Find out why entity tracking is not working
                Console.WriteLine("Entity state after ADD = " + _odataContext.EntityTracker.Entities.Where(et => et.Entity == weatherForecast).FirstOrDefault().State.ToString());

                _odataContext.SaveChanges();

                WeatherForecast data = _odataContext.WeatherForecasts.Where(w => w.Id == weatherForecast.Id).FirstOrDefault();

                if (data != null)
                {
                    Console.WriteLine("Added 1 record.");

                    Console.WriteLine($"ID = {data.Id.ToString()}");
                    Console.WriteLine($"Date = {data.Date.ToString()}");
                    Console.WriteLine($"TemperatureC = {data.TemperatureC.ToString()}");
                    Console.WriteLine($"Summary = {data.Summary}");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("No record was added.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                Console.WriteLine(ex.ToString());
            }
        }

        static void UpdateRecord(bool setModifiedState = false)
        {
            try
            {
                WeatherForecast weatherForecast = _odataContext.WeatherForecasts.FirstOrDefault();

                weatherForecast.Summary = "UPDATED";

                // TODO: Find out why entity tracking is not working
                Console.WriteLine("Entity state after UPDATE = " + _odataContext.EntityTracker.Entities.Where(et => et.Entity == weatherForecast).FirstOrDefault().State.ToString());


                if (setModifiedState == true)
                {
                    Console.WriteLine("Manually updated the entity state to Modified.");
                    _odataContext.ChangeState(weatherForecast, Microsoft.OData.Client.EntityStates.Modified);
                }

                // TODO: Find out why this is not working
                _odataContext.SaveChanges();

                // Get the updated version from the database using the proxy client
                // TODO: Find out why this is cached and not pulling down the "real" version?
                WeatherForecast updatedData = _odataContext.WeatherForecasts.Where(w => w.Id == weatherForecast.Id).FirstOrDefault();

                // get the "real" version from the database using http client
                WeatherForecast realData = Utilities.GetRemoteDataAsync<WeatherForecast>().Result.Where(w => w.Id == weatherForecast.Id).FirstOrDefault();

                // show the "updated" version
                Console.WriteLine("Local record.");

                Console.WriteLine($"ID = {updatedData.Id.ToString()}");
                Console.WriteLine($"Date = {updatedData.Date.ToString()}");
                Console.WriteLine($"TemperatureC = {updatedData.TemperatureC.ToString()}");
                Console.WriteLine($"Summary = {updatedData.Summary}");
                Console.WriteLine("");

                // show the "real" version
                Console.WriteLine("Server record.");

                Console.WriteLine($"ID = {realData.Id.ToString()}");
                Console.WriteLine($"Date = {realData.Date.ToString()}");
                Console.WriteLine($"TemperatureC = {realData.TemperatureC.ToString()}");
                Console.WriteLine($"Summary = {realData.Summary}");
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                Console.WriteLine(ex.ToString());
            }
        }

        static void DeleteRecord()
        {
            try
            {
                WeatherForecast weatherForecast = _odataContext.WeatherForecasts.FirstOrDefault();

                _odataContext.DeleteObject(weatherForecast);

                // TODO: Find out why entity tracking is not working
                Console.WriteLine("Entity state after DELETE = " + _odataContext.EntityTracker.Entities.Where(et => et.Entity == weatherForecast).FirstOrDefault().State.ToString());

                _odataContext.SaveChanges();

                // get the "real" version from the database using http client
                WeatherForecast realData = Utilities.GetRemoteDataAsync<WeatherForecast>().Result.Where(w => w.Id == weatherForecast.Id).FirstOrDefault();

                if (realData == null)
                {
                    Console.WriteLine("Deleted 1 record.");
                }
                else
                {
                    Console.WriteLine("No record was deleted.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
