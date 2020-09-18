using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderAirFreight;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderOceanFreight;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderRoadFreight;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderStorage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IServiceOrderService
    {
        #region service orders

        Task<bool> GetServiceOrderExistsAsync(int serviceOrderId);

        Task<bool> IsValidServiceOrder(int jobId, int serviceOrderId);

        Task<int> GetJobIdByServiceOrderIdAsync(int serviceOrderId);

        Task PatchServiceOrderAsync(int jobId, int serviceOrderId, JsonPatchDocument patch, string serviceAbbreviation = null);

        Task UpdateServiceOrderAsync(int jobId, int serviceOrderId, IServiceOrderBaseResponse dto, string serviceAbbreviation = null);

        Task CreateAppropriateServiceOrderSubTable(int id, string serviceAbbreviation);

        Task SetServiceOrderInsuranceVendorDefaultAsync(int serviceOrderId);

        Task<IServiceOrderBaseResponse> GetServiceOrderAsync(int jobId, int serviceOrderId, string serviceName = "");

        #endregion service orders

        #region service order contacts

        Task<IEnumerable<GetServiceOrderContactResponse>> GetServiceOrderContactsAsync(int serviceOrderId);

        Task<CreateServiceOrderContactResponse> CreateServiceOrderContactAsync(int serviceOrderId, CreateServiceOrderContactRequest request);

        Task PatchServiceOrderContactAsync(int id, JsonPatchDocument patch);

        Task UpdateServiceOrderContactAsync(int id, GetServiceOrderContactResponse dto);

        Task DeleteServiceOrderContactAsync(int serviceOrderContactId);

        #endregion service order contacts

        #region ocean freight

        #region legs

        Task<IEnumerable<GetOceanFreightLegResponse>> GetOceanFreightLegsAsync(int serviceOrderId);

        Task<int> CreateOceanFreightLegAsync(int serviceOrderId);

        Task PatchOceanFreightLegAsync(int id, JsonPatchDocument patch);

        Task UpdateOceanFreightLegAsync(int id, GetOceanFreightLegResponse dto);

        Task DeleteOceanFreightLegAsync(int id);

        Task<bool> GetExistsOFLegByTuple(int jobId, int serviceOrderId, int id);

        #endregion legs

        #region lcl

        Task<bool> GetExistsOFLclByTupleAsync(int jobId, int serviceOrderId, int id);

        Task<int> CreateOceanFreightLCLAsync(int serviceOrderId);

        Task<IEnumerable<GetOceanFreightLCLResponse>> GetOceanFreightLCLsAsync(int serviceOrderId);

        Task PatchOceanFreightLCLAsync(int id, JsonPatchDocument patch);

        Task UpdateOceanFreightLCLAsync(int id, GetOceanFreightLCLResponse dto);

        Task DeleteOceanFreightLCLAsync(int id);

        #endregion lcl

        #region vehicles (RoRo)

        Task<bool> GetExistsOFVehicleByTupleAsync(int jobId, int serviceOrderId, int id);

        Task<IEnumerable<GetOceanFreightVehicleResponse>> GetOceanFreightVehiclesAsync(int serviceOrderId);

        Task<int> CreateOceanFreightVehicleAsync(int serviceOrderId, int? containerId);

        Task PatchOceanFreightVehicleAsync(int id, JsonPatchDocument patch);

        Task UpdateOceanFreightVehicleAsync(int id, GetOceanFreightVehicleResponse dto);

        Task DeleteOceanFreightVehicleAsync(int id);

        #endregion vehicles (RoRo)

        #region lift vans

        Task<bool> GetExistsOFLiftVanByTupleAsync(int jobId, int containerId, int id);

        Task<IEnumerable<GetOceanFreightContainerLiftVanResponse>> GetOceanFreightLiftVansAsync(int containerId);

        Task<int> CreateOceanFreightLiftVanAsync(int containerId);

        Task PatchOceanFreightLiftVanAsync(int id, JsonPatchDocument patch);

        Task UpdateOceanFreightLiftVanAsync(int id, GetOceanFreightContainerLiftVanResponse dto);

        Task<string> ValidateUpdateStoragePartial(int id, JsonPatchDocument patch);

        Task DeleteOceanFreightLiftVanAsync(int id);

        #endregion lift vans

        #region loosey

        Task<bool> GetExistsOFLooseItemByTupleAsync(int jobId, int containerId, int id);

        Task<IEnumerable<GetOceanFreightContainerLooseItemResponse>> GetOceanFreightLooseItemsAsync(int containerId);

        Task<int> CreateOceanFreightLooseItemAsync(int containerId);

        Task PatchOceanFreightLooseItemAsync(int id, JsonPatchDocument patch);

        Task UpdateOceanFreightLooseItemAsync(int id, GetOceanFreightContainerLooseItemResponse dto);

        Task DeleteOceanFreightLooseItemAsync(int id);

        #endregion loosey

        #region containers

        Task<IEnumerable<GetOceanFreightContainerResponse>> GetOceanFreightContainersAsync(int serviceOrderId);

        Task<IEnumerable<GetOceanFreightContainerVehicleResponse>> GetOceanFreightContainerVehiclesAsync(int containerId);

        Task<int> CreateOceanFreightContainerAsync(int serviceOrderId);

        Task PatchOceanFreightContainerAsync(int id, JsonPatchDocument patch);

        Task UpdateOceanFreightContainerAsync(int id, GetOceanFreightContainerResponse dto);

        Task DeleteOceanFreightContainerAsync(int id);

        Task<bool> GetExistsOFContainerByTupleAsync(int jobId, int serviceOrderId, int id);

        Task<bool> GetExistsOFContainerVehicleByTupleAsync(int jobId, int serviceOrderId, int containerId, int id);

        Task<bool> GetExistsOFContainerLooseItemByTupleAsync(int jobId, int serviceOrderId, int containerId, int id);

        Task<bool> GetExistsOFContainerLiftVanByTupleAsync(int jobId, int serviceOrderId, int containerId, int id);

        #endregion containers

        #endregion ocean freight

        #region air freight

        #region legs

        Task<bool> GetExistsAFLegByTuple(int jobId, int serviceOrderId, int id);

        Task<IEnumerable<GetAirFreightLegResponse>> GetAirFreightLegsAsync(int serviceOrderId);

        Task<int> CreateAirFreightLegAsync(int serviceOrderId);

        Task PatchAirFreightLegAsync(int id, JsonPatchDocument patch);

        Task UpdateAirFreightLegAsync(int id, GetAirFreightLegResponse dto);

        Task DeleteAirFreightLegAsync(int id);

        #endregion legs

        #region items

        Task<bool> GetExistsAFItemByTuple(int jobId, int serviceOrderId, int id);

        Task<IEnumerable<GetAirFreightItemResponse>> GetAirFreightItemsAsync(int serviceOrderId);

        Task<int> CreateAirFreightItemAsync(int serviceOrderId);

        Task PatchAirFreightItemAsync(int id, JsonPatchDocument patch);

        Task UpdateAirFreightItemAsync(int id, GetAirFreightItemResponse dto);

        Task DeleteAirFreightItemAsync(int id);

        #endregion items

        #endregion air freight

        #region road freight

        #region legs

        Task<IEnumerable<GetRoadFreightLegResponse>> GetRoadFreightLegsAsync(int serviceOrderId);

        Task<int> CreateRoadFreightLegAsync(int serviceOrderId);

        Task PatchRoadFreightLegAsync(int id, JsonPatchDocument patch);

        Task UpdateRoadFreightLegAsync(int id, GetRoadFreightLegResponse dto);

        Task DeleteRoadFreightLegAsync(int id);

        Task<bool> GetExistsRFLegByTuple(int jobId, int serviceOrderId, int id);

        #endregion legs

        #region ltls

        Task<IEnumerable<GetRoadFreightLTLResponse>> GetRoadFreightLTLsAsync(int serviceOrderId);

        Task<int> CreateRoadFreightLTLAsync(int serviceOrderId);

        Task PatchRoadFreightLTLAsync(int id, JsonPatchDocument patch);

        Task UpdateRoadFreightLTLAsync(int id, GetRoadFreightLTLResponse dto);

        Task DeleteRoadFreightLTLAsync(int id);

        Task<bool> GetExistsRFLTLByTuple(int jobId, int serviceOrderId, int id);

        #endregion ltls

        #endregion road freight

        #region storage

        Task<int> CreatePartialStorage(int serviceOrderId);

        Task PatchStoragePartialAsync(int id, JsonPatchDocument patch);

        Task UpdateStoragePartialAsync(int id, GetStoragePartialDeliveryResponse dto);

        Task<IEnumerable<GetStoragePartialDeliveryResponse>> GetStoragePartialDeliveries(int serviceOrderId);

        #endregion storage
    }
}