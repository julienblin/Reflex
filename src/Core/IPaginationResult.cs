// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPaginationResult.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CGI.Reflex.Core
{
    public interface IPaginationResult
    {
        int TotalItems { get; }

        int PageSize { get; }

        int PageCount { get; }

        int CurrentPage { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }

        bool IsFirstPage { get; }

        bool IsLastPage { get; }
    }
}