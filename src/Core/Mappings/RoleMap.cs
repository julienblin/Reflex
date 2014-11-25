// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Mappings.UserTypes;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class RoleMap : BaseConcurrentEntityMap<Role>
    {
        public RoleMap()
        {
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.AllowedOperations).CustomType<DelimitedListUserType<string>>();
            Map(x => x.DeniedOperations).CustomType<DelimitedListUserType<string>>();
        }
    }
}
