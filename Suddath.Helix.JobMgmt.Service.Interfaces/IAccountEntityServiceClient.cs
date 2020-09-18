using System.Threading.Tasks;
using Helix.API.Results;
using Suddath.Helix.JobMgmt.Models;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IAccountEntityServiceClient
    {
        Task<PagedResults<AccountEntityDTO>> GetAccountEntities();
    }
}