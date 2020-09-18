using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface ILegacyMoveService
    {
        SystemHint[] Hints { get; }
        Task<ServiceDto> GetMoveFromOrderNumberAsync(string order);
        Task<ServiceDto> GetMoveFromIdAsync(string id);
        MoveIdPrefix IdPrefix { get; }
    }
}
