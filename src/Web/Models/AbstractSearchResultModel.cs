// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractSearchResultModel.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CGI.Reflex.Core;
using CGI.Reflex.Core.Queries;

namespace CGI.Reflex.Web.Models
{
    public abstract class AbstractSearchResultModel
    {
        protected AbstractSearchResultModel()
        {
            Page = 1;
        }

        public int Page { get; set; }

        public string OrderBy { get; set; }

        public OrderType OrderType { get; set; }

        public virtual bool HasResults
        {
            get { return GetPaginationResults() != null && GetPaginationResults().TotalItems > 0; }
        }

        public abstract IPaginationResult GetPaginationResults();
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class AbstractSearchResultModel<T> : AbstractSearchResultModel
    {
        public PaginationResult<T> SearchResults { get; set; }

        public override IPaginationResult GetPaginationResults()
        {
            return SearchResults;
        }
    }
}