///Credits: https://stackoverflow.com/questions/52163500/net-core-web-api-with-queue-processing

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundTasksDemo.BackgroundWorker
{
    public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, (Guid, Task<IEnumerable<WeatherForecast>>)>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, (Guid, Task<IEnumerable<WeatherForecast>>)>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(
            Func<CancellationToken, (Guid, Task<IEnumerable<WeatherForecast>>)> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, (Guid, Task<IEnumerable<WeatherForecast>>)>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
