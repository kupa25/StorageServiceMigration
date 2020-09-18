using Suddath.Helix.JobMgmt.Models.RequestModels;
using System.Threading.Tasks;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.PaymentRequestUpdated;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IServiceItemService
    {
        #region service items

        Task<bool> IsValidServiceitemAsync(int serviceItemId);

        Task<int> CreateServiceItemsPaymentRequestAsync(CreateServiceItemsPaymentRequest request);

        Task HandlePaymentRequestUpdatedAsync(PaymentRequestUpdatedEventDto data);

        #endregion service items
    }
}