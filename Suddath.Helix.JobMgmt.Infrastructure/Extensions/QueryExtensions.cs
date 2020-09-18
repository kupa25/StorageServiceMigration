using Helix.API.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure
{
    public static class QueryExtensions
    {
        public static IQueryable<T> PageResults<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;

            return source.Skip(skip).Take(pageSize);
        }
    }
}
