using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using System;
using System.Collections.Generic;
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

        #endregion Jobs Api call

        internal static async Task<CreateSuperServiceOrderResponse> CreateStorageSSO(HttpClient _httpClient, int jobId)
        {
            Console.WriteLine("Creating Storage SSO");
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
            var origin = move.MoveAgents.FirstOrDefault(ma => ma.JobCategory.Equals("ORIGIN"));
            var storageEntity = move.MoveAgents.FirstOrDefault(ma => ma.JobCategory.Equals("STORAGE"));

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

            if (modified)
            {
                var patch = new JsonPatchDocument();
                FillPatchForObject(JObject.FromObject(origObj), JObject.FromObject(modifiedObj), patch, "/");

                await Patch(httpClient, soUrl, patch);
            }
        }

        internal static async Task UpdateDestinationMilestone(HttpClient httpClient, int serviceOrderId, Move move, int jobId)
        {
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

        internal static Task UpdateStorageMilestone(HttpClient httpClient, int serviceOrderId, Move move, int jobId)
        {
            var legacyStorageEntity = move.MoveAgents.FirstOrDefault(ma => ma.JobCategory.Equals("STORAGE"));

            var soSTUrl = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=ST";
            var soQAUrl = $"/{jobId}/services/orders/{serviceOrderId}?serviceName=OA";

            throw new NotImplementedException();
        }

        #endregion Storage

        internal static Task UpdateICtMilestone(HttpClient httpClient, int id, Move move, int jobId)
        {
            throw new NotImplementedException();
        }

        internal static Task UpdateJobCostMilestone(HttpClient httpClient, int id, Move move, int jobId)
        {
            throw new NotImplementedException();
        }

        #endregion Update MileStone

        private static T Convert<T>(string parsedResponse)
        {
            try
            {
                return ((!string.IsNullOrEmpty(parsedResponse)) ? JsonConvert.DeserializeObject<T>(parsedResponse) : default(T));
            }
            catch (Exception ex) { Console.WriteLine("*********ERROR**"); }

            return default(T);
        }

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