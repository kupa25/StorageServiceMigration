using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public class ComparableConstant<T>
    {
        public static bool Contains(string obj)
        {
            return typeof(T).GetFields(BindingFlags.Public)
                .Any(f => f.GetValue(f).Equals(obj));
        }
    }
}