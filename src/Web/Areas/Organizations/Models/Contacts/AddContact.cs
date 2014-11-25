// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddContact.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Organizations.Models.Contacts
{
    public class AddContact
    {
        public string AppName { get; set; }

        public int ApplicationId { get; set; }
    }
}