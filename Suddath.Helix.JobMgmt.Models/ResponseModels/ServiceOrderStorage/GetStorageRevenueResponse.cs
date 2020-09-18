using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderStorage
{
    public class GetStorageRevenueResponse
    {
        public int Id { get; set; }
        public int ServiceOrderId { get; set; }
        public int? BillToId { get; set; }
        public string BillToType { get; set; }
        public string BillToName { get; set; }
        public string BillToLabel { get; set; }
        public string ContactEmail { get; set; }
        public int? StorageBillableWeightLb { get; set; }
        public DateTime? StorageEffectiveBillDate { get; set; }
        public DateTime? BillingRecordEndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsArrears { get; set; }
        public decimal? StorageCostRate { get; set; }
        public string StorageCostUnit { get; set; }
        public decimal? InsuranceCostRate { get; set; }
        public string InsuranceCostUnit { get; set; }
        public string BillingCycle { get; set; }
        public DateTime? FreePeriodStartDate { get; set; }
        public DateTime? FreePeriodEndDate { get; set; }
    }
}