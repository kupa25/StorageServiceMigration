using Suddath.Helix.Common.Infrastructure.EventBus.Events.AccountManagement;
using Suddath.Helix.JobMgmt.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IAccountEntityService
    {
        Task Create(List<AccountEntityDTO> dtoAccountEntities);

        Task Create(AccountEntityCreatedIntegrationEvent dto);

        Task Update(AccountEntityUpdatedIntegrationEvent accountEntity);
    }
}