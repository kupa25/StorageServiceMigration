using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void Queue<TWorkOrder, TWorker>(IBackgroundWorkOrder<TWorkOrder, TWorker> order)
            where TWorker : IBackgroundWorker<TWorkOrder, TWorker>
            where TWorkOrder : IBackgroundWorkOrder<TWorkOrder, TWorker>;

        Task<IBackgroundWorkOrder> DequeueAsync(CancellationToken cancellationToken);
    }
}