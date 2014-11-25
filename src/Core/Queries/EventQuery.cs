// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class EventQuery : BaseQueryOver<Event>
    {
        public int? ApplicationId { get; set; }

        public string SourceLike { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? TypeId { get; set; }

        public string DescriptionLike { get; set; }

        protected override IQueryOver<Event, Event> OverImpl(ISession session)
        {
            var query = session.QueryOver<Event>();

            if (ApplicationId.HasValue)
                query.Where(e => e.Application.Id == ApplicationId.Value);

            if (!string.IsNullOrEmpty(SourceLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Event>(e => e.Source), SourceLike, MatchMode.Anywhere));

            if (DateFrom.HasValue)
                query.Where(e => e.Date >= DateFrom.Value);

            if (DateTo.HasValue)
                query.Where(e => e.Date <= DateTo.Value);

            if (TypeId.HasValue)
                query.Where(e => e.Type.Id == TypeId.Value);

            if (!string.IsNullOrEmpty(DescriptionLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Event>(e => e.Description), DescriptionLike, MatchMode.Anywhere));

            return query;
        }

        protected override bool OrderByCallback(IQueryOver<Event, Event> queryOver, string propertyName, OrderType orderType)
        {
            switch (propertyName)
            {
                case "EventType.Name":
                    queryOver.JoinQueryOver(a => a.Type).OrderByDomainValue(orderType);
                    queryOver.OrderBy(a => a.Date, OrderType.Desc);
                    return true;
                case "Source":
                    queryOver.OrderBy(a => a.Source, orderType).OrderBy(a => a.Date, OrderType.Desc);
                    return true;
                default:
                    return false;
            }
        }
    }
}