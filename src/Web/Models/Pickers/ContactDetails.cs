// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactDetails.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class ContactDetails
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Sector { get; set; }

        public string Notes { get; set; }

        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ContactType)]
        public int? ContactTypeId { get; set; }
    }
}