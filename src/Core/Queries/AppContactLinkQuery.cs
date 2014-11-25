// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContactLinkQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.SqlCommand;

namespace CGI.Reflex.Core.Queries
{
    public class AppContactLinkQuery : BaseQueryOver<AppContactLink>
    {
        public int ApplicationId { get; set; }

        protected override IQueryOver<AppContactLink, AppContactLink> OverImpl(ISession session)
        {
            var query = session.QueryOver<AppContactLink>();

            query.Where(cl => cl.Application.Id == ApplicationId);

            return query;
        }

        protected override bool OrderByCallback(IQueryOver<AppContactLink, AppContactLink> queryOver, string propertyName, OrderType orderType)
        {
            switch (propertyName)
            {
                case "FullName":
                    queryOver.JoinQueryOver(cl => cl.Contact).OrderBy(c => c.LastName, orderType).OrderBy(c => c.FirstName, orderType).OrderBy(c => c.Company, orderType);
                    return true;
                case "Type.Order":
                    queryOver.JoinQueryOver(cl => cl.Contact).JoinQueryOver(c => c.Type).OrderBy(t => t.DisplayOrder, orderType);
                    return true;
                case "Sector.Name":
                    queryOver.JoinQueryOver(cl => cl.Contact).JoinQueryOver(c => c.Sector, JoinType.LeftOuterJoin).OrderBy(s => s.Name, orderType);
                    return true;
                default:
                    return false;
            }
        }
    }
}
