using System;
using System.Collections.Generic;
using System.Linq;
using BreweryAPI.Models;

namespace BreweryAPI.Helpers
{
    public static class PagedResultExtensions
    {
        public static PagedResult<T> CreatePagedResult<T>(this IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            return new PagedResult<T>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = items
            };
        }

        public static IEnumerable<T> ApplyPagination<T>(this IEnumerable<T> items, int page, int pageSize)
        {
            return items.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
