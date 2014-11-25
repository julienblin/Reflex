// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Organizations.Models.Contacts
{
    public class ContactEdit
    {
        public ContactEdit()
        {
            Compagnies = new List<string>();
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ConcurrencyVersion { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int? AssociateWithApplicationId { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "LastName")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "FirstName")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Company")]
        public string Company { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(CoreResources), Name = "Email")]
        [Unique(typeof(Contact), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
        [Email(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(CoreResources), Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ContactType)]
        [Display(ResourceType = typeof(CoreResources), Name = "ContactType")]
        public int? TypeId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Sector")]
        [UIHint("Sector")]
        public int? SectorId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public IEnumerable<string> Compagnies { get; set; }

        public string FormAction
        {
            get { return Id == 0 ? "Create" : "Edit"; }
        }
    }
}