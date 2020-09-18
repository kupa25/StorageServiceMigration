using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.Survey;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IVangardService
    {
        bool IsVangardTaskOrder(int jobId);

        Task HandleServiceOrderActivitiesAsync(int serviceOrderId, IServiceOrderBaseResponse serviceOrderResponseBefore, IServiceOrderBaseResponse serviceOrderResponseAfter, string serviceAbbreviation);

        Task HandleSurveyInfoActivitiesAsync(int jobId, GetSurveyInfoResponse surveyInfoResponseBefore, GetSurveyInfoResponse surveyInfoResponseAfter);

        Task HandleShipmentStorageInboundAsync(int serviceOrderId);

        Task HandleShipmentStorageOutboundAsync(int serviceOrderId, IServiceOrderBaseResponse pre, IServiceOrderBaseResponse post);
    }
}