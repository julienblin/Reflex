// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditView.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Models.History
{
    public class AuditView : AbstractSearchResultModel<AuditInfo>
    {
        public Type EntityType { get; set; }
    }
}