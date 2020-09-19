using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class ADUser
    {
        public bool accountEnabled { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string phoneExtenstion { get; set; }
    }
}