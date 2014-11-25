// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfoPropertyMap.cs" company="CGI">
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
    public class AuditInfoPropertyMap : BaseEntityMap<AuditInfoProperty>
    {
        public AuditInfoPropertyMap()
        {
            Map(x => x.PropertyName);
            Map(x => x.PropertyType);
            Map(x => x.OldValue);
            Map(x => x.NewValue);

            References(x => x.AuditInfo);
        }
    }
}
