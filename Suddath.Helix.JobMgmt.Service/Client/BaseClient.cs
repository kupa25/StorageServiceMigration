using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SunGatePolicy.Client.Config;

namespace Suddath.Helix.JobMgmt.Services.Client
{
    public abstract class BaseClient
    {

        private readonly string _baseUrl;
        private readonly RestClient _restClient;
        private readonly PolicyOptions _policyOptions;


        private string _authToken;

        public BaseClient(IOptions<PolicyOptions> policyOptions, string baseUrl)
        {
            _baseUrl = baseUrl;
            _restClient = new RestClient(_baseUrl);
            _policyOptions = policyOptions.Value;
        }

        public class AuthResponse
        {
            public string access_token { get; set; }
        }

        public abstract string Scope { get; }

        private async Task<bool> GetAuthenticationToken(string scope)
        {
            RestRequest req = new RestRequest();
            req.Resource = _policyOptions.TokenUrl;
            req.Method = Method.POST;
            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            req.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
            req.AddParameter("client_id", _policyOptions.ClientId, ParameterType.GetOrPost);
            req.AddParameter("client_secret", _policyOptions.Secret, ParameterType.GetOrPost);
            req.AddParameter("scope", scope, ParameterType.GetOrPost);

            var client = new RestClient();
            var resp = await client.ExecuteAsync(req);
            if (!resp.IsSuccessful)
                return false;

            var authResp = JsonConvert.DeserializeObject<AuthResponse>(resp.Content);
            _authToken = authResp.access_token;
            return true;
        }


        public async Task<T> ExecuteAsync<T>(IRestRequest request)
        {
            //Check for auth token
            if (string.IsNullOrEmpty(_authToken))
            {
                var success = await GetAuthenticationToken(Scope);
                if (!success)
                    throw new Exception("Could not obtain authentication token!");
            }

            //Add Auth token to header
            request.AddHeader("Authorization", "bearer " + _authToken);

            //Execute request
            var resp = await _restClient.ExecuteAsync(request);

            if (!resp.IsSuccessful)
                throw new Exception(resp.ErrorMessage, resp.ErrorException);

            //Get result object
            return JsonConvert.DeserializeObject<T>(resp.Content);
        }

    }
}
