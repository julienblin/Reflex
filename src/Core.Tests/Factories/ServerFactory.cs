// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerFactory.cs" company="CGI">
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
    public class ServerFactory : BaseFactory<Server>
    {
        public ServerFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Server CreateImpl()
        {
            return new Server
            {
                Name = Rand.String(10),
                Alias = Rand.String(10),
                Comments = Rand.String(),
                EvergreenDate = Rand.DateTime(),
                Environment = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.Environment),
                Role = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole),
                Status = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus),
                Type = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType),
                Landscape = Factories.Landscape.Save()
            };
        }
    }
}
