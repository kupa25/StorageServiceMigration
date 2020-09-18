using System;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class TransfereeFlatDto
    {
        public int TransfereeId { get; set; }
        public string AccountingId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string AdditionalAddressInfo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}