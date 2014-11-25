// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyQuery.cs" company="CGI">
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

namespace CGI.Reflex.Core.Queries
{
    public class TechnologyQuery : BaseQueryOver<Technology>
    {
        public string NameLike { get; set; }

        public int? TechnologyTypeId { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? ContactId { get; set; }

        protected override IQueryOver<Technology, Technology> OverImpl(ISession session)
        {
            var query = session.QueryOver<Technology>();

            if (!string.IsNullOrEmpty(NameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Technology>(t => t.Name), NameLike, MatchMode.Anywhere));

            if (TechnologyTypeId.HasValue)
                query.Where(t => t.TechnologyType.Id == TechnologyTypeId.Value);

            if (DateFrom.HasValue)
                query.Where(t => t.EndOfSupport >= DateFrom.Value);

            if (DateTo.HasValue)
                query.Where(t => t.EndOfSupport <= DateTo.Value);

            if (ContactId.HasValue)
                query.Where(t => t.Contact.Id == ContactId.Value);

            return query;
        }
    }
}
