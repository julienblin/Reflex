// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactLinkEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Applications.Models.Contacts
{
    public class ContactLinkEdit
    {
        public ContactLinkEdit()
        {
            RoleInAppIds = new List<int>();
            AvailableRoles = Enumerable.Empty<DomainValue>();
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public int ContactId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int? AssociateWithApplicationId { get; set; }

        public ICollection<int> RoleInAppIds { get; set; }

        public string FullContactName { get; set; }

        public string ApplicationName { get; set; }   
       
        public IEnumerable<DomainValue> AvailableRoles { get; set; }
    }
}