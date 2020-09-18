using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public static class ServiceId
    {
        public static int AIR_ORIGIN_AGENT = 1;
        public static int AIR_AIR_FREIGHT = 3;
        public static int AIR_JC = 8;

        public static int OCEAN_ORIGIN_AGENT = 9;
        public static int OCEAN_OCEAN_FREIGHT = 11;
        public static int OCEAN_JC = 16;

        public static int ROAD_ORIGIN_AGENT = 17;
        public static int ROAD_FREIGHT = 31;
        public static int ROAD_JC = 23;

        public static int STORAGE_ORIGIN_AGENT = 24;
        public static int STORAGE_DESTINATION_AGENT = 26;
        public static int STORAGE_JC = 29;
        public static int STORAGE_STORAGE = 32;
    }
}