using Helix.API.Results;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using Suddath.Helix.JobMgmt.Services.Water.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    internal class Program
    {
        private static HttpClient _httpClient = new HttpClient();
        private static List<AccountEntity> _accountEntities;
        private static List<Vendor> _vendor;

        private static string _sugGateBaseUrl = "https://daue2sungtv2wb02.azurewebsites.net";

        private static async Task Main(string[] args)
        {
            await setApiAccessTokenAsync();
            await RetrieveJobsAccountAndVendor();

            var moves = await WaterDbAccess.RetrieveWaterRecords();

            foreach (var move in moves)
            {
                //Add the job
                var jobId = await addStorageJob(move);

                //update datecreated on the job
                JobsDbAccess.ChangeDateCreated(jobId, move.DateEntered.GetValueOrDefault(DateTime.UtcNow));

                //Add JobContacts
                await addJobContacts(move, jobId);

                //Add SuperService
                await JobsApi.CreateStorageSSO(_httpClient, jobId);

                //Update Milestone Pages
                var serviceOrders = await JobsDbAccess.GetServiceOrderForJobs(jobId);

                await JobsApi.UpdateOriginMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 24).Id, move, jobId);

                await JobsApi.UpdateDestinationMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 26).Id, move, jobId);

                //await JobsApi.UpdateStorageMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 32).Id, move, jobId);

                //await JobsApi.UpdateJobCostMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 27).Id, move, jobId);

                //await JobsApi.UpdateJobCostMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 29).Id, move, jobId);

                //Add Notes
                var createJobNoteRequests = await WaterDbAccess.RetrieveNotesForMove(move.RegNumber);
                await TaskApi.CreateNotes(_httpClient, createJobNoteRequests, jobId);

                //Add Prompts -- Figure out what is system generated or manually entered
            }
        }

        private static async Task addJobContacts(Move move, int jobId)
        {
            var jobContactList = new List<CreateJobContactDto>();

            for (int i = 0; i < 4; i++) // we are only mapping 5 people.. since thats in the datamap
            {
                var dictionaryValue = string.Empty;
                var contactType = string.Empty;

                switch (i)
                {
                    case 0:
                        contactType = "Biller Contact";
                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(move.BILLER.Format());
                        break;

                    case 1:
                        contactType = "Move Consultant";
                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(move.MOVE_COORDINATOR.Format(), "TBuracchio@suddath.com");
                        break;

                    case 2:
                        contactType = "Traffic Consultant";
                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(move.TRAFFIC_MANAGER.Format());
                        break;

                    case 3:
                        contactType = "Pricing Consultant";
                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(move.QUOTED_BY.Format());
                        break;
                }

                var adObj = await GetADName(dictionaryValue);

                if (adObj == null) { continue; }

                jobContactList.Add(new CreateJobContactDto
                {
                    ContactType = contactType,
                    Email = adObj.FirstOrDefault().email,
                    FullName = adObj.FirstOrDefault().fullName,
                    Phone = adObj.FirstOrDefault().phone
                });
            }

            Console.WriteLine("Adding Job Contacts");

            var url = $"/{jobId}/contacts";

            await JobsApi.CallJobsApi(_httpClient, url, jobContactList);
        }

        private static async Task<List<ADUser>> GetADName(string v)
        {
            Console.WriteLine("Get the Ad Name for : " + v);

            var url = _sugGateBaseUrl + $"/api/v1/aad/lookup/{v}";
            var response = await _httpClient.GetAsync(url);
            var parsedResponse = await HandleResponse(response);
            List<ADUser> payload = null;

            try
            {
                payload = ((!string.IsNullOrEmpty(parsedResponse)) ? JsonConvert.DeserializeObject<SingleResult<List<ADUser>>>(parsedResponse) : null).Data;
            }
            catch (Exception ex) { }

            return payload;
        }

        private static async Task RetrieveJobsAccountAndVendor()
        {
            Console.WriteLine("Retrieving Existing Accounts and Vendors from Jobs");
            try
            {
                using (var context = new JobDbContext(JobsDbAccess.connectionString))
                {
                    _accountEntities = await context.AccountEntity.AsNoTracking().ToListAsync();
                    _vendor = await context.Vendor.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task<int> addStorageJob(Move move)
        {
            Console.WriteLine("Creating a job");

            var url = string.Empty;

            var movesAccount = _accountEntities.FirstOrDefault(ae => ae.AccountingId.Equals(move.AccountId));
            var movesBooker = _vendor.FirstOrDefault(ae => ae.AccountingId.Equals(move.Booker));

            dynamic billTo = null;
            var billToLabel = string.Empty;
            billTo = _accountEntities.FirstOrDefault(ae => ae.AccountingId.Equals(move.BILL));

            if (billTo != null)
            {
                billToLabel = "Account";
            }
            else
            {
                billTo = _vendor.FirstOrDefault(ae => ae.AccountingId.Equals(move.BILL));

                if (billTo != null)
                {
                    billToLabel = "Vendor";
                }
            }

            var model = move.ToJobModel(movesAccount.Id, movesBooker.Id, (int?)billTo?.Id, billToLabel);
            string parsedResponse = await JobsApi.CallJobsApi(_httpClient, url, model);

            Console.WriteLine($"Job added {parsedResponse}");
            return int.Parse(parsedResponse);
        }

        private static async Task setApiAccessTokenAsync()
        {
            Console.WriteLine("Getting the Sungate Token");

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