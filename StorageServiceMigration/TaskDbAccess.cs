using Suddath.Helix.TaskMgmt.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StorageServiceMigration
{
    public static class TaskDbAccess
    {
        public static string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;;database=Tasks;trusted_connection=yes;";
        //public static string connectionString = @"data source=daue2helix3sql01.database.windows.net;initial catalog=Helix3.Tasks;User ID=helix3_app;Password=CHEjSEK7qMHdt7!; Connect Timeout=120;MultipleActiveResultSets=True;";

        public static void ChangeDateCreated(int noteId, DateTime date, string regNumber)
        {
            using (var context = new TaskMgmtDbContext(connectionString))
            {
                var createdNote = context.Note.Find(noteId);

                createdNote.DateCreated = date;
                context.SaveChanges();
            }
        }
    }
}