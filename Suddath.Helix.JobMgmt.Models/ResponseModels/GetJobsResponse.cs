using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class GetJobsResponse
    {
        public int JobId { get; set; }
        public string AccountName { get; set; }
        public string BillToName { get; set; }
        public string TransfereeFirstName { get; set; }
        public string TransfereeLastName { get; set; }
        public string TransfereePhone { get; set; }
        public string TransfereeEmail { get; set; }
        public string OriginAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string MoveStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string CreatedBy { get; set; }
        public List<string> SuperServiceIconNames { get; set; }
        public string BranchName { get; set; }
        public string MoveConsultantName { get; set; }
    }
}