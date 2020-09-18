using System.Threading.Tasks;
using Helix.API.Results;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.JobMgmt.Models;

namespace Suddath.Helix.JobMgmt.Services.Interface
{
    public interface IEventBusService
    {
        Task SendIntegrationEvent(IntegrationEvent @event);
    }
}