// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfoQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace CGI.Reflex.Core.Queries
{
    public class AuditInfoQuery : BaseQueryOver<AuditInfo>
    {
        public string EntityType { get; set; }

        public int? EntityId { get; set; }

        public string PropertyName { get; set; }

        public bool DistinctRoot { get; set; }

        protected override IQueryOver<AuditInfo, AuditInfo> OverImpl(ISession session)
        {
            var queryOver = session.QueryOver<AuditInfo>();

            if (!string.IsNullOrEmpty(EntityType))
                queryOver.Where(a => a.EntityType == EntityType);

            if (EntityId.HasValue)
                queryOver.Where(a => a.EntityId == EntityId.Value);

            if (!string.IsNullOrEmpty(PropertyName))
            {
                var subQueryProperty = QueryOver.Of<AuditInfoProperty>()
                                                .Where(p => p.PropertyName == PropertyName)
                                                .SelectList(list => list.Select(p => p.AuditInfo.Id));
                queryOver.WithSubquery
                         .WhereProperty(a => a.Id).In(subQueryProperty);
            }

            if (DistinctRoot)
                queryOver.TransformUsing(Transformers.DistinctRootEntity);

            return queryOver;
        }

        protected override bool OrderByCallback(IQueryOver<AuditInfo, AuditInfo> queryOver, string propertyName, OrderType orderType)
        {
            switch (propertyName)
            {
                case "User.UserName":
                    queryOver.Left.JoinQueryOver(a => a.User).OrderBy(u => u.UserName, orderType);
                    return true;
                default:
                    return false;
            }
        }
    }
}