// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportOperationLineResult.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace CGI.Reflex.Core.Importers
{
    public enum LineResultStatus
    {
        [Display(ResourceType = typeof(CoreResources), Name = "Created")]
        Created,

        [Display(ResourceType = typeof(CoreResources), Name = "Merged")]
        Merged,

        [Display(ResourceType = typeof(CoreResources), Name = "Rejected")]
        Rejected,

        [Display(ResourceType = typeof(CoreResources), Name = "Error")]
        Error
    }

    public class ImportOperationLineResult
    {
        public string Section { get; set; }

        public int LineNumber { get; set; }

        public LineResultStatus Status { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}