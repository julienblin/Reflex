// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerDetails.cs" company="CGI">
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

namespace CGI.Reflex.Web.Models.Pickers
{
    public class ServerDetails
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Environment { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string EvergreenDate { get; set; }

        public string Landscape { get; set; }

        public string Comments { get; set; }
    }
}