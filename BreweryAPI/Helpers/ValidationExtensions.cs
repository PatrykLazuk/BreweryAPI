using System;
using System.Collections.Generic;
using System.Linq;
using BreweryAPI.Models;

namespace BreweryAPI.Helpers
{
    public static class ValidationExtensions
    {
        public static bool IsValidSortOption(this string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return true;

            var validSortOptions = Enum.GetNames(typeof(SortOption)).Select(s => s.ToLower());
            return validSortOptions.Contains(sortBy.ToLower());
        }

        public static bool RequiresCoordinates(this string? sortBy)
        {
            return sortBy?.ToLower() == SortOption.Distance.ToString().ToLower();
        }

        public static bool HasValidCoordinates(this (double? lat, double? lng) coordinates)
        {
            return coordinates.lat.HasValue && coordinates.lng.HasValue;
        }

        public static bool IsValidPagination(this int page, int pageSize)
        {
            return page >= Constants.Pagination.DefaultPage && 
                   pageSize >= Constants.Pagination.MinPageSize && 
                   pageSize <= Constants.Pagination.MaxPageSize;
        }

        public static (int page, int pageSize) NormalizePagination(this (int page, int pageSize) pagination)
        {
            var normalizedPage = pagination.page < Constants.Pagination.DefaultPage ? Constants.Pagination.DefaultPage : pagination.page;
            var normalizedPageSize = pagination.pageSize < Constants.Pagination.MinPageSize ? Constants.Pagination.MinPageSize : 
                                   pagination.pageSize > Constants.Pagination.MaxPageSize ? Constants.Pagination.MaxPageSize : pagination.pageSize;
            
            return (normalizedPage, normalizedPageSize);
        }
    }
}
