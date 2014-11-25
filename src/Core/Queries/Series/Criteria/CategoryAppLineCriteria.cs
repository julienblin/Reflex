// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryAppLineCriteria.cs" company="CGI">
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
    internal class CategoryAppLineCriteria : BaseAppLineCriteria
    {
        public CategoryAppLineCriteria()
            : base("ApplicationCategory")
        {
        }

        public override IEnumerable<Type> ExcludedColumnCriterias
        {
            get { yield break; }
        }

        public override void ApplyJoins(IQueryOver<Application, Application> query)
        {
            AppInfo appInfoAlias = null;
            query.JoinQueryOver(a => a.AppInfo, () => appInfoAlias).JoinQueryOver(ai => ai.Category);
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
                                        .And(ai => ai.Category == appInfoAlias.Category)
                                        .SelectList(sublist => sublist.SelectCount(a => a.Id));
                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }

        public override void SelectGroup(QueryOverProjectionBuilder<Application> list)
        {
            AppInfo appInfoAlias = null;
            list.SelectGroup(a => appInfoAlias.Category).WithAlias(() => lineAlias.LineCriteria);
        }

        public override IList<ApplicationSeriesResultLine> Order(IList<ApplicationSeriesResultLine> lines)
        {
            return lines.OrderBy(l => l.LineCriteria == null ? int.MaxValue : ((DomainValue)l.LineCriteria).DisplayOrder).ToList();
        }

        public override void ColumnCorrelation(QueryOver<Application, AppInfo> appInfoQueryOver)
        {
            AppInfo appInfoAlias = null;
            appInfoQueryOver.Where(ai => ai.Category == appInfoAlias.Category);
            if (OnlyActiveApplications)
            {
                var appStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                appInfoQueryOver.WhereRestrictionOn(ai => ai.Status.Id).IsIn(appStatuses.ToArray());
            }
        }
    }
}
