// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactsDetails.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Applications.Models.Contacts
{
    public class ContactsDetails : AbstractSearchResultModel<AppContactLink>
    {
        public string AppName { get; set; }

        public int AppId { get; set; }

        public string ReturnUrl { get; set; }

        public int AssociateWithApplicationId { get; set; }
    }
}