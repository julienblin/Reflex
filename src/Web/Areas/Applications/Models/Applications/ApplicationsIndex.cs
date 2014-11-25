// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsIndex.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Applications.Models.Applications
{
    public class ApplicationsIndex : AbstractSearchResultModel<Application>
    {
        public ApplicationsIndex()
        {
            Statuses = Enumerable.Empty<int>();
            ApplicationTypes = Enumerable.Empty<int>();
        }

        [Display(ResourceType = typeof(WebResources), Name = "QuickNameOrCode")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string QuickNameOrCode { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationType")]
        [UIHint("DomainValues")]
        [DomainValue(DomainValueCategory.ApplicationType)]
        public IEnumerable<int> ApplicationTypes { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Status")]
        [UIHint("DomainValues")]
        [DomainValue(DomainValueCategory.ApplicationStatus)]
        public IEnumerable<int> Statuses { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Criticity")]
        [UIHint("DomainValues")]
        [DomainValue(DomainValueCategory.ApplicationCriticity)]
        public IEnumerable<int> Criticities { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Contact")]
        [UIHint("Contact")]
        public int? Contact { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Sector")]
        [UIHint("Sector")]
        public int? Sector { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Technology")]
        [UIHint("Technology")]
        public int? Technology { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Server")]
        [UIHint("Server")]
        public int? Server { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "ContactRoleInApp")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ContactRoleInApp)]
        public int? Role { get; set; }

        public bool SearchDefined
        {
            get
            {
                return ((ApplicationTypes != null) && ApplicationTypes.Any())
                     || ((Statuses != null) && Statuses.Any())
                     || ((Criticities != null) && Criticities.Any())
                     || Contact.HasValue
                     || Sector.HasValue
                     || Technology.HasValue
                     || Server.HasValue
                     || Role.HasValue;
            }
        }
    }
}