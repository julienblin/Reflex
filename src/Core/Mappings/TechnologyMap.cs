// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyMap.cs" company="CGI">
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
    public class TechnologyMap : BaseHierarchicalEntityMap<Technology>
    {
        public TechnologyMap()
        {
            Map(x => x.EndOfSupport);
            Map(x => x.Description);

            References(x => x.TechnologyType).Cascade.None();
            References(x => x.Contact).Cascade.None();

            HasMany(x => x.ApplicationLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("TechnologyId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.None();

            HasMany(x => x.ServerLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("TechnologyId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.None();

            HasMany(x => x.IntegrationLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("TechnologyId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.None();
        }
    }
}
