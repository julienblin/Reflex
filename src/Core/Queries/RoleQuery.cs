// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class RoleQuery : BaseQueryOver<Role>
    {
        public string NameLike { get; set; }

        protected override IQueryOver<Role, Role> OverImpl(ISession session)
        {
            var query = session.QueryOver<Role>();

            if (!string.IsNullOrEmpty(NameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Role>(r => r.Name), NameLike, MatchMode.Start));

            return query;
        }
    }
}