// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRangeAppColumnCriteria.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using CGI.Reflex.Core.Entities;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;

namespace CGI.Reflex.Core.Queries.Series.Criteria
{
    internal class UserRangeAppColumnCriteria : BaseAppColumnCriteria
    {
        public UserRangeAppColumnCriteria()
            : base("ApplicationUserRange")
        {
        }

        public override IList<object> GetColumns()
        {
            var ranges = new DomainValueQuery { Category = DomainValueCategory.ApplicationUserRange }.OrderBy(dv => dv.DisplayOrder).List().Cast<object>().ToList();
            ranges.Add(null);
            return ranges;
        }

        public override void SelectCounts(QueryOverProjectionBuilder<Application> list, IList<object> columns, IAppLineCriteria lineCriteria)
        {
            foreach (DomainValue range in columns)
            {
                var subQuery = QueryOver.Of<Application>();

                lineCriteria.ColumnCorrelation(subQuery);

                var appInfoQueryOver = subQuery.JoinQueryOver(a => a.AppInfo);
                lineCriteria.ColumnCorrelation(appInfoQueryOver);
                appInfoQueryOver.Where(ai => ai.UserRange == range);

                subQuery.SelectList(sublist => sublist.SelectCount(a => a.Id));

                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }
    }
}
