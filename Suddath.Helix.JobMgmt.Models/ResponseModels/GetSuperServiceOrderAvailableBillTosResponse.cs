using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class GetSuperServiceOrderAvailableBillTosResponse
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string BillToType { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Label { get; set; }

        public override bool Equals(object obj)
        {
            var other = (GetSuperServiceOrderAvailableBillTosResponse)obj;
            return this.Id == other.Id
                && this.Value == other.Value
                && this.BillToType == other.BillToType
                && this.Name == other.Name
                && this.Label == other.Label
                && this.Category == other.Category;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Id) ? Id.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Value) ? Value.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, BillToType) ? BillToType.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Name) ? Name.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Category) ? Category.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Label) ? Label.GetHashCode() : 0);
                return hash;
            }
        }
    }
}