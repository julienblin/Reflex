// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexConfig.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.System.Models.Configuration
{
    public class ReflexConfig
    {
        public ReflexConfig()
        {
            ActiveAppStatusDVIds = new List<int>();
            ApplicationStatuses = Enumerable.Empty<DomainValue>();
        }

        [Display(ResourceType = typeof(CoreResources), Name = "ActiveAppStatusDVIds")]
        public IList<int> ActiveAppStatusDVIds { get; set; }

        public IEnumerable<DomainValue> ApplicationStatuses { get; set; }
    }
}