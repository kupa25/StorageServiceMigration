using System;
using System.Collections.Generic;
using System.Text;

namespace StorageServiceMigration
{
    public static class StringExtension
    {
        public static string Format(this string s)
        {
            if (s == null)
            {
                s = " ";
            }

            return s;
        }
    }
}