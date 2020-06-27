using BackgroundTasksDemo.BackgroundWorker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundTasksDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IBackgroundTaskQueue queue;
        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBackgroundTaskQueue queue)
        {
            this.logger = logger;
            this.queue = queue;
        }

        /// <summary>
        /// This is a long running job
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            // Purposeful delay to make this long running
            Thread.Sleep(20000);

            var rng = new Random();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// A method to queue a long running job and quickly return Accepted status
        /// If a Guid provided, this will check and return the result of that 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getasync/{id?}")]
        public IActionResult GetAsync(Guid? id)
        { 
            if (id != null)
            {
                IEnumerable<WeatherForecast> weatherForecastResult = Enumerable.Empty<WeatherForecast>();
                var hasResult = DataStore.DataStore.Results.TryGetValue((Guid)id, out weatherForecastResult);
                var isScheduledItem = DataStore.DataStore.ScheduledItems.Contains((Guid)id);
                if (hasResult)
                    return Ok(weatherForecastResult);
                else if (isScheduledItem)
                    return Accepted(id);
                else
                    return NotFound(id);
            }
            else
            {
                Guid taskId = Guid.NewGuid();

                // queue the long running job and return Accepted
                queue.QueueBackgroundWorkItem(token => { return (taskId, Get()); });

                DataStore.DataStore.ScheduledItems.Add(taskId);

                return Accepted(taskId);
            }
        }
    }
}
