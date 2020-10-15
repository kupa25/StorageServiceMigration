using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class JobsDbAccess
    {
        public static string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;;database=Jobs;trusted_connection=yes;";
        //public static string connectionString = @"data source=daue2helix3sql01.database.windows.net;initial catalog=Helix3.Jobs;User ID=helix3_app;Password=CHEjSEK7qMHdt7!; Connect Timeout=120;MultipleActiveResultSets=True;";

        private static List<BillableItemType> _billableItemTypes;

        public static void ChangeDateCreated(int jobId, DateTime date, string regNumber)
        {
            Console.WriteLine($"Updating Jobs Created Date to {date}");
            Trace.WriteLine($"{regNumber}, Updating Jobs Created Date to {date}");
            using (var context = new JobDbContext(connectionString))
            {
                var createdJob = context.Job.Find(jobId);

                createdJob.DateCreated = date;
                context.SaveChanges();
            }
        }

        internal static async Task<List<ServiceOrder>> GetServiceOrderForJobs(int jobId, string regNumber)
        {
            Console.WriteLine($"Retrieve all the ServiceOrders Created");
            Trace.WriteLine($"{regNumber}, Retrieve all the ServiceOrders Created");

            List<ServiceOrder> result;
            using (var context = new JobDbContext(connectionString))
            {
                result = await context.ServiceOrder.AsNoTracking()
                                                 .Include(so => so.SuperServiceOrder)
                                                 .Where(so => so.SuperServiceOrder.JobId == jobId).ToListAsync();
            }

            return result;
        }

        internal static async Task<Transferee> GetJobsTransfereeId(int jobId)
        {
            Transferee transfereeEntity;

            using (var context = new JobDbContext(connectionString))
            {
                transfereeEntity = context.Job.AsNoTracking().Include(j => j.Transferee)
                                              .Single(j => j.Id == jobId)
                                              .Transferee;
            }

            return transfereeEntity;
        }

        internal static void ChangeDisplayName(int ssoId, string regNumber)
        {
            Console.WriteLine($"Changing Storage DisplayName to {regNumber}");
            Trace.WriteLine($"{regNumber}, Changing Storage DisplayName to {regNumber}");

            using (var context = new JobDbContext(connectionString))
            {
                var createdJob = context.SuperServiceOrder.Find(ssoId);

                createdJob.DisplayId = regNumber;
                context.SaveChanges();
            }
        }

        internal static async Task<List<BillableItemType>> RetrieveBillableItemTypes(string regNumber)
        {
            Console.WriteLine($"Retrieve JC BillableItemTypes");
            Trace.WriteLine($"{regNumber}, Retrieve JC BillableItemTypes");

            if (_billableItemTypes == null || _billableItemTypes.Count == 0)
            {
                using (var context = new JobDbContext(connectionString))
                {
                    _billableItemTypes = await context.BillableItemType.AsNoTracking().Where(bi => bi.IsActive.Value).ToListAsync();
                }
            }

            return _billableItemTypes;
        }

        internal static async Task ChangePayableItemStatus(string status, int pisId, string regNumber)
        {
            Console.WriteLine($"Changing Status of JC Expense record {regNumber}");
            Trace.WriteLine($"{regNumber}, Changing Status of JC Expense record ");

            using (var context = new JobDbContext(connectionString))
            {
                var pitem = context.PayableItem.Find(pisId);

                pitem.PayableItemStatusIdentifier = status;
                context.SaveChanges();
            }
        }

        internal static async Task ChangeBillableItemStatus(string status, int bid, string regNumber)
        {
            Console.WriteLine($"Changing Status of JC Rev record {regNumber}");
            Trace.WriteLine($"{regNumber}, Changing Status of JC Rev record ");

            using (var context = new JobDbContext(connectionString))
            {
                var pitem = context.BillableItem.Find(bid);

                pitem.BillableItemStatusIdentifier = status;
                context.SaveChanges();
            }
        }

        internal static async Task CreateVendorInvoiceRecord(int id, string regNumber, string cHECK, string iNVOICE_NUMBER, DateTime? dATE_PAID)
        {
            Console.WriteLine($"Adding Vendor Invoice Record for {regNumber}");
            Trace.WriteLine($"{regNumber}, Adding Vendor Invoice Record for ");

            using (var context = new JobDbContext(connectionString))
            {
                //var vendorInvoice = new VendorInvoice
                //{
                //}

                //pitem.PayableItemStatusIdentifier = status;
                //context.SaveChanges();
            }
        }
    }
}