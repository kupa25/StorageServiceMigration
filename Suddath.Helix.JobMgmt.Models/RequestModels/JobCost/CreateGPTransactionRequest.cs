using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.JobCost
{
    public class CreateGPTransactionRequest
    {
        public string GPTransactionType { get; set; }
        public int RelatedId { get; set; }
    }
}