using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models
{
    public class VendorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Accounting_SI_Code { get; set; }
        public string Accounting_MIL_Code { get; set; }
        public bool IsActive { get; set; }
        public List<VendorAddressDTO> VendorAddress { get; set; }
    }

    public class VendorAddressDTO
    {
        public int Id { get; set; }
        public NameIdDto VendorAddressType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string AdditionalAddressInfo { get; set; }
        public string Type { get; set; }

        public NameIdDto State { get; set; }
        public NameIdDto Country { get; set; }
    }
}