// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactsIndex.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Organizations.Models.Contacts
{
    public class ContactsIndex : AbstractSearchResultModel<Contact>
    {
        [Display(ResourceType = typeof(CoreResources), Name = "LastName")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "FirstName")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "ContactType")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ContactType)]
        public int? TypeId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "FullName")]
        public string FullName { get; set; }

        [UIHint("Sector")]
        [Display(ResourceType = typeof(CoreResources), Name = "Sector")]
        public int? SectorId { get; set; }

        public bool SearchDefined
        {
            get
            {
                return !string.IsNullOrEmpty(LastName) || !string.IsNullOrEmpty(FirstName) || TypeId.HasValue;
            }
        }
    }
}