using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.JobCost
{
    public class PostAccruableItemRequest
    {
        [Required]
        public DateTime FinancialPeriodDateTime { get; set; }
    }
}