// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlashData.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGI.Reflex.Web.Infra.Controllers
{
    public class FlashData
    {
        public string Heading { get; set; }

        public string Text { get; set; }

        public FlashLevel Level { get; set; }

        public bool DisableHtmlEscaping { get; set; }
    }
}