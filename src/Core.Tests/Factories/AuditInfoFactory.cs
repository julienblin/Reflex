// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfoFactory.cs" company="CGI">
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
    public class AuditInfoFactory : BaseFactory<AuditInfo>
    {
        public AuditInfoFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override AuditInfo CreateImpl()
        {
            return new AuditInfo
            {
                EntityType = Rand.String(),
                EntityId = Rand.Int(10000),
                ConcurrencyVersion = Rand.Int(10),
                Timestamp = Rand.DateTime(),
                Action = Rand.Enum<AuditInfoAction>(),
                User = Factories.User.Save()
            };
        }
    }
}
