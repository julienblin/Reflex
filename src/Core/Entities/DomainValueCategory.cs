// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueCategory.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace CGI.Reflex.Core.Entities
{
    public enum DomainValueCategory
    {
        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationType")]
        ApplicationType,

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationStatus")]
        ApplicationStatus,

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationCriticity")]
        ApplicationCriticity,

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationUserRange")]
        ApplicationUserRange,

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationCategory")]
        ApplicationCategory,

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationCertification")]
        ApplicationCertification,

        [Display(ResourceType = typeof(CoreResources), Name = "IntegrationNature")]
        IntegrationNature,

        [Display(ResourceType = typeof(CoreResources), Name = "ContactType")]
        ContactType,

        [Display(ResourceType = typeof(CoreResources), Name = "EventType")]
        EventType,

        [Display(ResourceType = typeof(CoreResources), Name = "UserCompany")]
        UserCompany,

        [Display(ResourceType = typeof(CoreResources), Name = "TechnologyType")]
        TechnologyType,

        [Display(ResourceType = typeof(CoreResources), Name = "Environment")]
        Environment,

        [Display(ResourceType = typeof(CoreResources), Name = "ServerRole")]
        ServerRole,

        [Display(ResourceType = typeof(CoreResources), Name = "ServerStatus")]
        ServerStatus,

        [Display(ResourceType = typeof(CoreResources), Name = "ServerType")]
        ServerType,

        [Display(ResourceType = typeof(CoreResources), Name = "ContactRoleInApp")]
        ContactRoleInApp
    }
}