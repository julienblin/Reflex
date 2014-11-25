// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusAppLineCriteria.cs" company="CGI">
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
    internal class StatusAppLineCriteria : BaseAppLineCriteria
    {
        public StatusAppLineCriteria()
            : base("ApplicationStatus")
        {
        }

        public override IEnumerable<Type> ExcludedColumnCriterias
        {
            get { yield return typeof(StatusAppColumnCriteria); }
        }

        public override void ApplyJoins(IQueryOver<Application, Application> query)
        {
            AppInfo appInfoAlias = null;
            query.JoinQueryOver(a => a.AppInfo, () => appInfoAlias).JoinQueryOver(ai => ai.Status);
        }

        public override void SelectCountIfNoColumns(QueryOverProjectionBuilder<Application> list)
        {
            if (!OnlyActiveApplications)
            {
                list.SelectCount(a => a.Id).WithAlias(() => lineAlias.CumulativeCount);
            }
            else
            {
                AppInfo appInfoAlias = null;
                var appStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                var subQuery = QueryOver.Of<Application>()
                                        .JoinQueryOver(ai => ai.AppInfo)
                                        .WhereRestrictionOn(ai => ai.Status.Id).IsIn(appStatuses.ToArray())
                                        .And(ai => ai.Status == appInfoAlias.Status)
                                        .SelectList(sublist => sublist.SelectCount(a => a.Id));
                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }

        public override void SelectGroup(QueryOverProjectionBuilder<Application> list)
        {
            AppInfo appInfoAlias = null;
            list.SelectGroup(a => appInfoAlias.Status).WithAlias(() => lineAlias.LineCriteria);
        }

        public override IList<ApplicationSeriesResultLine> Order(IList<ApplicationSeriesResultLine> lines)
        {
            return lines.OrderBy(l => ((DomainValue)l.LineCriteria).DisplayOrder).ToList();
        }

        public override void ColumnCorrelation(QueryOver<Application, AppInfo> appInfoQueryOver)
        {
            AppInfo appInfoAlias = null;
            appInfoQueryOver.Where(ai => ai.Status == appInfoAlias.Status);
            if (OnlyActiveApplications)
            {
                var appStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                appInfoQueryOver.WhereRestrictionOn(ai => ai.Status.Id).IsIn(appStatuses.ToArray());
            }
        }
    }
}
