using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundTasksDemo.BackgroundWorker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BackgroundTasksDemo.Controllers
{
    [ApiController]
    // Below is commented because of: https://twitter.com/Isham_M_Iqbal/status/1274724738682613761
    //[Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private IBackgroundTaskQueue queue;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBackgroundTaskQueue queue)
        {
            this.logger = logger;
            this.queue = queue;
        }

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

        [HttpGet]
        [Route("getasync")]
        public IActionResult GetAsync()
        {
            queue.QueueBackgroundWorkItem(async token =>
            {
                Get();
            });

            return Accepted();
        }
    }
}
