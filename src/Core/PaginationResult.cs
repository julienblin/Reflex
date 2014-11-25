// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaginationResult.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace CGI.Reflex.Core
{
    public class PaginationResult<T> : IPaginationResult
    {
        public int TotalItems { get; set; }

        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount
        {
            get
            {
                return (TotalItems / PageSize) + (TotalItems % PageSize == 0 ? 0 : 1);
            }
        }

        public bool HasPreviousPage
        {
            get { return CurrentPage > 1; }
        }

        public bool HasNextPage
        {
            get { return CurrentPage < PageCount; }
        }

        public bool IsFirstPage
        {
            get { return CurrentPage <= 1; }
        }

        public bool IsLastPage
        {
            get { return CurrentPage >= PageCount; }
        }

        public IEnumerable<T> Items { get; set; }
    }
}