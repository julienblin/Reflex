// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportConfig.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace CGI.Reflex.Web.Areas.System.Models.ImportExport
{
    public class ImportConfig
    {
        public string Importer { get; set; }

        public IEnumerable<KeyValuePair<string, string>> ImportersList { get; set; }
    }
}