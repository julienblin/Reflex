// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CriticityAppColumnCriteria.cs" company="CGI">
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
    internal class CriticityAppColumnCriteria : BaseAppColumnCriteria
    {
        public CriticityAppColumnCriteria()
            : base("ApplicationCriticity")
        {
        }

        public override IList<object> GetColumns()
        {
            var criticities = new DomainValueQuery { Category = DomainValueCategory.ApplicationCriticity }.OrderBy(dv => dv.DisplayOrder).List().Cast<object>().ToList();
            criticities.Add(null);
            return criticities;
        }

        public override void SelectCounts(QueryOverProjectionBuilder<Application> list, IList<object> columns, IAppLineCriteria lineCriteria)
        {
            foreach (DomainValue criticity in columns)
            {
                var subQuery = QueryOver.Of<Application>();

                lineCriteria.ColumnCorrelation(subQuery);

                var appInfoQueryOver = subQuery.JoinQueryOver(a => a.AppInfo);
                lineCriteria.ColumnCorrelation(appInfoQueryOver);
                appInfoQueryOver.Where(ai => ai.Criticity == criticity);

                subQuery.SelectList(sublist => sublist.SelectCount(a => a.Id));

                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }
    }
}
