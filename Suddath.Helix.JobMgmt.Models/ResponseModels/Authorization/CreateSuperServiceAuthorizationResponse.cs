using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.Authorization
{
    public class CreateSuperServiceAuthorizationResponse
    {
        public int Id { get; set; }
        public int SuperServiceId { get; set; }
        public string SuperServiceName { get; set; }
        public int Amount { get; set; }
        public string MeasurementType { get; set; }
    }
}