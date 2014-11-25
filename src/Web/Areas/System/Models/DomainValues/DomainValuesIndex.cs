// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValuesIndex.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.System.Models.DomainValues
{
    public class DomainValuesIndex
    {
        [Display(ResourceType = typeof(CoreResources), Name = "Category")]
        public DomainValueCategory Category { get; set; }

        public IEnumerable<object> CategoryList { get; set; }

        public IList<DomainValue> Items { get; set; }
    }
}