// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyDelete.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGI.Reflex.Web.Areas.Applications.Models.Technologies
{
    public class TechnologyDelete
    {
        [HiddenInput(DisplayValue = false)]
        public int TechnologyId { get; set; }

        public string TechnologyName { get; set; }
    }
}