// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfoAction.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace CGI.Reflex.Core.Entities
{
    public enum AuditInfoAction
    {
        [Display(ResourceType = typeof(CoreResources), Name = "Creation")]
        Create,

        [Display(ResourceType = typeof(CoreResources), Name = "Updation")]
        Update,

        [Display(ResourceType = typeof(CoreResources), Name = "Deletion")]
        Delete,

        [Display(ResourceType = typeof(CoreResources), Name = "AddAssociation")]
        AddAssociation,

        [Display(ResourceType = typeof(CoreResources), Name = "UpdateAssociation")]
        UpdateAssociation,

        [Display(ResourceType = typeof(CoreResources), Name = "RemoveAssociation")]
        RemoveAssociation
    }
}