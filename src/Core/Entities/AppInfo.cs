// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInfo.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "AppInfo")]
    public class AppInfo : BaseConcurrentEntity
    {
        [Display(ResourceType = typeof(CoreResources), Name = "Status")]
        [DomainValue(DomainValueCategory.ApplicationStatus)]
        public virtual DomainValue Status { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Criticity")]
        [DomainValue(DomainValueCategory.ApplicationCriticity)]
        public virtual DomainValue Criticity { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "UserRange")]
        [DomainValue(DomainValueCategory.ApplicationUserRange)]
        public virtual DomainValue UserRange { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        public virtual string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Category")]
        [DomainValue(DomainValueCategory.ApplicationCategory)]
        public virtual DomainValue Category { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Certification")]
        [DomainValue(DomainValueCategory.ApplicationCertification)]
        public virtual DomainValue Certification { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "MaintenanceWindow")]
        [DataType(DataType.MultilineText)]
        public virtual string MaintenanceWindow { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public virtual string Notes { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Sector")]
        public virtual Sector Sector { get; set; }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}