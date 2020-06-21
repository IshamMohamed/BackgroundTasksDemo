using BackgroundTasksDemo.BackgroundWorker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackgroundTasksDemo.Controllers
{
    [ApiController]
    // Below is modified because of: https://twitter.com/Isham_M_Iqbal/status/1274724738682613761
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private IBackgroundTaskQueue queue;
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
        public IEnumerable<WeatherForecast> Get()
        {
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
                // execute the logic to get the status of a previous Get task
                return Ok();
            }
            else
            {
                Guid taskId = Guid.NewGuid();

                // queue the long running job and return Accepted
                queue.QueueBackgroundWorkItem(async token =>
                {
                    Get();
                });

                return Accepted();
            }
        }
    }
}
