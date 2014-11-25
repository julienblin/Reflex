// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContactLinkFactory.cs" company="CGI">
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
    public class AppContactLinkFactory : BaseFactory<AppContactLink>
    {
        public AppContactLinkFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override AppContactLink CreateImpl()
        {
            return new AppContactLink
            {
                Application = Factories.Application.Save(),
                Contact = Factories.Contact.Save()
            };
        }
    }
}
