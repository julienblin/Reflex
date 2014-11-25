// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.System.Models.Users
{
    public class UserEdit
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Unique(typeof(User), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 4, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "UserName")]
        public string UserName { get; set; }

        [Unique(typeof(User), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
        [Email(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "InvalidEmail")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(CoreResources), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Company")]
        [DomainValue(DomainValueCategory.UserCompany)]
        [UIHint("DomainValue")]
        public int? Company { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "IsLockedOut")]
        public bool IsLockedOut { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Role")]
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public int? RoleId { get; set; }

        public IEnumerable<Role> Roles { get; set; }

        public string FormAction
        {
            get { return Id == 0 ? "Create" : "Edit"; }
        }
    }
}