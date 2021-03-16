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
        private static Dictionary<string, int> transfereeAccountingId = new Dictionary<string, int>();

        private static async Task Main(string[] args)
        {
            //loadAllRecords = true;

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

                    JobsDbAccess.ChangeTransfereeAccountingId(jobId, regNumber, transfereeAccountingId.GetValueOrDefault(regNumber));

                    //Add JobContacts
                    await addJobContacts(move, jobId, regNumber);

                    //Add SuperService
                    var result = await JobsApi.CreateStorageSSO(_httpClient, jobId, regNumber);
                    var ssoId = result.Id;

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
                        if (move.READY_TO_ACCRUE_DATE != null)
                        {
                            await JobsDbAccess.LockJC(jobId, regNumber, superServiceOrderId, move.READY_TO_ACCRUE_DATE.Value);
                            await JobsDbAccess.MarkAsPosted(superServiceOrderId, DateTime.Now, true, regNumber, move.ACCRUED_DATE);
                            //await JobsDbAccess.MarkAllAsVoid(superServiceOrderId, regNumber);
                        }
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
                        Trace.WriteLine($"{regNumber}, ,Sales: GMMS-{move.SALES.Format()} Arive-{dictionaryValue}");
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
                movesToImport.Add("234933");
            }
            else
            {
                var array = new String[]
                {
                    "106417",
"121356",
"126658",
"126836",
"127017",
"133965",
"140675",
"151325",
"152245",
"154280",
"155129",
"157923",
"160010",
"164504",
"167096",
"168611",
"174697",
"175353",
"176484",
"177872",
"179413",
"180158",
"180509",
"185977",
"188393",
"188511",
"188984",
"190132",
"191731",
"193038",
"196270",
"197263",
"199186",
"200069",
"201219",
"202334",
"202681",
"203448",
"204789",
"206146",
"206626",
"206792",
"207812",
"209366",
"210120",
"210494",
"211489",
"212713",
"215297",
"216320",
"217185",
"217420",
"217742",
"217986",
"218364",
"219470",
"221731",
"221856",
"222775",
"224051",
"226248",
"226406",
"228028",
"228764",
"228882",
"233000",
"233233",
"233827",
"233939",
"234101",
"234933",
"235336",
"235378",
"236545",
"236709",
"236852",
"237017",
"237039",
"237134",
"237351",
"237988",
"238072",
"238272",
"238375",
"239921",
"240538",
"240900",
"241077",
"241270",
"242041",
"242219",
"242434",
"243579",
"243625",
"244369",
"244915",
"246067",
"246157",
"246572",
"246948",
"247656",
"248296",
"249036",
"249104",
"249998",
"250208",
"250323",
"250332",
"251031",
"251169",
"251741",
"252117",
"252607",
"252702",
"252823",
"252880",
"253117",
"253497",
"253987",
"254186",
"254636",
"255532",
"256737",
"257435",
"257492",
"257523",
"257580",
"257590",
"257885",
"258328",
"259014",
"259036",
"260168",
"260280",
"260402",
"260566",
"260967",
"260991",
"261004",
"261144",
"261470",
"261676",
"261684",
"261718",
"261792",
"261963",
"262089",
"262220",
"262617",
"262646",
"262893",
"263099",
"263221",
"263235",
"263240",
"263512",
"264330",
"264871",
"265423",
"265500",
"265958",
"266013",
"266045",
"266125",
"266133",
"266260",
"266342",
"266432",
"266716",
"266718",
"266728",
"266735",
"266743",
"266945",
"267116",
"267176",
"267222",
"267349",
"267412",
"267477",
"267482",
"267497",
"267796",
"267839",
"267925",
"268436",
"268493",
"268674",
"268741",
"268750",
"268835",
"268860",
"268870",
"268987",
"269095",
"269232",
"269356",
"269374",
"269537",
"269733",
"269744",
"269753",
"269875",
"269913",
"269922",
"270007",
"270050",
"270051",
"270059",
"270060",
"270061",
"270062",
"270064",
"270067",
"270068",
"270069",
"270070",
"270072",
"270073",
"270074",
"270075",
"270076",
"270082",
"270083",
"270085",
"270086",
"270087",
"270098",
"270099",
"270103",
"270104",
"270148",
"270155",
"270228",
"270399",
"270460",
"270505",
"270511",
"270555",
"270618",
"270682",
"270688",
"270697",
"270702",
"270747",
"270927",
"271054",
"271200",
"271267",
"271295",
"271460",
"271468",
"271531",
"271640",
"271800",
"271831",
"271897",
"272124",
"272169",
"272420",
"272500",
"272740",
"272992",
"273081",
"273086",
"273148",
"273473",
"273517",
"274045",
"274364",
"274394",
"274482",
"274486",
"274527",
"274550",
"274580",
"274681",
"274736",
"274985",
"275029",
"275038",
"275210",
"275240",
"275706",
"276080",
"276088",
"276198",
"276299",
"276461",
"276561",
"276584",
"276587",
"276683",
"276704",
"276716",
"276903",
"277011",
"277066",
"277144",
"277151",
"277519",
"277539",
"277809",
"277862",
"277890",
"278308",
"278380",
"279127",
"279196",
"279197",
"279271",
"279288",
"279309",
"279403",
"279458",
"279473",
"279602",
"279615",
"279635",
"279971",
"280037",
"280135",
"280155",
"280202",
"280263",
"280284",
"280431",
"280510",
"280800",
"280839",
"281020",
"281091",
"281130",
"281211",
"281539",
"281584",
"281598",
"281768",
"282017",
"282039",
"282051",
"282058",
"282304",
"282390",
"282435",
"282459",
"282486",
"282642",
"282702",
"282745",
"282896",
"282912",
"282954",
"283071",
"283230",
"283244",
"283338",
"283416",
"283601",
"283602",
"283688",
"283778",
"283858",
"283860",
"283880",
"283993",
"284032",
"284127",
"284201",
"284207",
"284280",
"284470",
"284497",
"284536",
"284574",
"284780",
"284781",
"284782",
"284784",
"284821"
                };

                movesToImport = array.ToList<string>();
            }

            //KEY: regnumber
            //Value: Transferee accountingID
            transfereeAccountingId.Add("106417", 112761);
            transfereeAccountingId.Add("121356", 142109);
            transfereeAccountingId.Add("126658", 152648);
            transfereeAccountingId.Add("126836", 153003);
            transfereeAccountingId.Add("127017", 164505);
            transfereeAccountingId.Add("133965", 166584);
            transfereeAccountingId.Add("140675", 178105);
            transfereeAccountingId.Add("151325", 191507);
            transfereeAccountingId.Add("152245", 192761);
            transfereeAccountingId.Add("154280", 195542);
            transfereeAccountingId.Add("155129", 196724);
            transfereeAccountingId.Add("157923", 200345);
            transfereeAccountingId.Add("160010", 203027);
            transfereeAccountingId.Add("164504", 208798);
            transfereeAccountingId.Add("167096", 212308);
            transfereeAccountingId.Add("168611", 214294);
            transfereeAccountingId.Add("174697", 222730);
            transfereeAccountingId.Add("175353", 223746);
            transfereeAccountingId.Add("176484", 225398);
            transfereeAccountingId.Add("177872", 227232);
            transfereeAccountingId.Add("179413", 229227);
            transfereeAccountingId.Add("180158", 230248);
            transfereeAccountingId.Add("180509", 230626);
            transfereeAccountingId.Add("185977", 238024);
            transfereeAccountingId.Add("188393", 241355);
            transfereeAccountingId.Add("188511", 241493);
            transfereeAccountingId.Add("188984", 242033);
            transfereeAccountingId.Add("190132", 243557);
            transfereeAccountingId.Add("191731", 245728);
            transfereeAccountingId.Add("193038", 247526);
            transfereeAccountingId.Add("196270", 251751);
            transfereeAccountingId.Add("197263", 252733);
            transfereeAccountingId.Add("199186", 255523);
            transfereeAccountingId.Add("200069", 256854);
            transfereeAccountingId.Add("201219", 258776);
            transfereeAccountingId.Add("202334", 260656);
            transfereeAccountingId.Add("202681", 261274);
            transfereeAccountingId.Add("203448", 262417);
            transfereeAccountingId.Add("204789", 264406);
            transfereeAccountingId.Add("206146", 266091);
            transfereeAccountingId.Add("206626", 266744);
            transfereeAccountingId.Add("206792", 267041);
            transfereeAccountingId.Add("207812", 268445);
            transfereeAccountingId.Add("209366", 270785);
            transfereeAccountingId.Add("210120", 271914);
            transfereeAccountingId.Add("210494", 272432);
            transfereeAccountingId.Add("211489", 273912);
            transfereeAccountingId.Add("212713", 275770);
            transfereeAccountingId.Add("215297", 279029);
            transfereeAccountingId.Add("216320", 282049);
            transfereeAccountingId.Add("217185", 283480);
            transfereeAccountingId.Add("217420", 283887);
            transfereeAccountingId.Add("217742", 284419);
            transfereeAccountingId.Add("217986", 284833);
            transfereeAccountingId.Add("218364", 285423);
            transfereeAccountingId.Add("219470", 287291);
            transfereeAccountingId.Add("221731", 291209);
            transfereeAccountingId.Add("221856", 291410);
            transfereeAccountingId.Add("222775", 292962);
            transfereeAccountingId.Add("224051", 295182);
            transfereeAccountingId.Add("226248", 299044);
            transfereeAccountingId.Add("226406", 299299);
            transfereeAccountingId.Add("228028", 302154);
            transfereeAccountingId.Add("228764", 303409);
            transfereeAccountingId.Add("228882", 303619);
            transfereeAccountingId.Add("233000", 310774);
            transfereeAccountingId.Add("233233", 311191);
            transfereeAccountingId.Add("233827", 312186);
            transfereeAccountingId.Add("233939", 312386);
            transfereeAccountingId.Add("234101", 312669);
            transfereeAccountingId.Add("234933", 314095);
            transfereeAccountingId.Add("235336", 314691);
            transfereeAccountingId.Add("235378", 314882);
            transfereeAccountingId.Add("236545", 316894);
            transfereeAccountingId.Add("236709", 317128);
            transfereeAccountingId.Add("236852", 317392);
            transfereeAccountingId.Add("237017", 317669);
            transfereeAccountingId.Add("237039", 317704);
            transfereeAccountingId.Add("237134", 317818);
            transfereeAccountingId.Add("237351", 310757);
            transfereeAccountingId.Add("237988", 318868);
            transfereeAccountingId.Add("238072", 318888);
            transfereeAccountingId.Add("238272", 318886);
            transfereeAccountingId.Add("238375", 314364);
            transfereeAccountingId.Add("239921", 321973);
            transfereeAccountingId.Add("240538", 322963);
            transfereeAccountingId.Add("240900", 323560);
            transfereeAccountingId.Add("241077", 323849);
            transfereeAccountingId.Add("241270", 324171);
            transfereeAccountingId.Add("242041", 325512);
            transfereeAccountingId.Add("242219", 325854);
            transfereeAccountingId.Add("242434", 326216);
            transfereeAccountingId.Add("243579", 328251);
            transfereeAccountingId.Add("243625", 328344);
            transfereeAccountingId.Add("244369", 329606);
            transfereeAccountingId.Add("244915", 330542);
            transfereeAccountingId.Add("246067", 332555);
            transfereeAccountingId.Add("246157", 332716);
            transfereeAccountingId.Add("246572", 333494);
            transfereeAccountingId.Add("246948", 334151);
            transfereeAccountingId.Add("247656", 335455);
            transfereeAccountingId.Add("248296", 336573);
            transfereeAccountingId.Add("249036", 337903);
            transfereeAccountingId.Add("249104", 338025);
            transfereeAccountingId.Add("249998", 339601);
            transfereeAccountingId.Add("250208", 339974);
            transfereeAccountingId.Add("250323", 340179);
            transfereeAccountingId.Add("250332", 340199);
            transfereeAccountingId.Add("251031", 341425);
            transfereeAccountingId.Add("251169", 341662);
            transfereeAccountingId.Add("251741", 342719);
            transfereeAccountingId.Add("252117", 343428);
            transfereeAccountingId.Add("252607", 344369);
            transfereeAccountingId.Add("252702", 344556);
            transfereeAccountingId.Add("252823", 344786);
            transfereeAccountingId.Add("252880", 344894);
            transfereeAccountingId.Add("253117", 345348);
            transfereeAccountingId.Add("253497", 346059);
            transfereeAccountingId.Add("253987", 346999);
            transfereeAccountingId.Add("254186", 347378);
            transfereeAccountingId.Add("254636", 348215);
            transfereeAccountingId.Add("255532", 196527);
            transfereeAccountingId.Add("256737", 352060);
            transfereeAccountingId.Add("257435", 353414);
            transfereeAccountingId.Add("257492", 353527);
            transfereeAccountingId.Add("257523", 353592);
            transfereeAccountingId.Add("257580", 353698);
            transfereeAccountingId.Add("257590", 353718);
            transfereeAccountingId.Add("257885", 354257);
            transfereeAccountingId.Add("258328", 355113);
            transfereeAccountingId.Add("259014", 356398);
            transfereeAccountingId.Add("259036", 356440);
            transfereeAccountingId.Add("260168", 358526);
            transfereeAccountingId.Add("260280", 358744);
            transfereeAccountingId.Add("260402", 358974);
            transfereeAccountingId.Add("260566", 359270);
            transfereeAccountingId.Add("260967", 360044);
            transfereeAccountingId.Add("260991", 360090);
            transfereeAccountingId.Add("261004", 360120);
            transfereeAccountingId.Add("261144", 360371);
            transfereeAccountingId.Add("261470", 360999);
            transfereeAccountingId.Add("261676", 361382);
            transfereeAccountingId.Add("261684", 361398);
            transfereeAccountingId.Add("261718", 361460);
            transfereeAccountingId.Add("261792", 361600);
            transfereeAccountingId.Add("261963", 361919);
            transfereeAccountingId.Add("262089", 362163);
            transfereeAccountingId.Add("262220", 362414);
            transfereeAccountingId.Add("262617", 363175);
            transfereeAccountingId.Add("262646", 363233);
            transfereeAccountingId.Add("262893", 363700);
            transfereeAccountingId.Add("263099", 364103);
            transfereeAccountingId.Add("263221", 360193);
            transfereeAccountingId.Add("263235", 364365);
            transfereeAccountingId.Add("263240", 364375);
            transfereeAccountingId.Add("263512", 364895);
            transfereeAccountingId.Add("264330", 366403);
            transfereeAccountingId.Add("264871", 367326);
            transfereeAccountingId.Add("265423", 368299);
            transfereeAccountingId.Add("265500", 368456);
            transfereeAccountingId.Add("265958", 369236);
            transfereeAccountingId.Add("266013", 369341);
            transfereeAccountingId.Add("266045", 369402);
            transfereeAccountingId.Add("266125", 369557);
            transfereeAccountingId.Add("266133", 369569);
            transfereeAccountingId.Add("266260", 369796);
            transfereeAccountingId.Add("266342", 369947);
            transfereeAccountingId.Add("266432", 370081);
            transfereeAccountingId.Add("266716", 370521);
            transfereeAccountingId.Add("266718", 370526);
            transfereeAccountingId.Add("266728", 370545);
            transfereeAccountingId.Add("266735", 370559);
            transfereeAccountingId.Add("266743", 370573);
            transfereeAccountingId.Add("266945", 370907);
            transfereeAccountingId.Add("267116", 371226);
            transfereeAccountingId.Add("267176", 371372);
            transfereeAccountingId.Add("267222", 371444);
            transfereeAccountingId.Add("267349", 371657);
            transfereeAccountingId.Add("267412", 371781);
            transfereeAccountingId.Add("267477", 371902);
            transfereeAccountingId.Add("267482", 371911);
            transfereeAccountingId.Add("267497", 371942);
            transfereeAccountingId.Add("267796", 372467);
            transfereeAccountingId.Add("267839", 372554);
            transfereeAccountingId.Add("267925", 372691);
            transfereeAccountingId.Add("268436", 373665);
            transfereeAccountingId.Add("268493", 373787);
            transfereeAccountingId.Add("268674", 374107);
            transfereeAccountingId.Add("268741", 374235);
            transfereeAccountingId.Add("268750", 374259);
            transfereeAccountingId.Add("268835", 374428);
            transfereeAccountingId.Add("268860", 374477);
            transfereeAccountingId.Add("268870", 374494);
            transfereeAccountingId.Add("268987", 374720);
            transfereeAccountingId.Add("269095", 374929);
            transfereeAccountingId.Add("269232", 375199);
            transfereeAccountingId.Add("269356", 375411);
            transfereeAccountingId.Add("269374", 375447);
            transfereeAccountingId.Add("269537", 375772);
            transfereeAccountingId.Add("269733", 376114);
            transfereeAccountingId.Add("269744", 376136);
            transfereeAccountingId.Add("269753", 376153);
            transfereeAccountingId.Add("269875", 376387);
            transfereeAccountingId.Add("269913", 376455);
            transfereeAccountingId.Add("269922", 376473);
            transfereeAccountingId.Add("270007", 376620);
            transfereeAccountingId.Add("270050", 376702);
            transfereeAccountingId.Add("270051", 376704);
            transfereeAccountingId.Add("270059", 376718);
            transfereeAccountingId.Add("270060", 376720);
            transfereeAccountingId.Add("270061", 376722);
            transfereeAccountingId.Add("270062", 376724);
            transfereeAccountingId.Add("270064", 376728);
            transfereeAccountingId.Add("270067", 376734);
            transfereeAccountingId.Add("270068", 376737);
            transfereeAccountingId.Add("270069", 376738);
            transfereeAccountingId.Add("270070", 376740);
            transfereeAccountingId.Add("270072", 376744);
            transfereeAccountingId.Add("270073", 376746);
            transfereeAccountingId.Add("270074", 376748);
            transfereeAccountingId.Add("270075", 376750);
            transfereeAccountingId.Add("270076", 376752);
            transfereeAccountingId.Add("270082", 376764);
            transfereeAccountingId.Add("270083", 376766);
            transfereeAccountingId.Add("270085", 376770);
            transfereeAccountingId.Add("270086", 376772);
            transfereeAccountingId.Add("270087", 376774);
            transfereeAccountingId.Add("270098", 376796);
            transfereeAccountingId.Add("270099", 376798);
            transfereeAccountingId.Add("270103", 376806);
            transfereeAccountingId.Add("270104", 376808);
            transfereeAccountingId.Add("270148", 376891);
            transfereeAccountingId.Add("270155", 376905);
            transfereeAccountingId.Add("270228", 377012);
            transfereeAccountingId.Add("270399", 377364);
            transfereeAccountingId.Add("270460", 377463);
            transfereeAccountingId.Add("270505", 377526);
            transfereeAccountingId.Add("270511", 377536);
            transfereeAccountingId.Add("270555", 377616);
            transfereeAccountingId.Add("270618", 377721);
            transfereeAccountingId.Add("270682", 377816);
            transfereeAccountingId.Add("270688", 377830);
            transfereeAccountingId.Add("270697", 377849);
            transfereeAccountingId.Add("270702", 377860);
            transfereeAccountingId.Add("270747", 377952);
            transfereeAccountingId.Add("270927", 378295);
            transfereeAccountingId.Add("271054", 378551);
            transfereeAccountingId.Add("271200", 378824);
            transfereeAccountingId.Add("271267", 378959);
            transfereeAccountingId.Add("271295", 379018);
            transfereeAccountingId.Add("271460", 379312);
            transfereeAccountingId.Add("271468", 379327);
            transfereeAccountingId.Add("271531", 379446);
            transfereeAccountingId.Add("271640", 379645);
            transfereeAccountingId.Add("271800", 379926);
            transfereeAccountingId.Add("271831", 379984);
            transfereeAccountingId.Add("271897", 380073);
            transfereeAccountingId.Add("272124", 380477);
            transfereeAccountingId.Add("272169", 380551);
            transfereeAccountingId.Add("272420", 380997);
            transfereeAccountingId.Add("272500", 381142);
            transfereeAccountingId.Add("272740", 381611);
            transfereeAccountingId.Add("272992", 382078);
            transfereeAccountingId.Add("273081", 382239);
            transfereeAccountingId.Add("273086", 382249);
            transfereeAccountingId.Add("273148", 382362);
            transfereeAccountingId.Add("273473", 382911);
            transfereeAccountingId.Add("273517", 382978);
            transfereeAccountingId.Add("274045", 383983);
            transfereeAccountingId.Add("274364", 384610);
            transfereeAccountingId.Add("274394", 384672);
            transfereeAccountingId.Add("274482", 384846);
            transfereeAccountingId.Add("274486", 384855);
            transfereeAccountingId.Add("274527", 384938);
            transfereeAccountingId.Add("274550", 384983);
            transfereeAccountingId.Add("274580", 385045);
            transfereeAccountingId.Add("274681", 385249);
            transfereeAccountingId.Add("274736", 385359);
            transfereeAccountingId.Add("274985", 385836);
            transfereeAccountingId.Add("275029", 385920);
            transfereeAccountingId.Add("275038", 385940);
            transfereeAccountingId.Add("275210", 386245);
            transfereeAccountingId.Add("275240", 386307);
            transfereeAccountingId.Add("275706", 387212);
            transfereeAccountingId.Add("276080", 387900);
            transfereeAccountingId.Add("276088", 273923);
            transfereeAccountingId.Add("276198", 388132);
            transfereeAccountingId.Add("276299", 388326);
            transfereeAccountingId.Add("276461", 352812);
            transfereeAccountingId.Add("276561", 388830);
            transfereeAccountingId.Add("276584", 388874);
            transfereeAccountingId.Add("276587", 388881);
            transfereeAccountingId.Add("276683", 389062);
            transfereeAccountingId.Add("276704", 389104);
            transfereeAccountingId.Add("276716", 389137);
            transfereeAccountingId.Add("276903", 389500);
            transfereeAccountingId.Add("277011", 389707);
            transfereeAccountingId.Add("277066", 389811);
            transfereeAccountingId.Add("277144", 389958);
            transfereeAccountingId.Add("277151", 389972);
            transfereeAccountingId.Add("277519", 390680);
            transfereeAccountingId.Add("277539", 390720);
            transfereeAccountingId.Add("277809", 391262);
            transfereeAccountingId.Add("277862", 391364);
            transfereeAccountingId.Add("277890", 391425);
            transfereeAccountingId.Add("278308", 392253);
            transfereeAccountingId.Add("278380", 392389);
            transfereeAccountingId.Add("279127", 393816);
            transfereeAccountingId.Add("279196", 393921);
            transfereeAccountingId.Add("279197", 393923);
            transfereeAccountingId.Add("279271", 394071);
            transfereeAccountingId.Add("279288", 394093);
            transfereeAccountingId.Add("279309", 394136);
            transfereeAccountingId.Add("279403", 394324);
            transfereeAccountingId.Add("279458", 394434);
            transfereeAccountingId.Add("279473", 394461);
            transfereeAccountingId.Add("279602", 394710);
            transfereeAccountingId.Add("279615", 394732);
            transfereeAccountingId.Add("279635", 394775);
            transfereeAccountingId.Add("279971", 395416);
            transfereeAccountingId.Add("280037", 395538);
            transfereeAccountingId.Add("280135", 395726);
            transfereeAccountingId.Add("280155", 395753);
            transfereeAccountingId.Add("280202", 395848);
            transfereeAccountingId.Add("280263", 395942);
            transfereeAccountingId.Add("280284", 395981);
            transfereeAccountingId.Add("280431", 396246);
            transfereeAccountingId.Add("280510", 396380);
            transfereeAccountingId.Add("280800", 396923);
            transfereeAccountingId.Add("280839", 396989);
            transfereeAccountingId.Add("281020", 397308);
            transfereeAccountingId.Add("281091", 397446);
            transfereeAccountingId.Add("281130", 397526);
            transfereeAccountingId.Add("281211", 397663);
            transfereeAccountingId.Add("281539", 398255);
            transfereeAccountingId.Add("281584", 398345);
            transfereeAccountingId.Add("281598", 398371);
            transfereeAccountingId.Add("281768", 398696);
            transfereeAccountingId.Add("282017", 399143);
            transfereeAccountingId.Add("282039", 399177);
            transfereeAccountingId.Add("282051", 399202);
            transfereeAccountingId.Add("282058", 399216);
            transfereeAccountingId.Add("282304", 399632);
            transfereeAccountingId.Add("282390", 399820);
            transfereeAccountingId.Add("282435", 399912);
            transfereeAccountingId.Add("282459", 399958);
            transfereeAccountingId.Add("282486", 400005);
            transfereeAccountingId.Add("282642", 400284);
            transfereeAccountingId.Add("282702", 400402);
            transfereeAccountingId.Add("282745", 400486);
            transfereeAccountingId.Add("282896", 400756);
            transfereeAccountingId.Add("282912", 400789);
            transfereeAccountingId.Add("282954", 400851);
            transfereeAccountingId.Add("283071", 401073);
            transfereeAccountingId.Add("283230", 401395);
            transfereeAccountingId.Add("283244", 401422);
            transfereeAccountingId.Add("283338", 401573);
            transfereeAccountingId.Add("283416", 401737);
            transfereeAccountingId.Add("283601", 402044);
            transfereeAccountingId.Add("283602", 402046);
            transfereeAccountingId.Add("283688", 402197);
            transfereeAccountingId.Add("283778", 402357);
            transfereeAccountingId.Add("283858", 402496);
            transfereeAccountingId.Add("283860", 402500);
            transfereeAccountingId.Add("283880", 402537);
            transfereeAccountingId.Add("283993", 402731);
            transfereeAccountingId.Add("284032", 402801);
            transfereeAccountingId.Add("284127", 402989);
            transfereeAccountingId.Add("284201", 403148);
            transfereeAccountingId.Add("284207", 403160);
            transfereeAccountingId.Add("284280", 403278);
            transfereeAccountingId.Add("284470", 403581);
            transfereeAccountingId.Add("284497", 403633);
            transfereeAccountingId.Add("284536", 403710);
            transfereeAccountingId.Add("284574", 403784);
            transfereeAccountingId.Add("284780", 404162);
            transfereeAccountingId.Add("284781", 404164);
            transfereeAccountingId.Add("284782", 404166);
            transfereeAccountingId.Add("284784", 404170);
            transfereeAccountingId.Add("284821", 404242);
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