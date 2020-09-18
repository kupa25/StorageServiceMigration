using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class CreateGPTransactionResponse
    {
        public int TransactionId { get; set; }
        public string GPTransactionType { get; set; }
        public int RelatedId { get; set; }
    }
}