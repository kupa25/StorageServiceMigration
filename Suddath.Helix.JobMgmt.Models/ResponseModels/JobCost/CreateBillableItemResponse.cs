using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class CreateBillableItemResponse
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public string BillToType { get; set; }
        public int? BillToId { get; set; }
        public string BillToName { get; set; }
    }
}