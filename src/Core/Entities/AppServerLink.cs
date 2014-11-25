// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServerLink.cs" company="CGI">
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
    [Auditable(typeof(CoreResources), "AppServerLink")]
    public class AppServerLink : BaseEntity
    {
        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Application")]
        [AuditableCollectionReference("ServerLinks")]
        public virtual Application Application { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Server")]
        [AuditableCollectionReference("ApplicationLinks")]
        public virtual Server Server { get; set; }

        public override string ToString()
        {
            return string.Format("{0} => {1}", Application, Server);
        }
    }
}
