// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusAppColumnCriteria.cs" company="CGI">
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
    internal class StatusAppColumnCriteria : BaseAppColumnCriteria
    {
        public StatusAppColumnCriteria()
            : base("ApplicationStatus")
        {
        }

        public override IList<object> GetColumns()
        {
            var statuses = new DomainValueQuery { Category = DomainValueCategory.ApplicationStatus }.OrderBy(dv => dv.DisplayOrder).List().Cast<object>().ToList();
            statuses.Add(null);
            return statuses;
        }

        public override void SelectCounts(QueryOverProjectionBuilder<Application> list, IList<object> columns, IAppLineCriteria lineCriteria)
        {
            foreach (DomainValue status in columns)
            {
                var subQuery = QueryOver.Of<Application>();

                lineCriteria.ColumnCorrelation(subQuery);

                var appInfoQueryOver = subQuery.JoinQueryOver(a => a.AppInfo);
                lineCriteria.ColumnCorrelation(appInfoQueryOver);
                appInfoQueryOver.Where(ai => ai.Status == status);

                subQuery.SelectList(sublist => sublist.SelectCount(a => a.Id));

                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }
    }
}
