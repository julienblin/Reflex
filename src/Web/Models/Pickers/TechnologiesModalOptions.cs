// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesModalOptions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class TechnologiesModalOptions
    {
        public TechnologiesModalOptions()
        {
            SelectionMode = SelectionMode.Multiple;
        }

        public string PostUrl { get; set; }

        public string AddFunctionName { get; set; }

        public SelectionMode SelectionMode { get; set; }
    }
}