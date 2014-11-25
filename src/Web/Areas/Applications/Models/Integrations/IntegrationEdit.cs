// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Applications.Models.Integrations
{
    public class IntegrationEdit
    {
        public IntegrationEdit()
        {
            TechnoIds = Enumerable.Empty<int>();
            Technologies = Enumerable.Empty<Technology>();
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ConcurrencyVersion { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "AppSource")]
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [UIHint("Application")]
        public int AppSourceId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "AppDest")]
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [UIHint("Application")]
        public int AppDestId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Nature")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.IntegrationNature)]
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public int? NatureId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "DataDescription")]
        [DataType(DataType.MultilineText)]
        public string DataDescription { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Frequency")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Frequency { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public IEnumerable<int> TechnoIds { get; set; }

        public IEnumerable<Technology> Technologies { get; set; }

        public string FormAction
        {
            get { return Id == 0 ? "Create" : "Edit"; }
        }
    }
}