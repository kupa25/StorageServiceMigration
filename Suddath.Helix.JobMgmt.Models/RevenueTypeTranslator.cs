using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public static class RevenueTypeTranslator
    {
        public static Dictionary<string, string> repo = new Dictionary<string, string>
        {
            {"HOUSE ACCOUNT","House Account" },
            {"OVERSEAS AGENT", "Overseas Agent"},
            {"SHIPPER DIRECT", "Shipper Direct" },
            {"SUDDATH", "Suddath" },
            {"THIRD PARTY","RMC / Third Party" }
        };
    }
}