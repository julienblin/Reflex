// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactSeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public class ContactSeeder : BaseSeeder
    {
        public override int Priority { get { return 5; } }

        protected override void SeedImpl()
        {
            Session.Save(new Contact
            {
                LastName = "Robert",
                FirstName = "Sylvain",
                Company = "CGI",
                Email = "sylvain.robert@cgi.com",
                Type = Get(DomainValueCategory.ContactType, "Directeur TI"),
                Notes = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
            });

            Session.Save(new Contact
            {
                LastName = "Potvin",
                FirstName = "Mélanie",
                Company = "CGI",
                Email = "melanie.potvin@cgi.com",
                Type = Get(DomainValueCategory.ContactType, "Analyste d'affaire")
            });

            Session.Save(new Contact
            {
                LastName = "Paquet",
                FirstName = "Vincent",
                Company = "CGI",
                Email = "vincent.paquet@cgi.com",
                Type = Get(DomainValueCategory.ContactType, "Architecte"),
                Notes = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce pretium luctus purus, interdum eleifend elit tempus a. Suspendisse eu quam lacus, eget scelerisque ligula. Vestibulum felis eros, bibendum et ultricies eget, venenatis vel tortor. Suspendisse porttitor suscipit eleifend. Curabitur sed elit id erat blandit venenatis at nec nisl. Morbi quis turpis ac purus lobortis molestie. Nunc aliquam mi in augue elementum mattis. Duis commodo molestie eros, eget porta dui blandit nec. Nunc vitae nulla ac neque hendrerit dignissim."
            });

            Session.Save(new Contact
            {
                LastName = "Blin",
                FirstName = "Julien",
                Company = "CGI",
                Email = "julien.blin@cgi.com",
                Type = Get(DomainValueCategory.ContactType, "Architecte"),
                Notes = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam lacinia turpis eget lacus iaculis quis porttitor lacus blandit. In sit amet risus ipsum. Suspendisse potenti. "
            });
        }
    }
}