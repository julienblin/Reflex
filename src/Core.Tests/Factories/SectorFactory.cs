// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorFactory.cs" company="CGI">
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
    public class SectorFactory : BaseFactory<Sector>
    {
        public SectorFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Sector CreateImpl()
        {
            return new Sector
            {
                Name = Rand.String(10)
            };
        }
    }
}
