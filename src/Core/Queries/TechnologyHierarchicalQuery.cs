// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyHierarchicalQuery.cs" company="CGI">
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
    public class TechnologyHierarchicalQuery : BaseHierarchicalQueryOver<Technology>
    {
        public TechnologyHierarchicalQuery()
            : base(5)
        {
        }

        public int? RootId { get; set; }

        public IEnumerable<int> RootIds { get; set; }

        protected override IQueryOver<Technology, Technology> OverImpl(ISession session)
        {
            var query = session.QueryOver<Technology>();

            if (RootIds != null && RootIds.Any())
                query.WhereRestrictionOn(t => t.Id).IsIn(RootIds.ToArray());
            
            if (RootId.HasValue)
                query.Where(t => t.Id == RootId.Value);

            if (!RootId.HasValue && RootIds == null)
                query.Where(t => t.Parent == null);

            query.OrderBy(t => t.Name).Asc();
            return query;
        }
    }
}
