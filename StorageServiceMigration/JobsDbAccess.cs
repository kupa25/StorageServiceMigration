using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class JobsDbAccess
    {
        public static string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;;database=Jobs;trusted_connection=yes;";
        //public static string _connectionString= @"data source=daue2helix3sql01.database.windows.net;initial catalog=Helix3.Jobs;User ID=helix3_app;Password=CHEjSEK7qMHdt7!; Connect Timeout=120;MultipleActiveResultSets=True;";

        public static void ChangeDateCreated(int jobId, DateTime date)
        {
            Console.WriteLine($"Updating Jobs Created Date to {date}");
            using (var context = new JobDbContext(connectionString))
            {
                var createdJob = context.Job.Find(jobId);

                createdJob.DateCreated = date;
                context.SaveChanges();
            }
        }

        internal static async Task<List<ServiceOrder>> GetServiceOrderForJobs(int jobId)
        {
            Console.WriteLine($"Retrieve all the ServiceOrders Created");

            List<ServiceOrder> result;
            using (var context = new JobDbContext(connectionString))
            {
                result = await context.ServiceOrder.AsNoTracking()
                                                 .Include(so => so.SuperServiceOrder)
                                                 .Where(so => so.SuperServiceOrder.JobId == jobId).ToListAsync();
            }

            return result;
        }
    }
}