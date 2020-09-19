using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class JobsApi
    {
        public static async Task<string> CallJobsApi(HttpClient _httpClient, string url, dynamic model)
        {
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
    }
}