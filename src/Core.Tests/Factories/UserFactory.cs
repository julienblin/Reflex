// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserFactory.cs" company="CGI">
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
    public class UserFactory : BaseFactory<User>
    {
        public UserFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override User CreateImpl()
        {
            return new User
            {
                UserName = Rand.String(10),
                Email = Rand.Email(),
                Company = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.UserCompany),
                IsLockedOut = false,
                Role = Factories.Role.Save()
            };
        }
    }
}
