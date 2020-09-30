using Newtonsoft.Json;
using Suddath.Helix.JobMgmt.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class TaskApi
    {
        //private static string _tasksBaseUrl = "https://daue2helixtaskwa01.azurewebsites.net/api/v1/";
        private static string _tasksBaseUrl = "https://localhost:44366/api/v1/";

        #region Task Api call

        public static async Task<string> PostToTaskApi(HttpClient _httpClient, string url, dynamic model)
        {
            url = _tasksBaseUrl + url;

            var payload = JsonConvert.SerializeObject(model);
            var response = await _httpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var parsedResponse = await HandleResponse(response);
            return parsedResponse;
        }

        private static async Task<string> HandleResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"{response.ReasonPhrase} : {content}");
            }

            return content;
        }

        #endregion Task Api call

        internal static async Task CreateNotes(HttpClient httpClient, List<CreateJobNoteRequest> createJobNoteRequests, int jobId)
        {
            Console.WriteLine($"Adding {createJobNoteRequests.Count} Notes to Task");
            Trace.WriteLine($"Adding {createJobNoteRequests.Count} Notes to Task");

            //string url = "​Notes"; //TODO: figure out why this isn't working
            string url = $"Notes/jobs/{jobId}";

            foreach (var createJobNote in createJobNoteRequests)
            {
                createJobNote.JobId = jobId;
                createJobNote.ReferenceId = jobId;

                var noteId = await PostToTaskApi(httpClient, url, createJobNote);
                var note = JsonConvert.DeserializeObject<NoteResponseModel>(noteId);

                //call taskdb and update the datecreated
                TaskDbAccess.ChangeDateCreated(note.Id, createJobNote.DateCreated);
            }
        }
    }
}