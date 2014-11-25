// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersIndex.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.System.Models.Users
{
    public class UsersIndex : AbstractSearchResultModel<User>
    {
        public UsersIndex()
        {
            Roles = Enumerable.Empty<int>();
            AuthorizedRoles = Enumerable.Empty<Role>();
            AvailableRoles = Enumerable.Empty<Role>();
        }

        [Display(ResourceType = typeof(WebResources), Name = "NameOrEmail")]
        public string NameOrEmail { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Role")]
        public IEnumerable<int> Roles { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Company")]
        [UIHint("DomainValues")]
        [DomainValue(DomainValueCategory.UserCompany)]
        public IEnumerable<int> Companies { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "IncludeLockedOut")]
        public bool IncludeLockedOut { get; set; }

        public IEnumerable<Role> AuthorizedRoles { get; set; }

        public IEnumerable<Role> AvailableRoles { get; set; }

        public bool SearchDefined
        {
            get
            { 
                return !string.IsNullOrEmpty(NameOrEmail)
                     || ((Roles != null) && Roles.Any())
                     || ((Companies != null) && Companies.Any())
                     || IncludeLockedOut;
            }
        }
    }
}