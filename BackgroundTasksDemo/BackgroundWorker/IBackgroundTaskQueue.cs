///Credits: https://stackoverflow.com/questions/52163500/net-core-web-api-with-queue-processing

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundTasksDemo.BackgroundWorker
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, (Guid, Task<IEnumerable<WeatherForecast>>)> workItem);

        Task<Func<CancellationToken, (Guid, Task<IEnumerable<WeatherForecast>>)>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
