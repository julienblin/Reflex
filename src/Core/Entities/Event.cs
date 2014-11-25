// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Event.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "Event")]
    public class Event : BaseConcurrentEntity
    {
        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Application")]
        [AuditableCollectionReference("Events")]
        public virtual Application Application { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        [Required]
        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Source")]
        [Required]
        [StringLength(255)]
        public virtual string Source { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Date")]
        [Required]
        [DataType(DataType.Date)]
        public virtual DateTime Date { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "EventType")]
        [Required]
        [DomainValue(DomainValueCategory.EventType)]
        public virtual DomainValue Type { get; set; }

        public override string ToString()
        {
            return Description.TruncateWords(50);
        }
    }
}