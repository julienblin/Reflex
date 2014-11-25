// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventsDetails.cs" company="CGI">
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

namespace CGI.Reflex.Web.Areas.Applications.Models.Events
{
    public class EventsDetails : AbstractSearchResultModel<Event>
    {
        public string AppName { get; set; }

        public int AppId { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Source")]
        public string Source { get; set; }

        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(WebResources), Name = "DateFrom")]
        public DateTime? DateFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(WebResources), Name = "DateTo")]
        public DateTime? DateTo { get; set; }

        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.EventType)]
        [Display(ResourceType = typeof(CoreResources), Name = "EventType")]
        public int? Type { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        public string Description { get; set; }

        public IEnumerable<object> OrderList { get; set; }

        public bool SearchDefined
        {
            get { return !string.IsNullOrEmpty(Source) || DateFrom.HasValue || DateTo.HasValue || Type.HasValue || !string.IsNullOrEmpty(Description); }
        }
    }
}