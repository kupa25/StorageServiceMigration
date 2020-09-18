using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface ILookupService
    {
        Task<ICollection<GetSuperServicesResponse>> GetSuperServices(string serviceCategory, string availableTo);

        Task<ICollection<GetSuperServiceOrderStatusResponse>> GetSuperServiceOrderStatuses();

        Task<ICollection<GetJobStatusResponse>> GetJobStatuses();

        Task<string> GetCountryNameByAbbreviation3Async(string abbreviation3);
    }
}