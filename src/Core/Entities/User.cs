// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Queries;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "User")]
    public class User : BaseEntity
    {
        [Unique]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 4)]
        [Display(ResourceType = typeof(CoreResources), Name = "UserName")]
        public virtual string UserName { get; set; }

        [Unique]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(CoreResources), Name = "Email")]
        public virtual string Email { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Company")]
        [DomainValue(DomainValueCategory.UserCompany)]
        public virtual DomainValue Company { get; set; }

        [NotAuditable]
        [Display(ResourceType = typeof(CoreResources), Name = "IsLockedOut")]
        public virtual bool IsLockedOut { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Role")]
        public virtual Role Role { get; set; }

        public virtual bool IsAllowed(string operation)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (Role == null) return false;

            return Role.IsAllowed(operation);
        }

        public override string ToString()
        {
            return UserName;
        }
    }
}