// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceFactory.cs" company="CGI">
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
    public class DbInstanceFactory : BaseFactory<DbInstance>
    {
        public DbInstanceFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override DbInstance CreateImpl()
        {
            return new DbInstance
            {
                Name = Rand.String(20),
                Server = Factories.Server.Save()
            };
        }
    }
}
