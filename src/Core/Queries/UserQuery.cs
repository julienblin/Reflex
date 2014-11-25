// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserQuery.cs" company="CGI">
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
    public class UserQuery : BaseQueryOver<User>
    {
        public int? Id { get; set; }

        public string NameOrEmail { get; set; }

        public string UserName { get; set; }

        public string UserNameLike { get; set; }

        public string NameOrEmailLike { get; set; }

        public int? RoleId { get; set; }

        public IEnumerable<int> RoleIds { get; set; }

        public IEnumerable<int> CompanyIds { get; set; }

        public bool? IsLockedOut { get; set; }

        protected override IQueryOver<User, User> OverImpl(ISession session)
        {
            var query = session.QueryOver<User>();

            if (Id.HasValue)
                query.Where(u => u.Id == Id.Value);

            if (!string.IsNullOrEmpty(NameOrEmail))
                query.Where(
                    Restrictions.Or(
                        Restrictions.InsensitiveLike(Projections.Property<User>(u => u.UserName), NameOrEmail, MatchMode.Exact),
                        Restrictions.InsensitiveLike(Projections.Property<User>(u => u.Email), NameOrEmail, MatchMode.Exact)));

            if (!string.IsNullOrEmpty(UserNameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<User>(u => u.UserName), UserNameLike, MatchMode.Anywhere));

            if (!string.IsNullOrEmpty(NameOrEmailLike))
                query.Where(
                    Restrictions.Or(
                        Restrictions.InsensitiveLike(Projections.Property<User>(u => u.UserName), NameOrEmailLike, MatchMode.Anywhere),
                        Restrictions.InsensitiveLike(Projections.Property<User>(u => u.Email), NameOrEmailLike, MatchMode.Anywhere)));

            if (!string.IsNullOrEmpty(UserName))
                query.Where(u => u.UserName == UserName);

            if (RoleId.HasValue)
                query.Where(u => u.Role.Id == RoleId.Value);

            if ((RoleIds != null) && RoleIds.Any())
                query.WhereRestrictionOn(u => u.Role.Id).IsIn(RoleIds.ToArray());

            if ((CompanyIds != null) && CompanyIds.Any())
                query.WhereRestrictionOn(u => u.Company.Id).IsIn(CompanyIds.ToArray());

            if (IsLockedOut.HasValue)
                query.Where(u => u.IsLockedOut == IsLockedOut.Value);

            return query;
        }

        protected override bool OrderByCallback(IQueryOver<User, User> queryOver, string propertyName, OrderType orderType)
        {
            switch (propertyName)
            {
                case "Role.Name":
                    queryOver.JoinQueryOver(u => u.Role).OrderBy(r => r.Name, orderType);
                    return true;
                default:
                    return false;
            }
        }
    }
}