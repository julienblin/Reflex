// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class IntegrationQuery : BaseQueryOver<Integration>
    {
        public int? AppSourceId { get; set; }

        public int? AppDestId { get; set; }

        public int? ApplicationId { get; set; }

        public int? LinkedTechnologyId { get; set; }

        public int? NatureId { get; set; }

        public string NameLike { get; set; }

        protected override IQueryOver<Integration, Integration> OverImpl(ISession session)
        {
            var query = session.QueryOver<Integration>();

            if (AppSourceId.HasValue)
                query.Where(i => i.AppSource.Id == AppSourceId.Value);

            if (AppDestId.HasValue)
                query.Where(i => i.AppDest.Id == AppDestId.Value);

            if (NatureId.HasValue)
                query.Where(i => i.Nature.Id == NatureId.Value);

            if (!string.IsNullOrEmpty(NameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Integration>(r => r.Name), NameLike, MatchMode.Anywhere));

            if (ApplicationId.HasValue)
                query.Where(i => i.AppDest.Id == ApplicationId.Value || i.AppSource.Id == ApplicationId.Value);

            if (LinkedTechnologyId.HasValue)
            {
                var allTechnoIds = new[] { LinkedTechnologyId.Value };
                var techno = new TechnologyHierarchicalQuery { RootId = LinkedTechnologyId }.SingleOrDefault(session);
                if (techno != null)
                    allTechnoIds = techno.AllIds.ToArray();

                var subIntTechnoLinksQuery = QueryOver.Of<IntegrationTechnoLink>()
                                                      .WhereRestrictionOn(itl => itl.Technology.Id).IsIn(allTechnoIds)
                                                      .SelectList(list => list.Select(adl => adl.Integration.Id));

                query.WithSubquery.WhereProperty(i => i.Id).In(subIntTechnoLinksQuery);
            }

            return query;
        }

        protected override bool OrderByCallback(IQueryOver<Integration, Integration> queryOver, string propertyName, OrderType orderType)
        {
            switch (propertyName)
            {
                case "AppSource.Name":
                    queryOver.JoinQueryOver(i => i.AppSource).OrderBy(a => a.Name, orderType);
                    queryOver.JoinQueryOver(i => i.AppDest).OrderBy(a => a.Name, OrderType.Asc);
                    return true;
                case "AppDest.Name":
                    queryOver.JoinQueryOver(i => i.AppDest).OrderBy(a => a.Name, orderType);
                    queryOver.JoinQueryOver(i => i.AppSource).OrderBy(a => a.Name, OrderType.Asc);
                    return true;
                case "Nature.Name":
                    queryOver.JoinQueryOver(i => i.Nature).OrderByDomainValue(orderType);
                    return true;
                default:
                    return false;
            }
        }
    }
}