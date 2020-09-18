using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IBackgroundWorker { }

    public interface IBackgroundWorker<TWorkOrder, TWorker> : IBackgroundWorker
        where TWorker : IBackgroundWorker<TWorkOrder, TWorker>
        where TWorkOrder : IBackgroundWorkOrder<TWorkOrder, TWorker>
    {
        Task DoWork(TWorkOrder order, CancellationToken cancellationToken);
    }
}