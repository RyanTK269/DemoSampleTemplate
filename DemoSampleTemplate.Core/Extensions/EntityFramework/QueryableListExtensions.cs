using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoSampleTemplate.Core.Extensions.EntityFramework
{
    public static class QueryableListExtensions
    {
        public static List<T> ToSubList<T>(this IQueryable<T> source, int amount = 0, int offset = 1)
        {
            if (amount > 0)
            {
                return source.Skip((offset - 1) * amount).Take(amount).ToList();
            }
            return source.ToList();
        }

        public static async Task<List<T>> ToSubListAsync<T>(this IQueryable<T> source, int amount = 0, int offset = 1)
        {
            if (amount > 0)
            {
                return await source.Skip((offset - 1) * amount).Take(amount).ToListAsync();
            }
            return await source.ToListAsync();
        }
    }
}
