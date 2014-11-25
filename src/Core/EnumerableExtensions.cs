// -----------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="CGI">
//  Copyright (c) CGI. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core
{
    public static class EnumerableExtensions
    {
        public static int FindLeastCommonMultiple(this IEnumerable<int> values)
        {
            var curratedValues = values.Distinct().Where(x => x != 0).ToList();

            if (!curratedValues.Any())
                throw new InvalidOperationException("Unable to find least common multiple against no value or only zeros.");

            if (curratedValues.Count() == 1)
                return curratedValues.First();

            var maxNumber = curratedValues.Max();

            var prod = curratedValues.Aggregate<int, long>(1, (current, value) => current * value);

            for (var i = maxNumber; i <= prod; i += maxNumber)
            {
                if (curratedValues.Count(value => i % value == 0) == curratedValues.Count())
                    return i;
            }

            throw new Exception("Error in Least Common Multiple Algorithm.");
        }

        public static PaginationResult<T> Paginate<T>(this IEnumerable<T> values, int page = 1, int pageSize = 20)
        {
            var totalItems = values.Count();
            var paginatedResults = values.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PaginationResult<T>
            {
                CurrentPage = page,
                Items = paginatedResults,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
