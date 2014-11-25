// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationTypeAppColumnCriteria.cs" company="CGI">
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
    internal class ApplicationTypeAppColumnCriteria : BaseAppColumnCriteria
    {
        public ApplicationTypeAppColumnCriteria()
            : base("ApplicationType")
        {
        }

        public override IList<object> GetColumns()
        {
            return new DomainValueQuery { Category = DomainValueCategory.ApplicationType }.OrderBy(dv => dv.DisplayOrder).List().Cast<object>().ToList();
        }

        public override void SelectCounts(QueryOverProjectionBuilder<Application> list, IList<object> columns, IAppLineCriteria lineCriteria)
        {
            foreach (DomainValue appType in columns)
            {
                var subQuery = QueryOver.Of<Application>();

                lineCriteria.ColumnCorrelation(subQuery);

                var appInfoQueryOver = subQuery.JoinQueryOver(a => a.AppInfo);
                lineCriteria.ColumnCorrelation(appInfoQueryOver);

                subQuery.Where(a => a.ApplicationType == appType);
                subQuery.SelectList(sublist => sublist.SelectCount(a => a.Id));

                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }
    }
}
