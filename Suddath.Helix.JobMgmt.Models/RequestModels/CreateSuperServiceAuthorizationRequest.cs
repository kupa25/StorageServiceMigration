using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateSuperServiceAuthorizationRequest
    {
        public int Amount { get; set; }
        public string MeasurementType { get; set; }
    }
}