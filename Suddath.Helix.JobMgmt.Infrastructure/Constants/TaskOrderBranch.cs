using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public class TaskOrderBranch : ComparableConstant<TaskOrderBranch>
    {
        public static string ARMY = "Army";
        public static string NAVY = "Navy";
        public static string MARINES = "Marines";
        public static string AIR_FORCE = "Air Force";
        public static string SPACE_FORCE = "Space Force";
        public static string COAST_GUARD = "Coast Guard";
    }
}