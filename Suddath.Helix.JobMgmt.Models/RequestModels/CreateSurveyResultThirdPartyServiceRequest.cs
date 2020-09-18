using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateSurveyResultThirdPartyServiceRequest
    {
        public string ThirdPartyServiceName { get; set; }
        public int Count { get; set; }
    }
}