using Helix.API.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
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
        private static string _jobsBaseUrl = "https://daue2helixjobwa01.azurewebsites.net/api/v1/Jobs";
        //private static string _jobsBaseUrl = "https://localhost:5001/api/v1/Jobs";

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

        public static async Task<T> PostToJobsApi<T>(HttpClient _httpClient, string url, dynamic model, string regNumber)
        {
            url = _jobsBaseUrl + url;
            string payload = string.Empty;

            if (model != null)
            {
                payload = JsonConvert.SerializeObject(model);
            }

            var response = await _httpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var parsedResponse = await HandleResponse(response);

            var result = Convert<SingleResult<T>>(parsedResponse, regNumber);
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

        public static async Task<string> HandleResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"{response.ReasonPhrase} : {content}");
            }

            return content;
        }

        private static T Convert<T>(string parsedResponse, string regNumber)
        {
            try
            {
                var result = ((!string.IsNullOrEmpty(parsedResponse)) ? JsonConvert.DeserializeObject<T>(parsedResponse) : default(T));
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("*********ERROR parsing response**");
                Trace.WriteLine($"{regNumber}, *********ERROR parsing response**");
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

        internal static async Task CreateAndUpdateJobCostExpense(HttpClient httpClient, List<Vendor> _vendor, List<Suddath.Helix.JobMgmt.Services.Water.DbContext.PaymentSent> paymentSends,
    List<BillableItemType> billableItemTypes, int jobId, ServiceOrder serviceOrder, string regNumber)
        {
            Console.WriteLine("Starting JC Expense creation");
            Trace.WriteLine($"{regNumber}, Starting JC Expense creation");

            var url = $"/{jobId}/superServices/orders/{serviceOrder.SuperServiceOrderId}/payableItems";
            foreach (var legacyJC in paymentSends)
            {
                var original = await PostToJobsApi<GetPayableItemResponse>(httpClient, url, null, regNumber);
                var originalString = await CallJobsApi(httpClient, url + $"/{original.Id}", null);
                var duplicateObjString = originalString;

                var originalObj = Convert<SingleResult<GetPayableItemResponse>>(originalString, regNumber).Data;
                var modifiedObj = Convert<SingleResult<GetPayableItemResponse>>(duplicateObjString, regNumber).Data;

                if (!string.IsNullOrEmpty(legacyJC.ACCOUNT_CODE))
                {
                    modifiedObj.PayableItemTypeId = billableItemTypes.Single(bi => bi.AccountCode.Equals(legacyJC.ACCOUNT_CODE.Substring(0, 2))).Id;
                }

                modifiedObj.Description = legacyJC.ACCOUNT_DESCRIPTION;
                modifiedObj.BillFromId = legacyJC.VendorID;
                modifiedObj.BillFromType = legacyJC.BillToLabel;
                modifiedObj.AccrualAmountUSD = modifiedObj.AccrualAmountVendorCurrency = legacyJC.ESTIMATED_AMOUNT.GetValueOrDefault() + legacyJC.ADJ_EST_AMOUNT;
                modifiedObj.ActualAmountUSD = modifiedObj.ActualAmountVendorCurrency = legacyJC.AMOUNT.GetValueOrDefault();
                modifiedObj.ActualPostedDateTime = legacyJC.ACTUAL_POSTED;
                modifiedObj.CheckWireNumber = legacyJC.CHECK;
                modifiedObj.VendorInvoiceNumber = legacyJC.INVOICE_NUMBER;

                await GenerateAndPatch(httpClient, url + $"/{original.Id}", originalObj, modifiedObj);

                if (legacyJC.DATE_PAID != null)
                {
                    await JobsDbAccess.CreateVendorInvoiceRecord(original.Id, regNumber, legacyJC.CHECK, legacyJC.INVOICE_NUMBER, legacyJC.DATE_PAID, serviceOrder.SuperServiceOrderId);
                }
            }
        }

        internal static async Task CreateAndUpdateJobCostRevenue(HttpClient httpClient, List<Vendor> vendor, List<Suddath.Helix.JobMgmt.Services.Water.DbContext.PaymentReceived> paymentReceived, List<BillableItemType> billableItemTypes, int jobId, ServiceOrder serviceOrder, string regNumber)
        {
            Console.WriteLine("Starting JC Revenue creation");
            Trace.WriteLine($"{regNumber}, Starting JC Revenue creation");

            var url = $"/{jobId}/superServices/orders/{serviceOrder.SuperServiceOrderId}/billableItems";

            int invoiceCounter = 0;
            foreach (var legacyJC in paymentReceived)
            {
                var original = await PostToJobsApi<CreateBillableItemResponse>(httpClient, url, null, regNumber);
                var originalString = await CallJobsApi(httpClient, url + $"/{original.Id}", null);
                var duplicateObjString = originalString;

                var originalObj = Convert<SingleResult<GetBillableItemResponse>>(originalString, regNumber).Data;
                var modifiedObj = Convert<SingleResult<GetBillableItemResponse>>(duplicateObjString, regNumber).Data;

                if (!string.IsNullOrEmpty(legacyJC.ACCOUNT_CODE))
                {
                    modifiedObj.BillableItemTypeId = billableItemTypes.Single(bi => bi.AccountCode.Equals(legacyJC.ACCOUNT_CODE.Substring(0, 2))).Id;
                }

                modifiedObj.Description = legacyJC.ACCOUNT_DESCRIPTION;

                if (modifiedObj.BillToId != legacyJC.VendorID)
                {
                    modifiedObj.BillToId = legacyJC.VendorID;
                    modifiedObj.BillToType = legacyJC.BillToLabel + " ";//Forcing a change.. verify if this is true
                }

                modifiedObj.AccrualAmountUSD = modifiedObj.AccrualAmountBillingCurrency = legacyJC.ESTIMATED_AMOUNT.GetValueOrDefault() + legacyJC.ADJ_EST_AMOUNT.GetValueOrDefault();
                modifiedObj.ActualAmountUSD = modifiedObj.ActualAmountBillingCurrency = legacyJC.AMOUNT.GetValueOrDefault();
                modifiedObj.ActualPostedDateTime = legacyJC.ACTUAL_POSTED;

                await GenerateAndPatch(httpClient, url + $"/{original.Id}", originalObj, modifiedObj);

                if (legacyJC.DATE_RECEIVED != null)
                {
                    await JobsDbAccess.CreateInvoiceRecord(original.Id, regNumber, string.Empty, legacyJC.INVOICE_NUMBER + "-" + ++invoiceCounter, legacyJC.DATE_RECEIVED, legacyJC.ACTUAL_POSTED, serviceOrder.SuperServiceOrderId);
                }
            }
        }

        internal static async Task<CreateSuperServiceOrderResponse> CreateStorageSSO(HttpClient _httpClient, int jobId, string regNumber)
        {
            Console.WriteLine("Creating Storage Service");
            Trace.WriteLine($"{regNumber}, Creating Storage Service");

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

        internal static async Task UpdateOriginMilestone(HttpClient httpClient, int serviceOrderId, Vendor oaVendor, Move move, int jobId, string regNumber)
        {
            Console.WriteLine("Updating OA");
            Trace.WriteLine($"{regNumber}, Updating OA");

            if (oaVendor == null)
            {
                Console.WriteLine("OA Vendor not found");
                Trace.WriteLine($"{regNumber}, OA Vendor not found");
            }

            var origin = move.OriginAgent;
            var storageEntity = move.StorageAgent;

            var soUrl = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=OA";

            var original = await CallJobsApi(httpClient, soUrl, null);
            var copyOfOriginal = original;

            var origObj = Convert<GetServiceOrderOriginAgentResponse>(original, regNumber);
            var modifiedObj = Convert<GetServiceOrderOriginAgentResponse>(copyOfOriginal, regNumber);

            modifiedObj.IsAllDocumentsReceived = origin.DOCS_RCV_DATE.HasValue;
            modifiedObj.ActualPickupStartDate = storageEntity.SITinDate;
            modifiedObj.ActualPickupEndDate = storageEntity.SITinDate;
            modifiedObj.NetWeightLb = move.NET_WEIGHT;

            if (oaVendor == null)
            {
                Console.WriteLine("OA Vendor not found");
                Trace.WriteLine($"{regNumber}, OA Vendor not found");
            }
            else
            {
                modifiedObj.VendorId = oaVendor.Id;
            }

            var patch = new JsonPatchDocument();
            FillPatchForObject(JObject.FromObject(origObj), JObject.FromObject(modifiedObj), patch, "/");

            await Patch(httpClient, soUrl, patch);
        }

        internal static async Task UpdateDestinationMilestone(HttpClient httpClient, int serviceOrderId, Vendor daVendor, Move move, int jobId, string regNumber)
        {
            Console.WriteLine("Updating DA");
            Trace.WriteLine($"{regNumber}, Updating DA");

            var destination = move.MoveAgents.FirstOrDefault(ma => ma.JobCategory.Equals("DESTINATION"));

            var soUrl = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=DA";

            var original = await CallJobsApi(httpClient, soUrl, null);
            var copyOfOriginal = original;

            var origObj = Convert<GetServiceOrderDestinationAgentResponse>(original, regNumber);
            var modifiedObj = Convert<GetServiceOrderDestinationAgentResponse>(copyOfOriginal, regNumber);

            if (daVendor == null)
            {
                Console.WriteLine("DA Vendor not found");
                Trace.WriteLine($"{regNumber}, DA Vendor not found");
            }
            else
            {
                modifiedObj.VendorId = daVendor.Id;
            }

            var partialDeliveryDate = DateTime.UtcNow;
            if (destination.PartialDeliveryDateIn.HasValue)
            {
                partialDeliveryDate = destination.PartialDeliveryDateIn.Value;
            }
            else
            {
                Trace.WriteLine($"{regNumber}, Defaulting date for Partial deliver because ACT_AR_DATE1 is not present here");
            }

            modifiedObj.ActualDeliveryStartDate = modifiedObj.ActualDeliveryEndDate = partialDeliveryDate;
            modifiedObj.TotalWeightDeliveredLb = destination.SurveyWeight;

            var patch = new JsonPatchDocument();
            FillPatchForObject(JObject.FromObject(origObj), JObject.FromObject(modifiedObj), patch, "/");

            await Patch(httpClient, soUrl, patch);
        }

        #region Storage

        internal static async Task UpdateStorageMilestone(HttpClient httpClient, int serviceOrderId, Move move, int jobId, Vendor vendorEntity, string regNumber, List<InsuranceClaims> legacyInsuranceClaims)
        {
            var legacyStorageEntity = move.StorageAgent;
            var icRecord = legacyInsuranceClaims.FirstOrDefault();

            var soSTUrl = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=ST";

            var original = await CallJobsApi(httpClient, soSTUrl, null);
            var copyOfOriginal = original;

            var origObj = Convert<GetServiceOrderStorageResponse>(original, regNumber);
            var modifiedObj = Convert<GetServiceOrderStorageResponse>(copyOfOriginal, regNumber);

            modifiedObj.VendorId = vendorEntity?.Id;
            modifiedObj.StorageCostRate = legacyStorageEntity.COST;
            modifiedObj.StorageCostUnit = legacyStorageEntity.DELY_DOCS;

            if (icRecord != null)
            {
                modifiedObj.InsuranceCostRate = Math.Round((icRecord.PREMIUM_COST * (icRecord.TOTAL_INSURANCE / 1000)).GetValueOrDefault(), 2);
            }

            modifiedObj.InsuranceCostUnit = "Monthly";

            Trace.WriteLine($"{regNumber}, Couldn't find Insurance Cost Unit.. defaulting it to Monthly");

            await GenerateAndPatch(httpClient, soSTUrl, origObj, modifiedObj);

            //TODO: Validate that we don't need this entry in the partial storage table.  Rather we need the survey weight in the rev section
            //create a partial delivery entry
            //var pdId = await JobsApi.CreatePartialDelivery(httpClient, jobId, serviceOrderId, regNumber);
            ////update the partial delivery record
            //var pdurl = $"/{jobId}/services/orders/{serviceOrderId}/storage/partialDeliveries";

            //var originalPartialDeliv = await CallJobsApi(httpClient, pdurl, null);
            //var copyOfOriginalPartialDeliv = originalPartialDeliv;

            //var origPdObj = Convert<SingleResult<IEnumerable<GetStoragePartialDeliveryResponse>>>(originalPartialDeliv, regNumber).Data.Single(pd => pd.Id == pdId);
            //var modifiedPdObj = Convert<SingleResult<IEnumerable<GetStoragePartialDeliveryResponse>>>(copyOfOriginalPartialDeliv, regNumber).Data.Single(pd => pd.Id == pdId);

            //modifiedPdObj.NetWeightLb = legacyStorageEntity.SurveyWeight.ToString();
            //modifiedPdObj.DateIn = legacyStorageEntity.PartialDeliveryDateIn;

            //await GenerateAndPatch(httpClient, pdurl + $"/{pdId}", origPdObj, modifiedPdObj);
        }

        private static async Task<int> CreatePartialDelivery(HttpClient httpClient, int jobId, int serviceOrderId, string regNumber)
        {
            var url = $"/{jobId}/services/orders/{serviceOrderId}/storage/partialDeliveries";
            var result = await PostToJobsApi<int>(httpClient, url, null, regNumber);
            return result;
        }

        internal static async Task<int> AddStorageRevRecord(HttpClient httpClient, int serviceOrderId, Move move, int jobId, string regNumber)
        {
            var soSTUrl = $"/{jobId}/services/orders/{serviceOrderId}/storage/revenues";
            var result = await PostToJobsApi<int>(httpClient, soSTUrl, null, regNumber);

            return result;
        }

        internal static async Task updateStorageRevRecord(HttpClient httpClient, int soId, int storageRevId, Move move, int jobId, string regNumber, dynamic billTo, string billToLabel, List<InsuranceClaims> legacyInsuranceClaims)
        {
            Console.WriteLine("Update ST Rev Record");
            Trace.WriteLine($"{regNumber}, Update ST Rev Record");

            var url = $"/{jobId}/services/orders/{soId}/storage/revenues";
            var icRecord = legacyInsuranceClaims.FirstOrDefault();
            var legacyStorageEntity = move.StorageAgent;

            var original = await CallJobsApi(httpClient, url, null);
            var copyOfOriginal = original;

            var origObj = Convert<SingleResult<List<GetStorageRevenueResponse>>>(original, regNumber).Data.FirstOrDefault();
            var modifiedObj = Convert<SingleResult<List<GetStorageRevenueResponse>>>(copyOfOriginal, regNumber).Data.FirstOrDefault();

            var freePeriodId = legacyStorageEntity.EXAM_AMOUNT1;
            modifiedObj.FreePeriodStartDate = FreePeriodDate.StartDate(freePeriodId, move.DateEntered.GetValueOrDefault().Year);
            modifiedObj.FreePeriodEndDate = FreePeriodDate.StartDate(freePeriodId, move.DateEntered.GetValueOrDefault().Year);
            modifiedObj.BillingCycle = legacyStorageEntity.PORT_IN;
            modifiedObj.StorageCostRate = legacyStorageEntity.QUOTED;
            modifiedObj.StorageCostUnit = legacyStorageEntity.QUOTE_REF;

            if (icRecord != null)
            {
                modifiedObj.InsuranceCostRate = Math.Round((icRecord.PREMIUM_RATE * (icRecord.TOTAL_INSURANCE / 1000)).GetValueOrDefault(), 2);
            }

            modifiedObj.InsuranceCostUnit = "Monthly";

            Trace.WriteLine($"{regNumber}, Couldn't find Insurance Cost Unit.. defaulting it to Monthly");

            if (billTo != null)
            {
                modifiedObj.BillToId = billTo.Id;
                modifiedObj.BillToType = billToLabel;
            }

            modifiedObj.ContactEmail = legacyStorageEntity.E_MAIL;
            modifiedObj.StorageEffectiveBillDate = legacyStorageEntity.EFFECTIVE_PS_BILL_DATE;
            modifiedObj.BillingRecordEndDate = legacyStorageEntity.EFFECTIVE_PS_RELEASE_DATE;
            modifiedObj.StorageBillableWeightLb = legacyStorageEntity.SurveyWeight;

            await GenerateAndPatch(httpClient, url + $"/{origObj.Id}", origObj, modifiedObj);
        }

        #endregion Storage

        internal static async Task UpdateICtMilestone(HttpClient httpClient, int serviceOrderId, Move move, int jobId, List<InsuranceClaims> legacyInsuranceClaims, string regNumber)
        {
            Console.WriteLine("Updating IC");
            Trace.WriteLine($"{regNumber}, Updating IC");

            var url = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=IC";

            var original = await CallJobsApi(httpClient, url, null);
            var copyOfOriginal = original;

            var origObj = Convert<GetServiceOrderInsuranceClaimResponse>(original, regNumber);
            var modifiedObj = Convert<GetServiceOrderInsuranceClaimResponse>(copyOfOriginal, regNumber);

            var record = legacyInsuranceClaims.FirstOrDefault();

            if (record == null)
            {
                Console.WriteLine($"{regNumber}, Insurance record not found in GMMS");
                Trace.WriteLine($"{regNumber}, Insurance record not found in GMMS");

                return;
            }

            switch (record.CARRIER)
            {
                case "3892":
                    modifiedObj.InsuranceCarrierName = "Pac Global Insurance";
                    break;

                case "CL4":
                    modifiedObj.InsuranceCarrierName = "Account";
                    break;

                case "CL5":
                    modifiedObj.InsuranceCarrierName = "Agent";
                    break;

                default:
                    Trace.WriteLine($"{regNumber}, Couldn't get the CarrierName");
                    break;
            }

            if (!string.IsNullOrEmpty(record.TYPE_OF_POLICY) && record.TYPE_OF_POLICY.Equals("FULL", StringComparison.InvariantCultureIgnoreCase))
            {
                modifiedObj.InsuranceType = "Full Value Inventory";
            }

            modifiedObj.Declaration = record.POLICY_NUMBER;
            modifiedObj.HHGAmount = record.REQ_AMOUNT;
            modifiedObj.HighValueAmount = record.HIGH_VALUE_AMOUNT;
            modifiedObj.VehicleAmount = record.VEHICLE_AMOUNT;
            modifiedObj.TotalInsuranceAmount = record.REQ_AMOUNT.GetValueOrDefault() + record.HIGH_VALUE_AMOUNT.GetValueOrDefault() + record.VEHICLE_AMOUNT.GetValueOrDefault();
            modifiedObj.DeductibleAmount = record.DEDUCTIBLE;
            modifiedObj.QuotedRate = record.PREMIUM_RATE;
            modifiedObj.PayableRate = record.PREMIUM_COST;

            if (!string.IsNullOrEmpty(record.PREMIUM_COST_TYPE) && record.PREMIUM_COST_TYPE.Equals("THOUSAND", StringComparison.InvariantCultureIgnoreCase))
            {
                modifiedObj.PayableRateType = "Per 1000 Insured";
            }

            //******************************** CLAIMS SECTION ****************

            modifiedObj.ClaimNumber = record.CLAIM_NUMBER;
            modifiedObj.ClaimedAmount = record.AMOUNT_PAID;
            modifiedObj.ClaimantDocsCreatedDate = record.CLAIMANT_DOCS;

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