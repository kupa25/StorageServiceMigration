using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.Accounting
{
    public class GetGLCodeResponse
    {
        public string GLCode { get; set; }
        public string ReceivableOrPayable { get; set; }
        public string AccrualOrActual { get; set; }
        public string BranchName { get; set; }
        public string RevenueType { get; set; }
        public string AccountGroupCode { get; set; }
        public string AccountCode { get; set; }
        public string AccountCodeName { get; set; }
        public int GPDistType { get; set; }
        public string GPDistTypeMeaning { get; set; }
    }
}