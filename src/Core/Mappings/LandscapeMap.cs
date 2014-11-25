// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapeMap.cs" company="CGI">
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
    public class LandscapeMap : BaseConcurrentEntityMap<Landscape>
    {
        public LandscapeMap()
        {
            Map(x => x.Name);
            Map(x => x.Description);

            HasMany(x => x.Servers)
               .AsSet()
               .Inverse()
               .KeyColumn("LandscapeId")
               .Access.CamelCaseField(Prefix.Underscore)
               .Cascade.None();
        }
    }
}
