// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppColumnCriteria.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using CGI.Reflex.Core.Entities;
using NHibernate.Criterion.Lambda;

namespace CGI.Reflex.Core.Queries.Series.Criteria
{
    internal interface IAppColumnCriteria
    {
        string DisplayName { get; }

        string TechnicalName { get; }

        IList<object> GetColumns();

        void SelectCounts(QueryOverProjectionBuilder<Application> list, IList<object> columns, IAppLineCriteria lineCriteria);
    }
}
