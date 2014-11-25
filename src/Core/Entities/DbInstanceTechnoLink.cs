// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceTechnoLink.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "DbInstanceTechnoLink")]
    public class DbInstanceTechnoLink : BaseEntity
    {
        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "DbInstance")]
        [AuditableCollectionReference("TechnologyLinks")]
        public virtual DbInstance DbInstance { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Technology")]
        public virtual Technology Technology { get; set; }

        public override string ToString()
        {
            return string.Format("{0} => {1}", DbInstance, Technology);
        }
    }
}
