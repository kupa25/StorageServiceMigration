using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string of words (the Red cat) to Title Case
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CovertToTitleCase(this string s) =>
            CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s);

        public static string ToUpperClean(this string input)
        {
            return input.ToUpper().Trim();
        }

        public static string Format(this string input)
        {
            if (input != null)
            {
                return input.ToLower().Trim();
            }
            return input;
        }
    }
}