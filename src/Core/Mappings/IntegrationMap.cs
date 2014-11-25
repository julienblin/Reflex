// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationMap.cs" company="CGI">
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
    public class IntegrationMap : BaseConcurrentEntityMap<Integration>
    {
        public IntegrationMap()
        {
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.DataDescription);
            Map(x => x.Frequency);
            Map(x => x.Comments);

            References(x => x.AppSource).Cascade.None();
            References(x => x.AppDest).Cascade.None();
            References(x => x.Nature).Cascade.None();

            HasMany(x => x.TechnologyLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("IntegrationId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();
        }
    }
}
