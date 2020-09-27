using Helix.API.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.Constant;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.InsuranceClaim;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderStorage;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class JobsApi
    {
        //private static string _jobsBaseUrl = "https://daue2helixjobwa01.azurewebsites.net/api/v1/Jobs";
        private static string _jobsBaseUrl = "https://localhost:5001/api/v1/Jobs";

        private static List<BillableItemType> billableItemTypes = new List<BillableItemType>();

        #region Jobs Api call

        public static async Task<string> CallJobsApi(HttpClient _httpClient, string url, dynamic model)
        {
            url = _jobsBaseUrl + url;

            if (model != null)
            {
                var payload = JsonConvert.SerializeObject(model);
                var response = await _httpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
                var parsedResponse = await HandleResponse(response);
                return parsedResponse;
            }
            else
            {
                var response = await _httpClient.GetAsync(url);
                var parsedResponse = await HandleResponse(response);
                return parsedResponse;
            }
        }

        public static async Task<T> PostToJobsApi<T>(HttpClient _httpClient, string url, dynamic model)
        {
            url = _jobsBaseUrl + url;
            string payload = string.Empty;

            if (model != null)
            {
                payload = JsonConvert.SerializeObject(model);
            }

            var response = await _httpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var parsedResponse = await HandleResponse(response);

            var result = Convert<SingleResult<T>>(parsedResponse);
            return result.Data;
        }

        public static async Task<string> Patch(HttpClient _httpClient, string url, dynamic model)
        {
            url = _jobsBaseUrl + url;

            var payload = JsonConvert.SerializeObject(model);
            var response = await _httpClient.PatchAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var parsedResponse = await HandleResponse(response);
            return parsedResponse;
        }

        internal static async Task CreateAndUpdateJobCostExpense(HttpClient httpClient, List<Vendor> _vendor, List<Suddath.Helix.JobMgmt.Services.Water.DbContext.PaymentSent> paymentSends,
            List<BillableItemType> billableItemTypes, int jobId, ServiceOrder serviceOrder)
        {
            Console.WriteLine("Starting JC creation");
            Trace.WriteLine("Starting JC creation");

            var url = $"/{jobId}/superServices/orders/{serviceOrder.SuperServiceOrderId}/payableItems";

            foreach (var legacyJC in paymentSends)
            {
                var orignal = await PostToJobsApi<GetPayableItemResponse>(httpClient, url, null);
                var duplicateObjString = await CallJobsApi(httpClient, url + $"/{orignal.Id}", null);
                var duplicateObj = Convert<GetPayableItemResponse>(duplicateObjString);

                duplicateObj.PayableItemTypeId = billableItemTypes.Single(bi => bi.AccountCode.Equals(legacyJC.ACCOUNT_CODE.Substring(0, 2))).Id;

                //duplicateObj.VendorInvoiceNumber = legacyJC.INVOICE_NUMBER; // Probably.
                //duplicateObj.CheckWireNumber = legacyJC.CHECK; //TODO: have to create vendor invoice record.. Arghh
                //duplicateObj.BillFromType = "Vendor"; //TODO: is this true???

                //await GenerateAndPatch(httpClient, url + $"/{orignal.Id}", orignal, duplicateObj);
            }
        }

        public static async Task<string> HandleResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"{response.ReasonPhrase} : {content}");
            }

            return content;
        }

        private static T Convert<T>(string parsedResponse)
        {
            try
            {
                var result = ((!string.IsNullOrEmpty(parsedResponse)) ? JsonConvert.DeserializeObject<T>(parsedResponse) : default(T));
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("*********ERROR parsing response**");
                Trace.WriteLine("*********ERROR parsing response**");
            }

            return default(T);
        }

        private static async Task GenerateAndPatch(HttpClient httpClient, string soSTUrl, dynamic origObj, dynamic modifiedObj)
        {
            var patch = new JsonPatchDocument();
            FillPatchForObject(JObject.FromObject(origObj), JObject.FromObject(modifiedObj), patch, "/");

            await Patch(httpClient, soSTUrl, patch);
        }

        #endregion Jobs Api call

        internal static async Task<CreateSuperServiceOrderResponse> CreateStorageSSO(HttpClient _httpClient, int jobId)
        {
            Console.WriteLine("Creating Storage SSO");
            Trace.WriteLine("Creating Storage SSO");

            var url = $"/{jobId}/superServices/order";
            var model = new CreateSuperServiceOrderRequest { SuperServiceId = 4 };
            var parsedResponse = await CallJobsApi(_httpClient, url, model);

            CreateSuperServiceOrderResponse response = null;

            try
            {
                response = ((!string.IsNullOrEmpty(parsedResponse)) ? JsonConvert.DeserializeObject<CreateSuperServiceOrderResponse>(parsedResponse) : null);
            }
            catch (Exception ex) { }

            return response;
        }

        #region Update MileStone

        internal static async Task UpdateOriginMilestone(HttpClient httpClient, int serviceOrderId, Move move, int jobId)
        {
            Console.WriteLine("Updating OA");
            Trace.WriteLine("Updating OA");

            var origin = move.OriginAgent;
            var storageEntity = move.StorageAgent;

            bool modified = false;
            var soUrl = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=OA";

            var original = await CallJobsApi(httpClient, soUrl, null);
            var copyOfOriginal = original;

            var origObj = Convert<GetServiceOrderOriginAgentResponse>(original);
            var modifiedObj = Convert<GetServiceOrderOriginAgentResponse>(copyOfOriginal);

            //All docs received.
            if (origin != null && origin.DOCS_RCV_DATE != null)
            {
                modified = true;

                modifiedObj.IsAllDocumentsReceived = true;
            }

            if (storageEntity.SITinDate.HasValue)
            {
                modified = true;

                modifiedObj.ActualPickupStartDate = storageEntity.SITinDate.Value;
                modifiedObj.ActualPickupEndDate = storageEntity.SITinDate.Value;
            }

            //currentweight - if null on ma ..m.weight - da.weight.
            if (storageEntity.SurveyWeight.HasValue)
            {
                modified = true;

                modifiedObj.NetWeightLb = storageEntity.SurveyWeight.Value;
            }

            if (modified)
            {
                var patch = new JsonPatchDocument();
                FillPatchForObject(JObject.FromObject(origObj), JObject.FromObject(modifiedObj), patch, "/");

                await Patch(httpClient, soUrl, patch);
            }
        }

        internal static async Task UpdateDestinationMilestone(HttpClient httpClient, int serviceOrderId, Move move, int jobId)
        {
            Console.WriteLine("Updating DA");
            Trace.WriteLine("Updating DA");

            var origin = move.MoveAgents.FirstOrDefault(ma => ma.JobCategory.Equals("DESTINATION"));

            var soUrl = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=DA";

            var original = await CallJobsApi(httpClient, soUrl, null);
            var copyOfOriginal = original;

            var origObj = Convert<GetServiceOrderDestinationAgentResponse>(original);
            var modifiedObj = Convert<GetServiceOrderDestinationAgentResponse>(copyOfOriginal);

            //All docs received.
            if (origin != null && origin.DOCS_RCV_DATE != null)
            {
                //TODO: do we need to implement this in the api??
                //modifiedObj.IsAllDocumentsReceived = true;

                var patch = new JsonPatchDocument();
                FillPatchForObject(JObject.FromObject(origObj), JObject.FromObject(modifiedObj), patch, "/");

                await Patch(httpClient, soUrl, patch);
            }
        }

        #region Storage

        internal static async Task UpdateStorageMilestone(HttpClient httpClient, int serviceOrderId, Move move, int jobId, Vendor vendorEntity)
        {
            var legacyStorageEntity = move.StorageAgent;

            var soSTUrl = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=ST";

            var original = await CallJobsApi(httpClient, soSTUrl, null);
            var copyOfOriginal = original;

            var origObj = Convert<GetServiceOrderStorageResponse>(original);
            var modifiedObj = Convert<GetServiceOrderStorageResponse>(copyOfOriginal);

            modifiedObj.VendorId = vendorEntity?.Id;
            modifiedObj.StorageCostRate = legacyStorageEntity.COST;
            modifiedObj.StorageCostUnit = legacyStorageEntity.DELY_DOCS;
            await GenerateAndPatch(httpClient, soSTUrl, origObj, modifiedObj);
        }

        internal static async Task<int> AddStorageRevRecord(HttpClient httpClient, int serviceOrderId, Move move, int jobId)
        {
            var soSTUrl = $"/{jobId}/services/orders/{serviceOrderId}/storage/revenues";
            var result = await PostToJobsApi<int>(httpClient, soSTUrl, null);

            return result;
        }

        internal static async Task updateStorageRevRecord(HttpClient httpClient, int soId, int storageRevId, Move move, int jobId)
        {
            Console.WriteLine("Update ST Rev Record");
            Trace.WriteLine("Update ST Rev Record");

            var url = $"/{jobId}/services/orders/{soId}/storage/revenues";
            var legacyStorageEntity = move.StorageAgent;

            var original = await CallJobsApi(httpClient, url, null);
            var copyOfOriginal = original;

            var origObj = Convert<SingleResult<List<GetStorageRevenueResponse>>>(original).Data.FirstOrDefault();
            var modifiedObj = Convert<SingleResult<List<GetStorageRevenueResponse>>>(copyOfOriginal).Data.FirstOrDefault();

            var freePeriodId = legacyStorageEntity.EXAM_AMOUNT1;

            modifiedObj.FreePeriodStartDate = FreePeriodDate.StartDate(freePeriodId, move.DateEntered.GetValueOrDefault().Year);
            modifiedObj.FreePeriodEndDate = FreePeriodDate.StartDate(freePeriodId, move.DateEntered.GetValueOrDefault().Year);

            await GenerateAndPatch(httpClient, url + $"/{origObj.Id}", origObj, modifiedObj);
        }

        #endregion Storage

        internal static async Task UpdateICtMilestone(HttpClient httpClient, int serviceOrderId, Move move, int jobId, List<InsuranceClaims> legacyInsuranceClaims)
        {
            Console.WriteLine("Updating IC");
            Trace.WriteLine("Updating IC");

            var url = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=IC";

            var original = await CallJobsApi(httpClient, url, null);
            var copyOfOriginal = original;

            var origObj = Convert<GetServiceOrderInsuranceClaimResponse>(original);
            var modifiedObj = Convert<GetServiceOrderInsuranceClaimResponse>(copyOfOriginal);

            var record = legacyInsuranceClaims.FirstOrDefault();

            modifiedObj.HHGAmount = record.REQ_AMOUNT;
            modifiedObj.HighValueAmount = record.HIGH_VALUE_AMOUNT;
            modifiedObj.VehicleAmount = record.VEHICLE_AMOUNT;

            await GenerateAndPatch(httpClient, url, origObj, modifiedObj);
        }

        #endregion Update MileStone

        private static void FillPatchForObject(JObject orig, JObject mod, JsonPatchDocument patch, string path)
        {
            var origNames = orig.Properties().Select(x => x.Name).ToArray();
            var modNames = mod.Properties().Select(x => x.Name).ToArray();

            // Names removed in modified
            foreach (var k in origNames.Except(modNames))
            {
                var prop = orig.Property(k);
                patch.Remove(path + prop.Name);
            }

            // Names added in modified
            foreach (var k in modNames.Except(origNames))
            {
                var prop = mod.Property(k);
                patch.Add(path + prop.Name, prop.Value);
            }

            // Present in both
            foreach (var k in origNames.Intersect(modNames))
            {
                var origProp = orig.Property(k);
                var modProp = mod.Property(k);

                if (origProp.Value.Type != modProp.Value.Type)
                {
                    patch.Replace(path + modProp.Name, modProp.Value);
                }
                else if (!string.Equals(
                                origProp.Value.ToString(Newtonsoft.Json.Formatting.None),
                                modProp.Value.ToString(Newtonsoft.Json.Formatting.None)))
                {
                    if (origProp.Value.Type == JTokenType.Object)
                    {
                        // Recurse into objects
                        FillPatchForObject(origProp.Value as JObject, modProp.Value as JObject, patch, path + modProp.Name + "/");
                    }
                    else
                    {
                        // Replace values directly
                        patch.Replace(path + modProp.Name, modProp.Value);
                    }
                }
            }
        }
    }
}