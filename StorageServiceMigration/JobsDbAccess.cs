using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class JobsDbAccess
    {
        public static void ChangeDateCreated(int jobId, DateTime date)
        {
            Console.WriteLine($"Updating Jobs Created Date to {date}");
            using (var context = new JobDbContext())
            {
                var createdJob = context.Job.Find(jobId);

                createdJob.DateCreated = date;
                context.SaveChanges();
            }
        }
    }
}