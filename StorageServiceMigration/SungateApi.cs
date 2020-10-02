using Helix.API.Results;
using IdentityModel.Client;
using Newtonsoft.Json;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class SungateApi
    {
        private static string _sugGateBaseUrl = "https://daue2sungtv2wb02.azurewebsites.net";

        private static Dictionary<string, List<ADUser>> cachedAdUser { get; set; } = new Dictionary<string, List<ADUser>>();

        public static async Task<HttpClient> setApiAccessTokenAsync(HttpClient _httpClient)
        {
            Console.WriteLine("Getting the Sungate Token");
            //Trace.WriteLine("Getting the Sungate Token");

            var response = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"{_sugGateBaseUrl}/connect/token",
                ClientId = "utility.ccrf",
                GrantType = "client_credentials",
                ClientSecret = "E5NDJlMDQzM2QwNjFiNTBlN2ZkZjA0YTgzYTc1ZGYiLCJzY29wZSI6WyJhZGd4",
                Scope = "jobsapi taskapi adapi "
            });
            if (response.IsError) throw new Exception(response.Error);

            var token = response.AccessToken;
            _httpClient.SetBearerToken(token);

            return _httpClient;
        }

        public static async Task<List<ADUser>> GetADName(HttpClient _httpClient, string v, string regNumber)
        {
            v = v.Format();

            List<ADUser> adUser;
            var found = cachedAdUser.TryGetValue(v, out adUser);

            if (!found && !string.IsNullOrEmpty(v))
            {
                Console.WriteLine("Get the Ad Name for : " + v);
                Trace.WriteLine($"{regNumber}, Get the Ad Name for : " + v);

                var url = _sugGateBaseUrl + $"/api/v1/aad/lookup/{v}";
                var response = await _httpClient.GetAsync(url);
                var parsedResponse = await HandleResponse(response);
                List<ADUser> payload = null;

                try
                {
                    payload = ((!string.IsNullOrEmpty(parsedResponse)) ? JsonConvert.DeserializeObject<SingleResult<List<ADUser>>>(parsedResponse) : null).Data;
                }
                catch (Exception ex) { }

                cachedAdUser.Add(v, payload);

                return payload;
            }

            return adUser;
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