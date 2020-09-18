using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.Survey;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface ISurveyService
    {
        #region survey info

        Task<int> CreateSurveyInfoAsync(int jobId);

        Task PatchSurveyInfoAsync(int jobId, JsonPatchDocument patch);

        Task UpdateSurveyInfoAsync(int jobId, GetSurveyInfoResponse dto);

        Task<GetSurveyInfoResponse> GetSurveyInfoAsync(int jobId);

        Task<bool> GetSurveyInfoExistsAsync(int jobId);

        #endregion survey info

        #region survey result

        Task<int> CreateSurveyResultAsync(int superServiceOrderId);

        Task PatchSurveyResultAsync(int superServiceOrderId, JsonPatchDocument patch);

        Task UpdateSurveyResultAsync(int superServiceOrderId, GetSurveyResultResponse dto);

        Task<List<GetSurveyResultResponse>> GetSurveyResultsAsync(int jobId);

        Task<bool> GetSurveyResultExistsAsync(int jobId);

        #endregion survey result

        #region survey result accessorial

        Task<int> CreateSurveyResultAccessorialAsync(int superServiceOrderId, CreateSurveyResultAccessorialRequest request);

        Task PatchSurveyResultAccessorialAsync<T>(int id, JsonPatchDocument patch);

        Task UpdateSurveyResultAccessorialAsync<T>(int id, T dto);

        Task<bool> GetSurveyResultAccessorialOrThirdPartyExistsAsync(int jobId, int superServiceOrderId, int accessorialId);

        Task DeleteSurveyResultAccessorialAsync(int accessorialId);

        #endregion survey result accessorial

        #region survey result 3rd party service

        Task<int> CreateSurveyResultThirdPartyServiceAsync(int superServiceOrderId, CreateSurveyResultThirdPartyServiceRequest request);

        #endregion survey result 3rd party service

        #region survey result crate

        Task<int> CreateSurveyResultCrateAsync(int superServiceOrderId, CreateSurveyResultCrateRequest request);

        Task PatchSurveyResultCrateAsync(int id, JsonPatchDocument patch);
        
        Task UpdateSurveyResultCrateAsync(int id, GetSurveyResultCrateResponse dto);

        Task<bool> GetSurveyResultCrateExistsAsync(int jobId, int superServiceOrderId, int crateId);

        Task DeleteSurveyResultCrateAsync(int crateId);

        #endregion survey result crate
    }
}