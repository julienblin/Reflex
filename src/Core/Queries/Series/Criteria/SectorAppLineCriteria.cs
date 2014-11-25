// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorAppLineCriteria.cs" company="CGI">
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
    internal class SectorAppLineCriteria : BaseAppLineCriteria
    {
        public SectorAppLineCriteria()
            : base("Sector")
        {
        }

        public override IEnumerable<Type> ExcludedColumnCriterias
        {
            get { yield break; }
        }

        public override void ApplyJoins(IQueryOver<Application, Application> query)
        {
            AppInfo appInfoAlias = null;
            query.JoinQueryOver(a => a.AppInfo, () => appInfoAlias).JoinQueryOver(ai => ai.Sector);
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
                                        .And(ai => ai.Sector == appInfoAlias.Sector)
                                        .SelectList(sublist => sublist.SelectCount(a => a.Id));
                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }

        public override void SelectGroup(QueryOverProjectionBuilder<Application> list)
        {
            AppInfo appInfoAlias = null;
            list.SelectGroup(a => appInfoAlias.Sector).WithAlias(() => lineAlias.LineCriteria);
        }

        public override IList<ApplicationSeriesResultLine> Order(IList<ApplicationSeriesResultLine> lines)
        {
            return lines.OrderBy(l => l.LineCriteria == null ? string.Empty : ((Sector)l.LineCriteria).Name).ToList();
        }

        public override void ColumnCorrelation(QueryOver<Application, AppInfo> appInfoQueryOver)
        {
            AppInfo appInfoAlias = null;
            appInfoQueryOver.Where(ai => ai.Sector == appInfoAlias.Sector);
            if (OnlyActiveApplications)
            {
                var appStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                appInfoQueryOver.WhereRestrictionOn(ai => ai.Status.Id).IsIn(appStatuses.ToArray());
            }
        }
    }
}
