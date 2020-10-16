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
            //loadAllRecords = true;

            SetConsoleWriteLine();
            SetMovesToImport(loadAllRecords);
            await RetrieveJobsAccountAndVendor();

            foreach (var regNumber in movesToImport)
            {
                try
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                    Trace.WriteLine($"{regNumber}, -----------------------------------------------------------------------------------");

                    await SungateApi.setApiAccessTokenAsync(_httpClient);
                    var move = await WaterDbAccess.RetrieveWaterRecords(regNumber);

                    if (move == null)
                    {
                        continue;
                    }

                    //Add the job
                    var jobId = await addStorageJob(move, regNumber);
                    var transfereeEntity = await JobsDbAccess.GetJobsTransfereeId(jobId);

                    //update datecreated on the job
                    JobsDbAccess.ChangeDateCreated(jobId, move.DateEntered.GetValueOrDefault(DateTime.UtcNow), regNumber);

                    //Add JobContacts
                    await addJobContacts(move, jobId, regNumber);

                    //Add SuperService
                    var result = await JobsApi.CreateStorageSSO(_httpClient, jobId, regNumber);
                    JobsDbAccess.ChangeDisplayName(result.Id, move.RegNumber);

                    var serviceOrders = await JobsDbAccess.GetServiceOrderForJobs(jobId, regNumber);

                    // ORIGIN
                    var oaVendor = _vendor.Find(v => v.Accounting_SI_Code.Equals(move.OriginAgent.VendorNameId));
                    await JobsApi.UpdateOriginMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 24).Id, oaVendor, move, jobId, regNumber);

                    // DESTINATION
                    var daVendor = _vendor.Find(v => v.Accounting_SI_Code.Equals(move.DestinationAgent.VendorNameId));
                    await JobsApi.UpdateDestinationMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 26).Id, daVendor, move, jobId, regNumber);

                    var legacyInsuranceClaims = await WaterDbAccess.RetrieveInsuranceClaims(move.RegNumber);

                    // STORAGE
                    await updateStorageJob(move, jobId, serviceOrders, regNumber, transfereeEntity, legacyInsuranceClaims);

                    // INSURANCE
                    await JobsApi.UpdateICtMilestone(_httpClient, serviceOrders.FirstOrDefault(so => so.ServiceId == 27).Id, move, jobId, legacyInsuranceClaims, regNumber);

                    #region JobCost

                    var billableItemTypes = await JobsDbAccess.RetrieveBillableItemTypes(regNumber);

                    var paymentSends = await WaterDbAccess.RetrieveJobCostExpense(move.RegNumber);
                    foreach (var legacyJC in paymentSends)
                    {
                        var response = await DetermineBillTo(legacyJC.NAMES_ID, transfereeEntity, regNumber);

                        legacyJC.VendorID = response.BilltoId;
                        legacyJC.BillToLabel = response.BilltoType;
                    }

                    await JobsApi.CreateAndUpdateJobCostExpense(_httpClient, _vendor, paymentSends, billableItemTypes, jobId,
                        serviceOrders.FirstOrDefault(so => so.ServiceId == 29), regNumber);

                    var paymentReceived = await WaterDbAccess.RetrieveJobCostRevenue(move.RegNumber);
                    foreach (var legacyJC in paymentReceived)
                    {
                        var response = await DetermineBillTo(legacyJC.NAMES_ID, transfereeEntity, regNumber);

                        legacyJC.VendorID = response.BilltoId;
                        legacyJC.BillToLabel = response.BilltoType;
                    }

                    await JobsApi.CreateAndUpdateJobCostRevenue(_httpClient, _vendor, paymentReceived, billableItemTypes, jobId,
                        serviceOrders.FirstOrDefault(so => so.ServiceId == 29), regNumber);

                    var superServiceOrderId = serviceOrders.FirstOrDefault(so => so.ServiceId == 29).SuperServiceOrderId;

                    await JobsDbAccess.LockJC(jobId, regNumber, superServiceOrderId);
                    await JobsDbAccess.MarkAsPosted(superServiceOrderId, DateTime.Now, true, regNumber);
                    await JobsDbAccess.MarkAllAsVoid(superServiceOrderId, regNumber);

                    #endregion JobCost

                    //Add Notes
                    await AddNotesFromGmmsToArive(move, jobId, regNumber);

                    //Add Prompts
                    await AddPromptsFromGmmsToArive(move, jobId, regNumber);

                    Trace.WriteLine($"{regNumber}, EndTime: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"{regNumber}, *** ERROR ***");
                    if (ex.InnerException != null)
                    {
                        Trace.WriteLine($"{regNumber}, {ex.InnerException.Message}");
                    }
                    else
                    {
                        Trace.WriteLine($"{regNumber}, {ex.Message}");
                    }

                    Console.WriteLine($"**** ERROR ****");
                    Console.WriteLine($"{ex.Message}");
                }

                Trace.Flush();
            }
        }

        private static async Task updateStorageJob(Move move, int jobId, List<ServiceOrder> serviceOrders, string regNumber, Transferee transferee, List<InsuranceClaims> legacyInsuranceClaims)
        {
            Console.WriteLine("Updating ST");
            Trace.WriteLine($"{regNumber}, Updating ST");

            var vendorEntity = _vendor.FirstOrDefault(v => v.Accounting_SI_Code == move.StorageAgent.VendorNameId);
            var soId = serviceOrders.FirstOrDefault(so => so.ServiceId == 32).Id;

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
                    Console.WriteLine($"{regNumber}, Cant find the billto for Storage, so we are defaulting it");
                    Trace.Write($"{regNumber}, Cant find the billto for Storage, so we are defaulting it");
                }
            }

            if (!string.IsNullOrEmpty(move.StorageAgent.HOW_SENT) && billTo == null)
            {
                Trace.WriteLine($"{regNumber}, Missing Storage BillTo {move.StorageAgent.HOW_SENT}");
            }

            await JobsApi.updateStorageRevRecord(_httpClient, soId, storageRevId, move, jobId, regNumber, billTo, billToLabel, legacyInsuranceClaims);
        }

        private static async Task AddPromptsFromGmmsToArive(Move move, int jobId, string regNumber)
        {
            var legacyPromptEntity = await WaterDbAccess.RetrievePrompts(move.RegNumber);
            if (legacyPromptEntity == null || legacyPromptEntity.Count == 0)
            {
                Trace.WriteLine($"{regNumber}, No Available prompts found in GMMS");
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
                    Trace.WriteLine($"{regNumber}, Can't get Prompt created User So defaulting it to MigrationScript@test.com");
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
                Trace.WriteLine($"{regNumber}, No Available notes found in GMMS");
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
            }

            await TaskApi.CreateNotes(_httpClient, createJobNoteRequests, jobId, regNumber);
        }

        private static async Task addJobContacts(Move move, int jobId, string regNumber)
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
                                Console.WriteLine("Defaulting MoveConsultant to Angela La Fronza, due to bad data");
                                Trace.WriteLine($"{regNumber}, Defaulting MoveConsultant to Angela La Fronza, due to bad data");
                                nameToUse = "Angela.Lafronza";
                            }
                        }

                        dictionaryValue = NameTranslator.repo.GetValueOrDefault(nameToUse);

                        if (string.IsNullOrEmpty(dictionaryValue))
                        {
                            Trace.WriteLine($"{regNumber}, Move Consultant from GMMS {nameToUse} couldn't be found in Arive thus Defaulting to Angela La Fronza");
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
                }

                if (!string.IsNullOrEmpty(dictionaryValue))
                {
                    var adObj = await SungateApi.GetADName(_httpClient, dictionaryValue, regNumber);

                    if (adObj == null || adObj.Count == 0)
                    {
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
            Trace.WriteLine($"{regNumber}, Adding Job Contacts");

            var url = $"/{jobId}/contacts";

            await JobsApi.CallJobsApi(_httpClient, url, jobContactList);
        }

        private static async Task<int> addStorageJob(Move move, string regNumber)
        {
            Console.WriteLine("Creating a job");
            Trace.WriteLine($"{regNumber}, Creating a job");

            var url = string.Empty;

            var movesAccount = _accountEntities.FirstOrDefault(ae => ae.AccountingId.Equals(move.AccountId));
            var movesBooker = _vendor.FirstOrDefault(ae => ae.Accounting_SI_Code.Equals(move.Booker));

            if (movesAccount == null)
            {
                if (move.AccountId.Equals("1674")) //Shipper Direct
                {
                    Trace.WriteLine($"{regNumber}, Missing Account in Arive {move.AccountId}, thus Defaulting Shipper Direct");
                    movesAccount = _accountEntities.FirstOrDefault(ae => ae.Id == 283);
                }
                else
                {
                    throw new Exception($"Missing Account in Arive {move.AccountId}");
                }
            }

            var response = await DetermineBillTo(move.BILL, null, regNumber);

            if (response.BilltoId == null)
            {
                Trace.WriteLine($"{regNumber}, Missing BillTo in Arive {move.BILL}");
                Trace.WriteLine($"{regNumber}, Defaulting BillTo as Shipper Direct");
                response.BilltoId = 283;
                response.BilltoType = "Account";
            }

            var model = move.ToJobModel(movesAccount.Id, movesBooker?.Id, response.BilltoId, response.BilltoType);
            string parsedResponse = await JobsApi.CallJobsApi(_httpClient, url, model);

            Console.WriteLine($"Job added {parsedResponse}");
            Trace.WriteLine($"{regNumber}, Job added {parsedResponse}");
            return int.Parse(parsedResponse);
        }

        #region Helpers

        private static async Task RetrieveJobsAccountAndVendor()
        {
            Console.WriteLine("Retrieving Existing Accounts and Vendors from Jobs");
            //Trace.WriteLine("Retrieving Existing Accounts and Vendors from Jobs");
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
                //movesToImport.Add("274486");
                movesToImport.Add("274527"); // GOOD one to import according to heather
                //movesToImport.Add("266185"); // missing account
                //movesToImport.Add("270059"); // bill to is transferee
            }
            else
            {
                var array = new String[] {
"284497"
,"255950"
,"265958"
,"246572"
,"267349"
,"261718"
,"284216"
,"265500"
,"266185"
,"270059"
,"262287"
,"270702"
,"146231"
,"185977"
,"193038"
,"202334"
,"257586"
,"266836"
,"203448"
,"209609"
,"277539"
,"279343"
,"269733"
,"277151"
,"240538"
,"265423"
,"201219"
,"261684"
,"239921"
,"270087"
,"269922"
,"276704"
,"271200"
,"106417"
,"221856"
,"272992"
,"279602"
,"260402"
,"282912"
,"262893"
,"242434"
,"280839"
,"247065"
,"250323"
,"260280"
,"238272"
,"270399"
,"179413"
,"249104"
,"167096"
,"270460"
,"271800"
,"282954"
,"190132"
,"268255"
,"269924"
,"263240"
,"280510"
,"276198"
,"270511"
,"236545"
,"274482"
,"283230"
,"272740"
,"206146"
,"266728"
,"271640"
,"270062"
,"252607"
,"283858"
,"282486"
,"283601"
,"267764"
,"272358"
,"277890"
,"257435"
,"218364"
,"270408"
,"276299"
,"238375"
,"246948"
,"257885"
,"279196"
,"282610"
,"266013"
,"281081"
,"282370"
,"270682"
,"152245"
,"279197"
,"262380"
,"266133"
,"257998"
,"268741"
,"284821"
,"269374"
,"215297"
,"277401"
,"266945"
,"281598"
,"270060"
,"237351"
,"282642"
,"262220"
,"260168"
,"274394"
,"253497"
,"267116"
,"276461"
,"276716"
,"221731"
,"267482"
,"210120"
,"282435"
,"269744"
,"206884"
,"282896"
,"238072"
,"116638"
,"251741"
,"160010"
,"281020"
,"249998"
,"250554"
,"270064"
,"259014"
,"252117"
,"270098"
,"273517"
,"177872"
,"244915"
,"271831"
,"233827"
,"262089"
,"280111"
,"269095"
,"260967"
,"237589"
,"200069"
,"276903"
,"225365"
,"268870"
,"275628"
,"226406"
,"133965"
,"168611"
,"271295"
,"273148"
,"236709"
,"282390"
,"269537"
,"270688"
,"270099"
,"270067"
,"265933"
,"277809"
,"233939"
,"268674"
,"257580"
,"281663"
,"274580"
,"180632"
,"284574"
,"284470"
,"237039"
,"234933"
,"270148"
,"261004"
,"282039"
,"244369"
,"270069"
,"263221"
,"270508"
,"121356"
,"274527"
,"267549"
,"281539"
,"246067"
,"279557"
,"270070"
,"270747"
,"268493"
,"272420"
,"248044"
,"176484"
,"151325"
,"284201"
,"279288"
,"275681"
,"157923"
,"270068"
,"266432"
,"259036"
,"262758"
,"279127"
,"219470"
,"254186"
,"253987"
,"282058"
,"209366"
,"282017"
,"263512"
,"269875"
,"271897"
,"275029"
,"217742"
,"261963"
,"126658"
,"276561"
,"217185"
,"276584"
,"235378"
,"127017"
,"280284"
,"207812"
,"280800"
,"278380"
,"257523"
,"188511"
,"279329"
,"206792"
,"204789"
,"280431"
,"266260"
,"268936"
,"263235"
,"278051"
,"270051"
,"270555"
,"262617"
,"262646"
,"270697"
,"270072"
,"254636"
,"217986"
,"261144"
,"180509"
,"236852"
,"267925"
,"140767"
,"256737"
,"276080"
,"267839"
,"271531"
,"270664"
,"275240"
,"126836"
,"271468"
,"274486"
,"270076"
,"254333"
,"274364"
,"259907"
,"243579"
,"275706"
,"233233"
,"271460"
,"263931"
,"270074"
,"279473"
,"272500"
,"270050"
,"237017"
,"140675"
,"247050"
,"270073"
,"270075"
,"266735"
,"174697"
,"264088"
,"268690"
,"277144"
,"267497"
,"267734"
,"270705"
,"280263"
,"269356"
,"267412"
,"282459"
,"270079"
,"235336"
,"267477"
,"277519"
,"279458"
,"272080"
,"272124"
,"266718"
,"282702"
,"240900"
,"247656"
,"248296"
,"264871"
,"269232"
,"275308"
,"268835"
,"272169"
,"241270"
,"257492"
,"283180"
,"279635"
,"265047"
,"279271"
,"270082"
,"266716"
,"246157"
,"241077"
,"277011"
,"281768"
,"211489"
,"242219"
,"235589"
,"217420"
,"196270"
,"280037"
,"275510"
,"282051"
,"199186"
,"270155"
,"271629"
,"268750"
,"270083"
,"273081"
,"283244"
,"267176"
,"257590"
,"284280"
,"269753"
,"175353"
,"269350"
,"250208"
,"252702"
,"202681"
,"260566"
,"270228"
,"266743"
,"276587"
,"191731"
,"274681"
,"279309"
,"266045"
,"270085"
,"274045"
,"250332"
,"268436"
,"276683"
,"155129"
,"234101"
,"212713"
,"216320"
,"275525"
,"280202"
,"251169"
,"210494"
,"197263"
,"283778"
,"258328"
,"265074"
,"261470"
,"275038"
,"278308"
,"270061"
,"276088"
,"270086"
,"206626"
,"252823"
,"270927"
,"155004"
,"255532"
,"270007"
,"283993"
,"154280"
,"275210"
,"222775"
,"267796"
,"281130"
,"269913"
,"284207"
,"277862"
,"252880"
,"270103"
,"224051"
,"274550"
,"273086"
,"270505"
,"247051"
,"130902"
,"274390"
,"274985"
,"268987"
,"281091"
,"251031"
,"282351"
,"260991"
,"249036"
,"274736"
,"270104"
,"269316"
,"263099"
,"283880"
,"228764"
,"115613"
,"266125"
,"253117" };

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