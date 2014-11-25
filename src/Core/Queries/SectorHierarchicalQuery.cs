// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorHierarchicalQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace CGI.Reflex.Core.Queries
{
    public class SectorHierarchicalQuery : BaseHierarchicalQueryOver<Sector>
    {
        public int? RootId { get; set; }

        protected override IQueryOver<Sector, Sector> OverImpl(ISession session)
        {
            var query = session.QueryOver<Sector>();

            if (RootId.HasValue)
                query.Where(s => s.Id == RootId.Value);
            else
                query.Where(s => s.Parent == null);

            query.OrderBy(s => s.Name).Asc();
            return query;
        }
    }
}
