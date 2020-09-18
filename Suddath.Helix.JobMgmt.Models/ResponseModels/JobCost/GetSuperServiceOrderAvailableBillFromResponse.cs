using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetSuperServiceOrderAvailableBillFromResponse
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string BillFromType { get; set; }
        public string Name { get; set; }
        public string ShortAddress { get; set; }
        public string Category { get; set; }
        public string Label { get; set; }

        public override bool Equals(object obj)
        {
            var other = (GetSuperServiceOrderAvailableBillFromResponse)obj;
            return this.Id == other.Id
                && this.Value == other.Value
                && this.BillFromType == other.BillFromType
                && this.Name == other.Name
                && this.Label == other.Label
                && this.Category == other.Category;
        }

        public override int GetHashCode()
        {
            var hashCode = -2105909248;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Label);
            return hashCode;
        }
    }
}