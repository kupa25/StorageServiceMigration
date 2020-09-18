using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IMoveStrategy
    {
        Task<ServiceDto> GetMoveFromOrderNumberAsync(string order, SystemHint hint= SystemHint.None);
        Task<ServiceDto> GetMoveFromIdAsync(string id);
    }
}
