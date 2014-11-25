// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Contact.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "Contact")]
    public class Contact : BaseConcurrentEntity
    {
        private ICollection<AppContactLink> _applicationLinks;

        public Contact()
        {
            _applicationLinks = new List<AppContactLink>();
        }

        [StringLength(50)]
        [Display(ResourceType = typeof(CoreResources), Name = "FirstName")]
        public virtual string FirstName { get; set; }

        [StringLength(50)]
        [Display(ResourceType = typeof(CoreResources), Name = "LastName")]
        public virtual string LastName { get; set; }

        [StringLength(50)]
        [Display(ResourceType = typeof(CoreResources), Name = "Company")]
        public virtual string Company { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "FullName")]
        public virtual string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                    return Company;

                if (string.IsNullOrEmpty(FirstName))
                    return LastName;

                return string.Format("{0}, {1}", LastName, FirstName);
            }
        }

        [Unique]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(CoreResources), Name = "Email")]
        public virtual string Email { get; set; }

        [Required]
        [DomainValue(DomainValueCategory.ContactType)]
        [Display(ResourceType = typeof(CoreResources), Name = "Type")]
        public virtual DomainValue Type { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Sector")]
        public virtual Sector Sector { get; set; }

        [StringLength(255)]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(CoreResources), Name = "PhoneNumber")]
        public virtual string PhoneNumber { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "Notes")]
        public virtual string Notes { get; set; }

        [NotAuditable]
        [Display(ResourceType = typeof(CoreResources), Name = "Applications")]
        public virtual IEnumerable<AppContactLink> ApplicationLinks { get { return _applicationLinks; } }

        public virtual string GetEscapedFullName(string spaceReplacement = "_")
        {
            return string.Format(
                "{0} {1} {2}",
                FirstName == null ? string.Empty : FirstName.Replace(" ", spaceReplacement),
                LastName == null ? string.Empty : LastName.Replace(" ", spaceReplacement),
                Company == null ? string.Empty : Company.Replace(" ", spaceReplacement));
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}