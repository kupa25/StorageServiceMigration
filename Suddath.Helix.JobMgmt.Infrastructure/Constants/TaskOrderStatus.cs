using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public class TaskOrderStatus : ComparableConstant<TaskOrderStatus>
    {
        public static string ACTIVE = "Active";
        public static string INACTIVE = "Inactive";
    }
}