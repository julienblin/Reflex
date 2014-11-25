// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapeFactory.cs" company="CGI">
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
    public class LandscapeFactory : BaseFactory<Landscape>
    {
        public LandscapeFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Landscape CreateImpl()
        {
            return new Landscape
            {
                Name = Rand.String(10),
                Description = Rand.String()
            };
        }
    }
}
