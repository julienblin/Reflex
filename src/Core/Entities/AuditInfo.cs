// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfo.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CGI.Reflex.Core.Entities
{
    public class AuditInfo : BaseEntity
    {
        private ICollection<AuditInfoProperty> _properties;

        public AuditInfo()
        {
            _properties = new List<AuditInfoProperty>();
        }

        [Required]
        [StringLength(255)]
        [Display(ResourceType = typeof(CoreResources), Name = "EntityType")]
        public virtual string EntityType { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "EntityId")]
        public virtual int EntityId { get; set; }

        public virtual int? ConcurrencyVersion { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Timestamp")]
        public virtual DateTime Timestamp { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Action")]
        public virtual AuditInfoAction Action { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "User")]
        public virtual User User { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "DisplayName")]
        public virtual string DisplayName { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Properties")]
        public virtual IEnumerable<AuditInfoProperty> Properties { get { return _properties; } }

        public virtual void Add(AuditInfoProperty property)
        {
            _properties.Add(property);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Action, EntityType, EntityId);
        }
    }
}