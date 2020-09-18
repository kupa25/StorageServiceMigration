using Helix.API.Results;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ApplicationPatch;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IVendorServiceClient
    {
        Task<PagedResults<VendorLegacyQueryDto>> GetVendors();
    }
}