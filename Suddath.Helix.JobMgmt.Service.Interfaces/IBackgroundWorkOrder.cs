using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IBackgroundWorkOrder { }

    public interface IBackgroundWorkOrder<TWorkOrder, TWorker> : IBackgroundWorkOrder
        where TWorker : IBackgroundWorker<TWorkOrder, TWorker>
        where TWorkOrder : IBackgroundWorkOrder<TWorkOrder, TWorker>
    {
    }
}