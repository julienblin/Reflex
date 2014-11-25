// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerMap.cs" company="CGI">
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
    public class ServerMap : BaseConcurrentEntityMap<Server>
    {
        public ServerMap()
        {
            Map(x => x.Name);
            Map(x => x.Alias);
            Map(x => x.Comments);
            Map(x => x.EvergreenDate);
            References(x => x.Environment).Cascade.None();
            References(x => x.Role).Cascade.None();
            References(x => x.Status).Cascade.None();
            References(x => x.Type).Cascade.None();
            References(x => x.Landscape).Cascade.None();

            HasMany(x => x.ApplicationLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("ServerId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.None();

            HasMany(x => x.TechnologyLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("ServerId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.DbInstances)
                .AsSet()
                .Inverse()
                .KeyColumn("ServerId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();
        }
    }
}
