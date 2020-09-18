using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.JobCost
{
    public class CreateItemCreditMemoRequest
    {
        public int Id { get; set; }
        public decimal CreditMemoAmount { get; set; }
    }
}