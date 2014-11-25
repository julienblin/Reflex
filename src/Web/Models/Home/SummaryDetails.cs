// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SummaryDetails.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries.Series;

namespace CGI.Reflex.Web.Models.Home
{
    public enum DisplayType
    {
        Values,
        Percentage
    }

    public class SummaryDetails
    {
        public SummaryDetails()
        {
            OnlyActiveApplications = true;
        }

        [Display(ResourceType = typeof(CoreResources), Name = "LineCriteria")]
        public string LineCriteria { get; set; }

        public string LineCriteriaDisplayName { get { return ApplicationSeries.GetLineDisplayName(LineCriteria); } }

        public IEnumerable<KeyValuePair<string, string>> LineCriteriaList { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "ColumnCriteria")]
        public string ColumnCriteria { get; set; }

        public string ColumnCriteriaDisplayName { get { return string.IsNullOrEmpty(ColumnCriteria) ? string.Empty : ApplicationSeries.GetColumnDisplayName(ColumnCriteria); } }

        public IEnumerable<KeyValuePair<string, string>> ColumnCriteriaList { get; set; }

        public ApplicationSeriesResult Result { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "OnlyActiveApplications")]
        public bool OnlyActiveApplications { get; set; }

        public DisplayType DisplayType { get; set; }
    }
}