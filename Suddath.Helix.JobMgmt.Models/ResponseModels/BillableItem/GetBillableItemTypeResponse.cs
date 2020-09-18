using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.BillableItem
{
    public class GetBillableItemTypeResponse
    {
        public int Id { get; set; }
        public string BillableItemTypeName { get; set; }
        public string AccountCode { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}