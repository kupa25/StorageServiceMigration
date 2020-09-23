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

        private static async Task Main(string[] args)
        {
            await SungateApi.setApiAccessTokenAsync(_httpClient);
            await RetrieveJobsAccountAndVendor();

            var moves = await WaterDbAccess.RetrieveWaterRecords();

            foreach (var move in moves)
            {
                try
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

                    await updateStorageJob(move, jobId, serviceOrders);

                    //await JobsApi.UpdateICtMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 27).Id, move, jobId);

                    var paymentSends = await WaterDbAccess.RetrieveJobCostExpense(move.RegNumber);
                    var paymentReceived = await WaterDbAccess.RetrieveJobCostRevenue(move.RegNumber);

                    //await JobsApi.UpdateJobCostMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 29).Id, move, jobId);

                    //Add Notes
                    await AddNotesFromGmmsToArive(move, jobId);

                    //Add Prompts -- Figure out what is system generated or manually entered
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"**** ERROR ****");
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }

        private static async Task updateStorageJob(Move move, int jobId, List<ServiceOrder> serviceOrders)
        {
            Console.WriteLine("Updating ST");

            var vendorAccountingId = move.StorageAgent.VendorNameId;
            var vendorEntity = _vendor.FirstOrDefault(v => v.AccountingId == vendorAccountingId);
            var soId = serviceOrders.FirstOrDefault(so => so.ServiceId == 32).Id;

            await JobsApi.UpdateStorageMilestone(_httpClient, soId, move, jobId, vendorEntity);

            var storageRevId = await JobsApi.AddStorageRevRecord(_httpClient, soId, move, jobId);

            await JobsApi.updateStorageRevRecord(_httpClient, soId, storageRevId, move, jobId);
        }

        private static async Task AddNotesFromGmmsToArive(Move move, int jobId)
        {
            var notesEntity = await WaterDbAccess.RetrieveNotesForMove(move.RegNumber);

            foreach (var note in notesEntity)
            {
                var adObj = await SungateApi.GetADName(_httpClient, NameTranslator.repo.GetValueOrDefault(note.ENTERED_BY));

                if (adObj != null && adObj.Count > 0)
                {
                    note.ENTERED_BY = adObj.FirstOrDefault().email;
                }
                else
                {
                    note.ENTERED_BY = "MigrationScript@test.com";
                }
            }

            var createJobNoteRequests = notesEntity.ToNotesModel();

            await TaskApi.CreateNotes(_httpClient, createJobNoteRequests, jobId);
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
                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(move.MOVE_COORDINATOR.Format(), "Trevor Buracchio");
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

                var adObj = await SungateApi.GetADName(_httpClient, dictionaryValue);

                if (adObj == null || adObj.Count == 0) { continue; }

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
    }
}