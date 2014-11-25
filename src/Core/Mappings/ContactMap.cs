// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class ContactMap : BaseConcurrentEntityMap<Contact>
    {
        public ContactMap()
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Company);
            Map(x => x.Email);
            Map(x => x.PhoneNumber);
            Map(x => x.Notes);

            References(x => x.Type).Cascade.None();
            References(x => x.Sector).Cascade.None();

            HasMany(x => x.ApplicationLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("ContactId")
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
