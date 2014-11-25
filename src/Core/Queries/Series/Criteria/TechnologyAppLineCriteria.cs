// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyAppLineCriteria.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.SqlCommand;

namespace CGI.Reflex.Core.Queries.Series.Criteria
{
    internal class TechnologyAppLineCriteria : BaseAppLineCriteria
    {
        public TechnologyAppLineCriteria()
            : base("Technology")
        {
        }

        public override LineMultiplicities LineMultiplicities { get { return LineMultiplicities.ManyToOne; } }

        public override void ApplyJoins(IQueryOver<Application, Application> query)
        {
            AppTechnoLink appTechnoLinkAlias = null;
            query.JoinAlias(a => a.TechnologyLinks, () => appTechnoLinkAlias, JoinType.InnerJoin);
        }

        public override void SelectCountIfNoColumns(QueryOverProjectionBuilder<Application> list)
        {
            if (!OnlyActiveApplications)
            {
                list.SelectCount(a => a.Id).WithAlias(() => lineAlias.CumulativeCount);
            }
            else
            {
                AppTechnoLink appTechnoLinkAlias = null;

                var atlSubQuery = QueryOver.Of<AppTechnoLink>()
                                           .Where(atl => atl.Technology.Id == appTechnoLinkAlias.Technology.Id)
                                           .Select(atl => atl.Application.Id);

                var appStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                var subQuery = QueryOver.Of<Application>()
                                        .WithSubquery.WhereProperty(a => a.Id).In(atlSubQuery)
                                        .JoinQueryOver(ai => ai.AppInfo)
                                        .WhereRestrictionOn(ai => ai.Status.Id).IsIn(appStatuses.ToArray())
                                        .SelectList(sublist => sublist.SelectCount(a => a.Id));
                list.SelectSubQuery(subQuery).WithAlias(() => lineAlias.CumulativeCount);
            }
        }

        public override void SelectGroup(QueryOverProjectionBuilder<Application> list)
        {
            AppTechnoLink appTechnoLinkAlias = null;
            list.SelectGroup(a => appTechnoLinkAlias.Technology).WithAlias(() => lineAlias.LineCriteria);
        }

        public override IList<ApplicationSeriesResultLine> Order(IList<ApplicationSeriesResultLine> lines)
        {
            // Let's eager load technologies
            new TechnologyHierarchicalQuery().List();
            return lines;
        }

        public override void ColumnCorrelation(QueryOver<Application, Application> subQuery)
        {
            AppTechnoLink appTechnoLinkAlias = null;

            var atlSubQuery = QueryOver.Of<AppTechnoLink>()
                                       .Where(atl => atl.Technology.Id == appTechnoLinkAlias.Technology.Id)
                                       .Select(atl => atl.Application.Id);

            subQuery.WithSubquery.WhereProperty(a => a.Id).In(atlSubQuery);
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
