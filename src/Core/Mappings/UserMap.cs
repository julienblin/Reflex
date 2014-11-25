// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserMap.cs" company="CGI">
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
    public class UserMap : BaseEntityMap<User>
    {
        public UserMap()
        {
            Map(x => x.UserName);
            Map(x => x.Email);
            Map(x => x.IsLockedOut);

            References(x => x.Company).Cascade.None();
            References(x => x.Role).Cascade.None();
        }
    }
}
