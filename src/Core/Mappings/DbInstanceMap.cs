// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceMap.cs" company="CGI">
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
    public class DbInstanceMap : BaseConcurrentEntityMap<DbInstance>
    {
        public DbInstanceMap()
        {
            Map(x => x.Name);
            References(x => x.Server).Cascade.None();

            HasMany(x => x.TechnologyLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("DbInstanceId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.AppDbInstanceLinks)
               .AsSet()
               .Inverse()
               .KeyColumn("DbInstanceId")
               .Access.CamelCaseField(Prefix.Underscore)
               .Cascade.AllDeleteOrphan();
        }
    }
}
