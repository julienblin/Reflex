// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationTypeAppLineCriteria.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;

namespace CGI.Reflex.Core.Queries.Series.Criteria
{
    internal class ApplicationTypeAppLineCriteria : BaseAppLineCriteria
    {
        public ApplicationTypeAppLineCriteria()
            : base("ApplicationType")
        {
        }

        public override IEnumerable<Type> ExcludedColumnCriterias
        {
            get { yield return typeof(ApplicationTypeAppColumnCriteria); }
        }

        public override void ApplyJoins(IQueryOver<Application, Application> query)
        {
            query.JoinQueryOver(a => a.ApplicationType);
        }

        public override void SelectCountIfNoColumns(QueryOverProjectionBuilder<Application> list)
        {
            if (!OnlyActiveApplications)
            {
                list.SelectCount(a => a.Id).WithAlias(() => lineAlias.CumulativeCount);
            }
            else
            {
                var appStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                var subQuery = QueryOver.Of<Application>()
                                        .Where(a => a.ApplicationType == appAlias.ApplicationType)
                                        .JoinQueryOver(ai => ai.AppInfo)
                                        .WhereRestrictionOn(ai => ai.Status.Id).IsIn(appStatuses.ToArray())
                                        .SelectList(sublist => sublist.SelectCount(a => a.Id));
                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }

        public override void SelectGroup(QueryOverProjectionBuilder<Application> list)
        {
            list.SelectGroup(a => a.ApplicationType).WithAlias(() => lineAlias.LineCriteria);
        }

        public override IList<ApplicationSeriesResultLine> Order(IList<ApplicationSeriesResultLine> lines)
        {
            return lines.OrderBy(l => ((DomainValue)l.LineCriteria).DisplayOrder).ToList();
        }

        public override void ColumnCorrelation(QueryOver<Application, Application> subQuery)
        {
            subQuery.Where(a => a.ApplicationType == appAlias.ApplicationType);
        }

        public override void ColumnCorrelation(QueryOver<Application, AppInfo> appInfoQueryOver)
        {
            if (OnlyActiveApplications)
            {
                var appStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                appInfoQueryOver.WhereRestrictionOn(ai => ai.Status.Id).IsIn(appStatuses.ToArray());
            }
        }
    }
}
