using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundTasksDemo.DataStore
{
    // Quick and lazy inmemory storage
    public sealed class DataStore
    {
#pragma warning disable S2386 // Mutable fields should not be "public static" - To setup a quick and lazy inmemory storage - its ugly
        public static Dictionary<Guid, IEnumerable<WeatherForecast>> Results = new Dictionary<Guid, IEnumerable<WeatherForecast>>();
        public static List<Guid> ScheduledItems = new List<Guid>();
#pragma warning restore S2386 // Mutable fields should not be "public static"
    }
}
