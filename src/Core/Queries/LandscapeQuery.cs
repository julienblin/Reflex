// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapeQuery.cs" company="CGI">
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
    public class LandscapeQuery : BaseQueryOver<Landscape>
    {
        public string NameLike { get; set; }

        protected override NHibernate.IQueryOver<Landscape, Landscape> OverImpl(NHibernate.ISession session)
        {
            var query = session.QueryOver<Landscape>();

            if (!string.IsNullOrEmpty(NameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Landscape>(l => l.Name), NameLike, MatchMode.Anywhere));

            return query;
        }
    }        
}
