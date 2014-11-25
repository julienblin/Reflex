// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryQuery.cs" company="CGI">
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
    public class LogEntryQuery : BaseQueryOver<LogEntry>
    {
        public string CorrelationId { get; set; }

        public string Level { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Logger { get; set; }

        public string User { get; set; }

        public string MessageLike { get; set; }

        public string ContextLike { get; set; }

        public string ExceptionLike { get; set; }

        protected override IQueryOver<LogEntry, LogEntry> OverImpl(ISession session)
        {
            var query = session.QueryOver<LogEntry>();

            if (!string.IsNullOrEmpty(CorrelationId))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<LogEntry>(l => l.CorrelationId), CorrelationId, MatchMode.Exact));

            if (!string.IsNullOrEmpty(Level))
                query.Where(l => l.Level == Level);

            if (DateFrom.HasValue)
                query.Where(l => l.Date >= DateFrom.Value);

            if (DateTo.HasValue)
                query.Where(l => l.Date <= DateTo.Value);

            if (!string.IsNullOrEmpty(Logger))
                query.Where(l => l.Logger == Logger);

            if (!string.IsNullOrEmpty(User))
                query.Where(l => l.LoggedUser == User);

            if (!string.IsNullOrEmpty(MessageLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<LogEntry>(l => l.Message), MessageLike, MatchMode.Anywhere));

            if (!string.IsNullOrEmpty(ContextLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<LogEntry>(l => l.Context), ContextLike, MatchMode.Anywhere));

            if (!string.IsNullOrEmpty(ExceptionLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<LogEntry>(l => l.Exception), ExceptionLike, MatchMode.Anywhere));

            return query;
        }
    }
}
