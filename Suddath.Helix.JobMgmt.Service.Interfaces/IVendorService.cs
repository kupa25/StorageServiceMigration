using System.Collections.Generic;
using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ApplicationPatch;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IVendorService
    {
        Task<VendorDTO> Create(VendorDTO dto);

        Task<VendorDTO> Update(VendorDTO dto);

        Task Create(List<VendorLegacyQueryDto> dtoVendors);
    }
}