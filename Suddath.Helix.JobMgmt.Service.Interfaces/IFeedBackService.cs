using System.Collections.Generic;
using System.Threading.Tasks;
using Helix.API.Results;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IFeedBackService
    {
        Task<IEnumerable<FeedbackConfigDto>> GetConfig(bool onlyActive);
        Task<int> AddFeedBack(CreateFeedbackDto dto);
        Task<GetFeedbackDto> GetFeedbackById(int id);
        Task<PagedResults<GetFeedbackDto>> ListAll(int pageNumber, int pageSize);
    }
}
