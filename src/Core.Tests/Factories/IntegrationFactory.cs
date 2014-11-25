// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationFactory.cs" company="CGI">
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
    public class IntegrationFactory : BaseFactory<Integration>
    {
        public IntegrationFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Integration CreateImpl()
        {
            return new Integration
            {
                AppSource = Factories.Application.Save(),
                AppDest = Factories.Application.Save(),
                Nature = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature),
                Name = Rand.String(20),
                Description = Rand.LoremIpsum(),
                DataDescription = Rand.LoremIpsum(),
                Frequency = Rand.String(50),
                Comments = Rand.LoremIpsum()
            };
        }
    }
}
