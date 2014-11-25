// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyProfileEdit.cs" company="CGI">
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
using CGI.Reflex.Core.Utilities;

namespace CGI.Reflex.Web.Models.MyProfile
{
    public class MyProfileEdit : IValidatableObject
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Unique(typeof(User), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 4, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "UserName")]
        public string UserName { get; set; }

        [Unique(typeof(User), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Email(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "InvalidEmail")]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(CoreResources), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Company")]
        [DomainValue(DomainValueCategory.UserCompany)]
        [UIHint("DomainValue")]
        public int? CompanyId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Role")]
        public Role Role { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(WebResources), Name = "PasswordConfirm")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Password))
            {
                if (!PasswordPolicy.Validate(Password))
                {
                    yield return new ValidationResult("Le mot de passe n'est pas valide. Il doit faire 6 caractères minimum, et contenir au moins une minusucle, une majuscule et un chiffre.", new[] { "Password" });
                }

                if (!Password.Equals(PasswordConfirm))
                {
                    yield return new ValidationResult("Les mots de passe ne correspondent pas.", new[] { "PasswordConfirm" });
                }   
            }
        }
    }
}