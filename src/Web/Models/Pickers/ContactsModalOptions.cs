// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactsModalOptions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class ContactsModalOptions
    {
        public ContactsModalOptions()
        {
            SelectionMode = SelectionMode.Multiple;
        }

        public string PostUrl { get; set; }

        public string ReturnUrl { get; set; }

        public string AddFunctionName { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public int ApplicationId { get; set; }
    }
}