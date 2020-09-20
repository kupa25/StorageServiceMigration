using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
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

        public static async Task<string> CallJobsApi(HttpClient _httpClient, string url, dynamic model)
        {
            url = _jobsBaseUrl + url;

            var payload = JsonConvert.SerializeObject(model);

            var response = await _httpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var parsedResponse = await HandleResponse(response);
            return parsedResponse;
        }

        public static async Task<string> HandleResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            return content;
        }

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

        internal static async Task UpdateOriginMilestone(HttpClient httpClient, List<ServiceOrder> serviceOrders, Move move, int jobId)
        {
            var origin = move.MoveAgents.FirstOrDefault(ma => ma.JobCategory.Equals("ORIGIN"));
            var jobsOriginRecord = serviceOrders.FirstOrDefault(so => so.ServiceId == 24);

            var url = $"/{jobId}/services/orders/{jobsOriginRecord.Id}?serviceName=OA";
            var patchDoc = new List<JsonPatchDocument<ServiceOrderMoveInfo>>();

            //All docs received.
            if (origin != null && origin.DOCS_RCV_DATE != null)
            {
            }
        }
    }
}