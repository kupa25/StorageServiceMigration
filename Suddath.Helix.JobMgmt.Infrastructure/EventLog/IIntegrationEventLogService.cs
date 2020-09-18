using Microsoft.EntityFrameworkCore.Storage;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Infrastructure.EventLog
{
    public interface IIntegrationEventLogService
    {
        Task SaveEventAsync(IntegrationEvent @event);

        Task MarkEventAsPublishedAsync(Guid eventId);

        Task MarkEventAsInProgressAsync(Guid eventId);

        Task MarkEventAsFailedAsync(Guid eventId);
    }
}