using Newtonsoft.Json;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Infrastructure.EventLog
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly JobDbContext _integrationEventLogContext;
        private readonly List<Type> _eventTypes;

        public IntegrationEventLogService(JobDbContext dbContext)
        {
            _integrationEventLogContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public Task SaveEventAsync(IntegrationEvent @event)
        {
            var eventLogEntry = new IntegrationEventLog()
            {
                EventId = @event.Id,
                CreationTime = @event.CreationDate,
                EventTypeName = @event.GetType().Name,
                Content = JsonConvert.SerializeObject(@event),
                State = (int)EventStateEnum.NotPublished,
                TimesSent = 0
            };

            _integrationEventLogContext.IntegrationEventLog.Add(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _integrationEventLogContext.IntegrationEventLog.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = (int)status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _integrationEventLogContext.IntegrationEventLog.Update(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }
    }
}