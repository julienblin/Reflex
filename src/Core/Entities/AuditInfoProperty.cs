// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfoProperty.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace CGI.Reflex.Core.Entities
{
    public class AuditInfoProperty : BaseEntity
    {
        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "AuditInfo")]
        public virtual AuditInfo AuditInfo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        [Display(ResourceType = typeof(CoreResources), Name = "PropertyName")]
        public virtual string PropertyName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        [Display(ResourceType = typeof(CoreResources), Name = "PropertyType")]
        public virtual string PropertyType { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "OldValue")]
        public virtual string OldValue { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "NewValue")]
        public virtual string NewValue { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} -> {2}", PropertyName, OldValue, NewValue);
        }
    }
}