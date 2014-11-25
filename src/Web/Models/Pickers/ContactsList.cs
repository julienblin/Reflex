// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactsList.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class ContactsList
    {
        public IEnumerable<Contact> Contacts { get; set; }

        public string PostUrl { get; set; }

        public string ReturnUrl { get; set; }

        public string AddFunctionName { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public int ApplicationId { get; set; }
    }
}