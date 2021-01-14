using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using Suddath.Helix.JobMgmt.Services.Water.Mapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    internal class Program
    {
        private static HttpClient _httpClient = new HttpClient();
        private static List<AccountEntity> _accountEntities;
        private static List<Vendor> _vendor;
        private static bool loadAllRecords = false;
        private static List<string> movesToImport = new List<string>();

        private static async Task Main(string[] args)
        {
            loadAllRecords = true;

            SetConsoleWriteLine();
            SetMovesToImport(loadAllRecords);
            await RetrieveJobsAccountAndVendor();

            Trace.WriteLine($"GMMS REG Number, Arive Job# , Log text ");
            int counter = 0;
            var jobId = 0;

            //Normal Import
            foreach (var regNumber in movesToImport)
            {
                try
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                    Console.WriteLine($"Processing { ++counter} records of {movesToImport.Count} to import");

                    Trace.WriteLine($"{regNumber}, , ");
                    Trace.WriteLine($"{regNumber}, , -----------------------------------------------------------------------------------");

                    await SungateApi.setApiAccessTokenAsync(_httpClient);
                    var move = await WaterDbAccess.RetrieveWaterRecords(regNumber);

                    if (move == null)
                    {
                        continue;
                    }

                    //Add the job
                    jobId = await addStorageJob(move, regNumber);

                    //update datecreated on the job
                    JobsDbAccess.ChangeDateCreated(jobId, move.DateEntered.GetValueOrDefault(DateTime.UtcNow), regNumber);

                    //Add JobContacts
                    await addJobContacts(move, jobId, regNumber);

                    //Add SuperService
                    var result = await JobsApi.CreateStorageSSO(_httpClient, jobId, regNumber);
                    var ssoId = result.Id;
                    //JobsDbAccess.ChangeDisplayName(result.Id, move.RegNumber);

                    var serviceOrders = await JobsDbAccess.GetServiceOrderForJobs(jobId, regNumber);

                    // ORIGIN
                    var oaVendor = _vendor.Find(v => v.Accounting_SI_Code.Equals(move.OriginAgent.VendorNameId));
                    await JobsApi.UpdateOriginMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 24).Id, oaVendor, move, jobId, regNumber);

                    // DESTINATION
                    var daVendor = _vendor.Find(v => v.Accounting_SI_Code.Equals(move.DestinationAgent.VendorNameId));
                    await JobsApi.UpdateDestinationMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 26).Id, daVendor, move, jobId, regNumber);

                    var legacyInsuranceClaims = await WaterDbAccess.RetrieveInsuranceClaims(move.RegNumber);

                    // STORAGE
                    var transfereeEntity = await JobsDbAccess.GetJobsTransfereeId(jobId);
                    await updateStorageJob(move, jobId, serviceOrders, regNumber, transfereeEntity, legacyInsuranceClaims, ssoId);

                    // INSURANCE
                    await JobsApi.UpdateICtMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 27).Id, move, jobId, legacyInsuranceClaims, regNumber);

                    #region JobCost

                    var superServiceOrderId = serviceOrders.FirstOrDefault(so => so.ServiceId == 29).SuperServiceOrderId;

                    try
                    {
                        await JobsDbAccess.LockJC(jobId, regNumber, superServiceOrderId, move.READY_TO_ACCRUE_DATE);
                        await JobsDbAccess.MarkAsPosted(superServiceOrderId, DateTime.Now, true, regNumber, move.ACCRUED_DATE);
                        //await JobsDbAccess.MarkAllAsVoid(superServiceOrderId, regNumber);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while trying to change JC status manually");
                        Trace.WriteLine($"{regNumber}, , Error while trying to change JC status manually");
                        Trace.WriteLine($"{regNumber}, , {ex.Message}");
                    }

                    #endregion JobCost

                    //Add Notes
                    await AddNotesFromGmmsToArive(move, jobId, regNumber);

                    //Add Prompts
                    await AddPromptsFromGmmsToArive(move, jobId, regNumber);

                    decimal percentage = (decimal)(counter * 100) / movesToImport.Count;

                    Console.WriteLine($"{ Math.Round(percentage, 2)}% Completed ");
                    Trace.WriteLine($"{regNumber}, , EndTime: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"{regNumber}, , *** ERROR ***");
                    if (ex.InnerException != null)
                    {
                        Trace.WriteLine($"{regNumber}, , {ex.InnerException.Message}");
                    }
                    else
                    {
                        Trace.WriteLine($"{regNumber}, , {ex.Message}");
                    }

                    Console.WriteLine($"**** ERROR ****");
                    Console.WriteLine($"{ex.Message}");
                }

                Trace.Flush();
            }

            //Remove Prompts from MigrationScript
            TaskDbAccess.RemovePrompts();
        }

        private static async Task updateStorageJob(Move move, int jobId, List<ServiceOrder> serviceOrders, string regNumber, Transferee transferee, List<InsuranceClaims> legacyInsuranceClaims, int ssoId)
        {
            try
            {
                Console.WriteLine("Updating ST");
                Trace.WriteLine($"{regNumber}, , Updating ST Expense record");

                var vendorEntity = _vendor.FirstOrDefault(v => v.Accounting_SI_Code == move.StorageAgent.VendorNameId);
                var soId = serviceOrders.FirstOrDefault(so => so.ServiceId == 32 && so.SuperServiceOrderId == ssoId).Id;

                await JobsApi.UpdateStorageMilestone(_httpClient, soId, move, jobId, vendorEntity, regNumber, legacyInsuranceClaims);

                var storageRevId = await JobsApi.AddStorageRevRecord(_httpClient, soId, move, jobId, regNumber);

                dynamic billTo = null;
                var billToLabel = string.Empty;
                billTo = _accountEntities.FirstOrDefault(ae => ae.AccountingId.Equals(move.StorageAgent.HOW_SENT));

                if (billTo != null)
                {
                    billToLabel = "Account";
                }
                else
                {
                    billTo = _vendor.FirstOrDefault(ae => ae.Accounting_SI_Code.Equals(move.StorageAgent.HOW_SENT));

                    if (billTo != null)
                    {
                        billToLabel = "Vendor";
                    }
                }
                if (billTo == null)
                {
                    //check to see if billto is transferee
                    var NamesRecord = await WaterDbAccess.GetNames(move.StorageAgent.HOW_SENT);
                    if (NamesRecord != null && NamesRecord.FirstName.Equals(transferee.FirstName, StringComparison.CurrentCultureIgnoreCase)
                        && NamesRecord.LastName.Equals(transferee.LastName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        billTo = transferee;
                        billToLabel = "Transferee";
                    }
                    else
                    {
                        Console.WriteLine($"{regNumber}, , Cant find the billto for Storage so we are defaulting it");
                        Trace.WriteLine($"{regNumber}, , Cant find the billto for Storage so we are defaulting it");
                    }
                }

                if (!string.IsNullOrEmpty(move.StorageAgent.HOW_SENT) && billTo == null)
                {
                    Trace.WriteLine($"{regNumber}, , Missing Storage BillTo {move.StorageAgent.HOW_SENT}");
                }

                await JobsApi.updateStorageRevRecord(_httpClient, soId, storageRevId, move, jobId, regNumber, billTo, billToLabel, legacyInsuranceClaims);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating ST");
                Trace.WriteLine($"{regNumber}, , Error while updating ST");
                Trace.WriteLine($"{regNumber}, , {ex.Message}");
            }
        }

        private static async Task AddPromptsFromGmmsToArive(Move move, int jobId, string regNumber)
        {
            var legacyPromptEntity = await WaterDbAccess.RetrievePrompts(move.RegNumber);
            if (legacyPromptEntity == null || legacyPromptEntity.Count == 0)
            {
                Trace.WriteLine($"{regNumber}, , No Available prompts found in GMMS");
                return;
            }

            foreach (var prompt in legacyPromptEntity)
            {
                var adObj = await SungateApi.GetADName(_httpClient, NameTranslator.repo.GetValueOrDefault(prompt.ENTERED_BY), regNumber);

                if (adObj != null && adObj.Count > 0)
                {
                    prompt.ENTERED_BY = adObj.FirstOrDefault().email;
                }
                else
                {
                    Console.WriteLine($"{regNumber}, Can't get Prompt created User So defaulting it to MigrationScript@test.com");
                    Trace.WriteLine($"{regNumber}, , Can't get Prompt created User So defaulting it to MigrationScript@test.com");
                    prompt.ENTERED_BY = "MigrationScript@test.com";
                }

                prompt.JobId = jobId;
            }

            var workflowTasks = legacyPromptEntity.ToWorkFlowTask();

            await TaskDbAccess.AddPrompts(workflowTasks, regNumber, jobId);
        }

        private static async Task AddNotesFromGmmsToArive(Move move, int jobId, string regNumber)
        {
            var notesEntity = await WaterDbAccess.RetrieveNotesForMove(move.RegNumber);

            if (notesEntity == null)
            {
                Trace.WriteLine($"{regNumber}, , No Available notes found in GMMS");
                return;
            }

            foreach (var note in notesEntity)
            {
                var adObj = await SungateApi.GetADName(_httpClient, NameTranslator.repo.GetValueOrDefault(note.ENTERED_BY), regNumber);

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

            foreach (var noteobj in createJobNoteRequests)
            {
                if (!noteobj.Category.Equals("serviceboard"))
                {
                    noteobj.DisplayId = regNumber;
                }

                noteobj.ReferenceId = jobId;
            }

            await TaskDbAccess.AddNotes(createJobNoteRequests, jobId, regNumber);

            //await TaskApi.CreateNotes(_httpClient, createJobNoteRequests, jobId, regNumber);
        }

        private static async Task addJobContacts(Move move, int jobId, string regNumber)
        {
            var jobContactList = new List<CreateJobContactDto>();

            for (int i = 0; i < 5; i++)
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
                        var nameToUse = string.Empty;

                        if (!string.IsNullOrEmpty(move.MOVE_MANAGER) && !move.MOVE_MANAGER.Equals("STORAGE"))
                        {
                            nameToUse = move.MOVE_MANAGER.Format();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(move.MOVE_COORDINATOR) && !move.MOVE_COORDINATOR.Equals("STORAGE"))
                            {
                                nameToUse = move.MOVE_COORDINATOR.Format();
                            }
                            else
                            {
                                Console.WriteLine("Defaulting MoveConsultant to Angela La Fronza due to bad data");
                                Trace.WriteLine($"{regNumber}, , Defaulting MoveConsultant to Angela La Fronza due to bad data");
                                nameToUse = "Angela.Lafronza";
                            }
                        }

                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(nameToUse);

                        if (string.IsNullOrEmpty(dictionaryValue))
                        {
                            Trace.WriteLine($"{regNumber}, , Move Consultant from GMMS {nameToUse} couldn't be found in Arive thus Defaulting to Angela La Fronza");
                            dictionaryValue = NameTranslator.repo.GetValueOrDefault("Angela.Lafronza");
                        }
                        break;

                    case 2:
                        contactType = "Traffic Consultant";
                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(move.TRAFFIC_MANAGER.Format());
                        break;

                    case 3:
                        contactType = "Pricing Consultant";
                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(move.QUOTED_BY.Format());
                        break;

                    case 4:
                        contactType = "Salesperson Contact";
                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(move.SALES.Format());
                        break;
                }

                if (!string.IsNullOrEmpty(dictionaryValue))
                {
                    var adObj = await SungateApi.GetADName(_httpClient, dictionaryValue, regNumber);

                    if ((adObj == null || adObj.Count == 0) && contactType.Equals("Move Consultant"))
                    {
                        Console.WriteLine("User not found in sungate");
                        Trace.WriteLine($"{regNumber}, , user not found in sungate");

                        dictionaryValue = NameTranslator.repo.GetValueOrDefault("Angela.Lafronza");

                        adObj = await SungateApi.GetADName(_httpClient, dictionaryValue, regNumber);
                    }

                    if (adObj == null || adObj.Count == 0)
                    {
                        Console.WriteLine("User not found in sungate");
                        Trace.WriteLine($"{regNumber}, , user not found in sungate");

                        continue;
                    }

                    jobContactList.Add(new CreateJobContactDto
                    {
                        ContactType = contactType,
                        Email = adObj.FirstOrDefault().email,
                        FullName = adObj.FirstOrDefault().fullName,
                        Phone = adObj.FirstOrDefault().phone
                    });
                }
            }

            Console.WriteLine("Adding Job Contacts");
            Trace.WriteLine($"{regNumber}, , Adding Job Contacts");

            var url = $"/{jobId}/contacts";

            await JobsApi.CallJobsApi(_httpClient, url, jobContactList);
        }

        private static async Task<int> addStorageJob(Move move, string regNumber)
        {
            Console.WriteLine("Creating a job in Arive");
            Trace.WriteLine($"{regNumber}, , Creating a job in Arive");

            var url = string.Empty;

            var movesAccount = _accountEntities.FirstOrDefault(ae => ae.AccountingId.Equals(move.AccountId));
            var movesBooker = _vendor.FirstOrDefault(ae => ae.Accounting_SI_Code.Equals(move.Booker));

            if (movesAccount == null)
            {
                if (move.AccountId.Equals("1674") ||
                    move.AccountId.Equals("279029") ||
                    move.AccountId.Equals("273923") ||
                    move.AccountId.Equals("370377") ||
                    move.AccountId.Equals("OM1119")) //Shipper Direct
                {
                    Trace.WriteLine($"{regNumber}, , Missing Account in Arive {move.AccountId}, thus Defaulting Shipper Direct");
                    movesAccount = _accountEntities.FirstOrDefault(ae => ae.Id == 283);
                }
                else if (move.AccountId.Equals("181359"))
                {
                    Trace.WriteLine($"{regNumber}, , Missing Account in Arive {move.AccountId}, thus Defaulting Overseas Agent");
                    movesAccount = _accountEntities.FirstOrDefault(ae => ae.Name == "OVERSEAS AGENT BOOKING");
                }
                else
                {
                    throw new Exception($"Missing Account in Arive {move.AccountId}");
                }
            }

            var response = await DetermineBillTo(move.BILL, null, regNumber);

            if (response.BilltoId == null)
            {
                Trace.WriteLine($"{regNumber}, , Missing BillTo in Arive {move.BILL}");
                Trace.WriteLine($"{regNumber}, , Defaulting BillTo as Shipper Direct");
                response.BilltoId = 283;
                response.BilltoType = "Account";
            }

            var model = move.ToJobModel(movesAccount.Id, movesBooker?.Id, response.BilltoId, response.BilltoType);
            model.Job.ExternalReference = regNumber;
            model.Job.ExternalReferenceDescription = "RegNumber from GMMS : Storage Migration";

            string parsedResponse = await JobsApi.CallJobsApi(_httpClient, url, model);

            Console.WriteLine($"Job created {parsedResponse}");
            Trace.WriteLine($"{regNumber},{parsedResponse} , Job created");
            return int.Parse(parsedResponse);
        }

        #region Helpers

        private static async Task RetrieveJobsAccountAndVendor()
        {
            Console.WriteLine("Retrieving Existing Accounts and Vendors from Jobs");

            try
            {
                using (var context = new JobDbContext(JobsDbAccess.connectionString))
                {
                    _accountEntities = await context.AccountEntity.AsNoTracking().ToListAsync();
                    _vendor = await context.Vendor.AsNoTracking().Where(v => !string.IsNullOrEmpty(v.Accounting_SI_Code)).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Trace.WriteLine(ex);
            }
        }

        private static async Task<BillToResponse> DetermineBillTo(string id, Transferee transferee, string regNumber)
        {
            dynamic billTo = null;
            string billToLabel = null;
            billTo = _accountEntities.FirstOrDefault(ae => ae.AccountingId.Equals(id));

            if (billTo != null)
            {
                billToLabel = "ACCOUNT";
            }
            else
            {
                billTo = _vendor.FirstOrDefault(v => v.Accounting_SI_Code.Equals(id));

                if (billTo != null)
                {
                    billToLabel = "VENDOR";
                }
            }
            if (billTo == null && transferee != null)
            {
                //check to see if billto is transferee
                var NamesRecord = await WaterDbAccess.GetNames(id);
                if (NamesRecord != null && NamesRecord.FirstName.Equals(transferee.FirstName, StringComparison.CurrentCultureIgnoreCase)
                    && NamesRecord.LastName.Equals(transferee.LastName, StringComparison.CurrentCultureIgnoreCase))
                {
                    billTo = transferee;
                    billToLabel = "TRANSFEREE";
                }
                else
                {
                    Console.WriteLine($"{regNumber}, Cant find the billto for Storage");
                    Trace.Write($"{regNumber}, Cant find the billto for Storage");
                }
            }

            var response = new BillToResponse
            {
                BilltoId = billTo?.Id,
                BilltoType = billToLabel
            };

            return response;
        }

        private static void SetMovesToImport(bool loadAllRecords)
        {
            if (!loadAllRecords)
            {
                //movesToImport.Add("274527"); // GOOD one to import according to heather
                movesToImport.Add("283071");
                //movesToImport.Add("284497");
            }
            else
            {
                var array = new String[] {
"140675",
"218364",
"225365",
"283071",
"133965",
"282954",
"224051",
"215297",
"283230",
"126836",
"130902",
"226248",
"151325",
"217986",
"226406",
"106417",
"127017",
"126658",
"217742",
"217185",
"283778",
"219470",
"121356",
"283416",
"221731",
"283858",
"284032",
"222775",
"216320",
"221856",
"217420",
"152245",
"154280",
"157923",
"155129",
"188984",
"190132",
"185977",
"174697",
"164504",
"160010",
"228028",
"228764",
"228882",
"180509",
"175353",
"167096",
"168611",
"177872",
"179413",
"180158",
"191731",
"193038",
"196270",
"197263",
"199186",
"188511",
"239921",
"238072",
"240538",
"236709",
"201219",
"200069",
"237351",
"237039",
"237134",
"238375",
"237988",
"235378",
"237017",
"238272",
"209366",
"211489",
"210120",
"202334",
"202681",
"236545",
"235336",
"236852",
"234933",
"234101",
"233827",
"233939",
"272169",
"204789",
"233000",
"233233",
"212713",
"210494",
"206792",
"206626",
"275240",
"207812",
"274394",
"254186",
"253987",
"258328",
"257580",
"257523",
"257590",
"257885",
"257435",
"256737",
"250208",
"247656",
"188393",
"248296",
"241270",
"242041",
"242219",
"263099",
"263221",
"262617",
"262646",
"262893",
"261718",
"262220",
"252823",
"253497",
"254636",
"252117",
"252880",
"253117",
"242434",
"240900",
"241077",
"261470",
"260967",
"261684",
"261676",
"261963",
"262089",
"260402",
"260991",
"261004",
"261144",
"260280",
"260168",
"249104",
"250332",
"249036",
"251169",
"155004",
"250323",
"243625",
"244369",
"206146",
"243579",
"260566",
"259014",
"259036",
"257492",
"246157",
"244915",
"255532",
"251741",
"252702",
"252607",
"251031",
"246948",
"246067",
"246572",
"269232",
"270697",
"270702",
"267477",
"267482",
"269356",
"275038",
"270155",
"274580",
"268987",
"276587",
"280431",
"280155",
"281539",
"281020",
"267349",
"271295",
"273148",
"276299",
"274482",
"266133",
"274681",
"276903",
"270050",
"270051",
"276198",
"284318",
"269350",
"268870",
"271897",
"284574",
"276561",
"276584",
"264871",
"268741",
"267116",
"279602",
"279615",
"277144",
"276461",
"280800",
"267176",
"269733",
"280135",
"281768",
"263235",
"263512",
"282610",
"264088",
"283602",
"283880",
"270682",
"270688",
"266432",
"270228",
"280202",
"280839",
"275706",
"281584",
"282039",
"282435",
"283180",
"273517",
"282745",
"272500",
"274364",
"283244",
"267412",
"269095",
"281130",
"270399",
"271531",
"270555",
"272740",
"271640",
"283338",
"271831",
"265423",
"275681",
"281211",
"282058",
"276080",
"277519",
"271800",
"280263",
"284216",
"284821",
"275210",
"266260",
"272420",
"278308",
"279288",
"279635",
"280510",
"272124",
"281598",
"274550",
"267796",
"268750",
"284280",
"270059",
"270060",
"270062",
"270064",
"270067",
"270069",
"270070",
"270072",
"266125",
"270073",
"270075",
"270079",
"270082",
"270083",
"270085",
"270086",
"270087",
"277890",
"284201",
"266716",
"266718",
"266728",
"266735",
"266743",
"274778",
"264330",
"267549",
"269744",
"269875",
"270007",
"271460",
"271468",
"265500",
"284470",
"279127",
"273081",
"273086",
"282702",
"273473",
"279971",
"279309",
"268493",
"269537",
"270061",
"270068",
"270074",
"270076",
"270098",
"268674",
"270103",
"270104",
"274736",
"270099",
"283601",
"266342",
"275029",
"282912",
"267764",
"279557",
"282017",
"282486",
"279196",
"279197",
"282896",
"280037",
"281091",
"274486",
"274045",
"284127",
"282459",
"266045",
"282642",
"270747",
"269753",
"277862",
"271054",
"280284",
"266945",
"268936",
"274985",
"276088",
"276716",
"283993",
"277809",
"269913",
"269922",
"278380",
"279458",
"279473",
"270460",
"270511",
"267158",
"272992",
"284207",
"277011",
"268835",
"269374",
"267925",
"282390",
"270148",
"270927",
"279271",
"279403",
"268436",
"270618",
"265958",
"276683",
"276704",
"277151",
"274527",
"266836",
"271200",
"284497",
"270505",
"263240"
 };

                movesToImport = array.ToList<string>();
            }
        }

        private static void SetConsoleWriteLine()
        {
            Trace.Listeners.Add(new TextWriterTraceListener($"Migration{Guid.NewGuid()}.csv"));
            Trace.WriteLine(""); // Insert a blank line
            Trace.AutoFlush = true;
        }

        #endregion Helpers
    }
}