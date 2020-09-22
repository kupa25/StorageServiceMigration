using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.Constant
{
    public static class FreePeriodDate
    {
        public static DateTime? StartDate(int? value, int year)
        {
            DateTime? result;
            switch (value)
            {
                case 1:
                    result = new DateTime(year, 01, 01);
                    break;

                case 2:
                    result = new DateTime(year, 02, 01);
                    break;

                case 3:
                    result = new DateTime(year, 03, 01);
                    break;

                case 4:
                    result = new DateTime(year, 04, 01);
                    break;

                case 5:
                    result = new DateTime(year, 05, 01);
                    break;

                case 6:
                    result = new DateTime(year, 06, 01);
                    break;

                case 7:
                    result = new DateTime(year, 07, 01);
                    break;

                case 8:
                    result = new DateTime(year, 08, 01);
                    break;

                case 9:
                    result = new DateTime(year, 09, 01);
                    break;

                case 10:
                    result = new DateTime(year, 10, 01);
                    break;

                case 11:
                    result = new DateTime(year, 11, 01);
                    break;

                case 12:
                    result = new DateTime(year, 12, 01);
                    break;

                default:
                    result = null;
                    break;
            }

            return result;
        }

        public static DateTime? EndDate(int? value, int year)
        {
            DateTime? result;
            switch (value)
            {
                case 1:
                    result = new DateTime(year, 01, 01);
                    break;

                case 2:
                    result = new DateTime(year, 02, 01);
                    break;

                case 3:
                    result = new DateTime(year, 03, 01);
                    break;

                case 4:
                    result = new DateTime(year, 04, 01);
                    break;

                case 5:
                    result = new DateTime(year, 05, 01);
                    break;

                case 6:
                    result = new DateTime(year, 06, 01);
                    break;

                case 7:
                    result = new DateTime(year, 07, 01);
                    break;

                case 8:
                    result = new DateTime(year, 08, 01);
                    break;

                case 9:
                    result = new DateTime(year, 09, 01);
                    break;

                case 10:
                    result = new DateTime(year, 10, 01);
                    break;

                case 11:
                    result = new DateTime(year, 11, 01);
                    break;

                case 12:
                    result = new DateTime(year, 12, 01);
                    break;

                default:
                    result = null;
                    break;
            }

            return result;
        }
    }
}