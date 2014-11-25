// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContactLinkMap.cs" company="CGI">
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
    public class AppContactLinkMap : BaseEntityMap<AppContactLink>
    {
        public AppContactLinkMap()
        {
            References(x => x.Application).Cascade.None();
            References(x => x.Contact).Cascade.None();
            HasManyToMany(x => x.RolesInApp)
                .AsSet()
                .Cascade.None();
        }
    }
}
