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

        //private static string _jobsBaseUrl = "https://daue2helixjobwa01.azurewebsites.net";
        private static string _jobsBaseUrl = "https://localhost:5001";

        private static async Task Main(string[] args)
        {
            await setApiAccessTokenAsync();
            await RetrieveJobsAccountAndVendor();

            var moves = await RetrieveWaterRecords();

            foreach (var move in moves)
            {
                //Add the job
                var jobId = await addStorageJob(move);

                //Add JobContacts
                await addJobContacts(move);

                /// api / v{ version}/ Jobs /{ jobId}/ contacts

                //Add SuperService
                /// api / v{ version}/ jobs /{ jobId}/ superServices / Order

                //Each Milestone page we patch the info
                /// api / v{ version}/ jobs /{ jobId}/ services / orders /{ serviceOrderId}

                //            Add Notes
                //--TaskMgmt-- -
                //​/ api​ / v{ version}​/ Notes

                //Add Tasks
                //--TaskMgmt--
                /// api / v{ version}/ Tasks / jobs /{ jobId}
                ///
            }
        }

        private static async Task addJobContacts(Move move)
        {
            //Get BIller Name
            var adObj = await GetADName(NameTranslator.repo.GetValueOrDefault(move.BILLER.Format()));

            var jobContact = new CreateJobContactDto
            {
                ContactType = "Move Consultant",
                Email = adObj.FirstOrDefault().email,
                FullName = adObj.FirstOrDefault().fullName,
                Phone = adObj.FirstOrDefault().phone
            };

            adObj = await GetADName(NameTranslator.repo.GetValueOrDefault(move.MOVE_COORDINATOR.Format()));
        }

        private static async Task<List<ADUser>> GetADName(string v)
        {
            Console.WriteLine("Get the Ad Name for : " + v);

            var url = _sugGateBaseUrl + $"/api/v1/aad/lookup/{v}";
            var response = await _httpClient.GetAsync(url);
            var parsedResponse = await HandleResponse(response);

            var payload = ((!string.IsNullOrEmpty(parsedResponse)) ? JsonConvert.DeserializeObject<SingleResult<List<ADUser>>>(parsedResponse) : null).Data;

            Console.WriteLine($"Ad retreived {parsedResponse}");

            return payload;
        }

        private static async Task RetrieveJobsAccountAndVendor()
        {
            Console.WriteLine("Retrieving Existing Accounts and Vendors from Jobs");
            try
            {
                using (var context = new JobDbContext())
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

            var url = _jobsBaseUrl + "/api/v1/Jobs";

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
            var payload = JsonConvert.SerializeObject(model);

            var response = await _httpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var parsedResponse = await HandleResponse(response);

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

        private static async Task<List<Move>> RetrieveWaterRecords()
        {
            Console.WriteLine("Retrieving Legacy moves");
            try
            {
                using (var context = new WaterDbContext())
                {
                    var moves = await context.Moves
                   .Include(v => v.Profile)
                   .Include(v => v.Account)
                   .Include(v => v.Names)
                   .Include(v => v.MoveItems)
                   .Include(v => v.MoveAgents)
                       .ThenInclude(v => v.Name)
                   .AsNoTracking()
                   .Where(m => m.RegNumber == "255950").ToListAsync();

                    return moves;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
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