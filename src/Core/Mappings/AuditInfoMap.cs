// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfoMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class AuditInfoMap : BaseEntityMap<AuditInfo>
    {
        public AuditInfoMap()
        {
            Map(x => x.EntityType);
            Map(x => x.EntityId);
            Map(x => x.ConcurrencyVersion);
            Map(x => x.Timestamp);
            Map(x => x.Action);
            Map(x => x.DisplayName);

            References(x => x.User).Cascade.None();
            HasMany(x => x.Properties)
                .AsSet()
                .Access.CamelCaseField(Prefix.Underscore)
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}
