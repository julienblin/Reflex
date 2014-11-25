// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyFactory.cs" company="CGI">
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
    public class TechnologyFactory : BaseFactory<Technology>
    {
        public TechnologyFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Technology CreateImpl()
        {
            return new Technology
            {
                Name = Rand.String(10),
                EndOfSupport = Rand.DateTime(future: true),
                TechnologyType = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.TechnologyType)
            };
        }
    }
}
