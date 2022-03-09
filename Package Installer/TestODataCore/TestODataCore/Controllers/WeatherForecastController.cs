using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestODataCore.DbContexts;
using TestODataCore.Models;

namespace TestODataCore.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class WeatherForecastsController : ODataController
    {
        private ApiContext _dbContext;



        public WeatherForecastsController(ApiContext context)
        {
            _dbContext = context;
        }


        [EnableQuery]
        public IQueryable<WeatherForecast> Get()
        {
            return _dbContext.WeatherForecasts;
        }

        [EnableQuery]
        public SingleResult<WeatherForecast> Get([FromODataUri] int key)
        {
            IQueryable<WeatherForecast> result = _dbContext.WeatherForecasts.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IActionResult> Post(WeatherForecast weatherForecast)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.WeatherForecasts.Add(weatherForecast);

            await _dbContext.SaveChangesAsync();

            return Created(weatherForecast);
        }


        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<WeatherForecast> weatherForecast)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _dbContext.WeatherForecasts.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            weatherForecast.Patch(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeatherForecastExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        public async Task<IActionResult> Put([FromODataUri] int key, WeatherForecast update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != update.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(update).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeatherForecastExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(update);
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var weatherForecast = await _dbContext.WeatherForecasts.FindAsync(key);
            if (weatherForecast == null)
            {
                return NotFound();
            }

            _dbContext.WeatherForecasts.Remove(weatherForecast);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool WeatherForecastExists(int key)
        {
            return _dbContext.WeatherForecasts.Any(p => p.Id == key);
        }
    }
}
