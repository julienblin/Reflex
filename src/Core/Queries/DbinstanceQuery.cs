// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class DbInstanceQuery : BaseQueryOver<DbInstance>
    {
        public string NameLike { get; set; }

        public int? AssociatedWithApplicationId { get; set; }

        public int? NotAssociatedWithApplicationId { get; set; }

        public int? ServerId { get; set; }

        protected override NHibernate.IQueryOver<DbInstance, DbInstance> OverImpl(NHibernate.ISession session)
        {
            var query = session.QueryOver<DbInstance>();

            if (AssociatedWithApplicationId.HasValue)
            {
                var subDbInstanceIdAlreadyAssociatedQuery = QueryOver.Of<AppDbInstanceLink>()
                                                                   .Where(adl => adl.Application.Id == AssociatedWithApplicationId.Value)
                                                                   .SelectList(list => list.Select(adl => adl.DbInstances.Id));
                query.WithSubquery.WhereProperty(d => d.Id).In(subDbInstanceIdAlreadyAssociatedQuery);
            }

            if (NotAssociatedWithApplicationId.HasValue)
            {
                var subDbInstanceIdAlreadyAssociatedQuery = QueryOver.Of<AppDbInstanceLink>()
                                                                   .Where(adl => adl.Application.Id == NotAssociatedWithApplicationId.Value)
                                                                   .SelectList(list => list.Select(adl => adl.DbInstances.Id));
                query.WithSubquery.WhereProperty(d => d.Id).NotIn(subDbInstanceIdAlreadyAssociatedQuery);
            }

            if (!string.IsNullOrEmpty(NameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<DbInstance>(d => d.Name), NameLike, MatchMode.Anywhere));

            if (ServerId.HasValue)
                query.Where(d => d.Server.Id == ServerId.Value);

            return query;
        }
    }
}
