// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDbInstanceLink.cs" company="CGI">
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
    [Auditable(typeof(CoreResources), "AppDbInstanceLink")]
    public class AppDbInstanceLink : BaseEntity
    {
        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Application")]
        [AuditableCollectionReference("DbInstanceLinks")]
        public virtual Application Application { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "DbInstance")]
        [AuditableCollectionReference("AppDbInstanceLinks")]
        public virtual DbInstance DbInstances { get; set; }

        public override string ToString()
        {
            return string.Format("{0} => {1}", Application, DbInstances);
        }
    }
}
