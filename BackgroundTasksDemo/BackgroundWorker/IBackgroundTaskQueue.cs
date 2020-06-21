///Credits: https://stackoverflow.com/questions/52163500/net-core-web-api-with-queue-processing

using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundTasksDemo.BackgroundWorker
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
