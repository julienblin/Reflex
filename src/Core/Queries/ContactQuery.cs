// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactQuery.cs" company="CGI">
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
    public class ContactQuery : BaseQueryOver<Contact>
    {
        public string LastNameLike { get; set; }

        public string FirstNameLike { get; set; }

        public int? TypeId { get; set; }

        public int? SectorId { get; set; }

        protected override NHibernate.IQueryOver<Contact, Contact> OverImpl(NHibernate.ISession session)
        {
            var query = session.QueryOver<Contact>();

            if (!string.IsNullOrEmpty(LastNameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Contact>(r => r.LastName), LastNameLike, MatchMode.Anywhere));

            if (!string.IsNullOrEmpty(FirstNameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Contact>(r => r.FirstName), FirstNameLike, MatchMode.Anywhere));

            if (TypeId.HasValue)
                query.Where(c => c.Type.Id == TypeId.Value);

            if (SectorId.HasValue)
                query.Where(c => c.Sector.Id == SectorId.Value);

            return query;
        }

        protected override bool OrderByCallback(NHibernate.IQueryOver<Contact, Contact> queryOver, string propertyName, OrderType orderType)
        {
            switch (propertyName)
            {
                 case "FullName":
                    queryOver.OrderBy(c => c.LastName, orderType).OrderBy(c => c.FirstName, orderType).OrderBy(c => c.Company, orderType);
                    return true;
                 case "Type.Name":
                    queryOver.Left.JoinQueryOver(a => a.Type).OrderBy(dv => dv.Name, orderType);
                    return true;
                 case "Sector.Name":
                    queryOver.Left.JoinQueryOver(a => a.Sector).OrderBy(s => s.Name, orderType);
                    return true;
                 default:
                    return false;
            }
        }
    }
}
