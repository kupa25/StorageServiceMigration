using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.TaskMgmt.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Services.Water.Mapper;

namespace StorageServiceMigration
{
    public static class TaskDbAccess
    {
        //public static string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;;database=Tasks;trusted_connection=yes;";
        //public static string connectionString = @"data source=daue2helix3sql01.database.windows.net;initial catalog=Helix3.Tasks;User ID=helix3_app;Password=CHEjSEK7qMHdt7!; Connect Timeout=120;MultipleActiveResultSets=True;";
        //public static string connectionString = @"data source=qaue2helix3sql01.database.windows.net;initial catalog=Helix3.Tasks;User ID=helix3_app;Password=c%$xm61RqykHjWU4; Connect Timeout=120;MultipleActiveResultSets=True;";
        public static string connectionString = @"data source=uaue2helix3sql01.database.windows.net;initial catalog=Helix3.Tasks;User ID=helix3_app;Password=g8f3Y0zMFT3emUL#; Connect Timeout=120;MultipleActiveResultSets=True;";

        //public static string connectionString = @"data source=qaue2helix3sql01.database.windows.net;initial catalog=Helix3.Tasks;User ID=helix3_app;Password=V$@h@ERZnrDFGvZ9; Connect Timeout=120;MultipleActiveResultSets=True;";

        public static void ChangeDateCreated(int noteId, DateTime date, string regNumber)
        {
            using (var context = new TaskMgmtDbContext(connectionString))
            {
                var createdNote = context.Note.Find(noteId);

                createdNote.DateCreated = date;
                context.SaveChanges();
            }
        }

        internal static async Task AddPrompts(List<WorkflowTask> workflowTasksToAdd, string regNumber, int jobId)
        {
            var originalTaskToAddCount = workflowTasksToAdd.Count;

            Console.WriteLine($"Adding {originalTaskToAddCount} Prompts");
            Trace.WriteLine($"{regNumber}, ,Processing {originalTaskToAddCount} Prompts");

            using (var context = new TaskMgmtDbContext(connectionString))
            {
                var existingTask = await context.WorkflowTask.Where(wt => wt.ReferenceId == jobId).ToListAsync();

                Trace.WriteLine($"{regNumber}, ,Looking for duplicates because Milestone pages might have already created prompts");

                foreach (var wtTask in existingTask)
                {
                    workflowTasksToAdd.RemoveAll(wt => wt.Subject.Equals(wtTask.Subject,
                        StringComparison.CurrentCultureIgnoreCase));
                }

                var revisedTaskToAddCount = workflowTasksToAdd.Count;

                if (revisedTaskToAddCount != originalTaskToAddCount)
                {
                    Trace.WriteLine($"{regNumber}, ,Found and removed duplicate prompts");
                }

                if (workflowTasksToAdd.Count == 0)
                {
                    return;
                }

                context.WorkflowTask.AddRange(workflowTasksToAdd);
                context.SaveChanges();
            }
        }

        internal static async Task AddNotes(List<CreateJobNoteRequest> createJobNoteRequests, int jobId, string regNumber)
        {
            Console.WriteLine($"Adding {createJobNoteRequests.Count} Notes to Task");
            Trace.WriteLine($"{regNumber}, , Adding {createJobNoteRequests.Count} Notes to Task");

            var noteEntity = createJobNoteRequests.ToNotesEntity();

            using (var context = new TaskMgmtDbContext(connectionString))
            {
                context.Note.AddRange(noteEntity);
                context.SaveChanges();
            }
        }

        internal static void RemovePrompts()
        {
            Console.WriteLine($"Removing Prompts create on behalf of MigrationScript@test.com");

            using (var context = new TaskMgmtDbContext(connectionString))
            {
                context.Database.ExecuteSqlCommand(@"
                                                Delete from WorkFlowTask
                                                where ModifiedBy = 'MigrationScript@test.com'
                                                ");
            }
        }
    }
}