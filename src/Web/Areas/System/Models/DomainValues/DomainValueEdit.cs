// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.System.Models.DomainValues
{
    public class DomainValueEdit
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ConcurrencyVersion { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DomainValueCategory Category { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(25, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public virtual string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Color")]
        [UIHint("Color")]
        public virtual string Color { get; set; }

        public string FormAction
        {
            get { return Id == 0 ? "Create" : "Edit"; }
        }
    }
}