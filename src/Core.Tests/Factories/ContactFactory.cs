// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactFactory.cs" company="CGI">
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
    public class ContactFactory : BaseFactory<Contact>
    {
        public ContactFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Contact CreateImpl()
        {
            return new Contact
            {
                FirstName = Rand.String(),
                LastName = Rand.String(),
                Company = Rand.String(),
                Email = Rand.Email(),
                Type = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType)
            };
        }
    }
}
