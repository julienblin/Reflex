// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleFactory.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Tests.Factories
{
    public class RoleFactory : BaseFactory<Role>
    {
        public RoleFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Role CreateImpl()
        {
            var role = new Role { Name = Rand.String(10), Description = Rand.LoremIpsum(255) };
            role.SetAllowedOperations(new[] { "/*" });
            return role;
        }
    }
}
