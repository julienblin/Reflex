// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContactLink.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "AppContactLink")]
    public class AppContactLink : BaseEntity
    {
        private ICollection<DomainValue> _rolesInApp;

        public AppContactLink()
        {
            _rolesInApp = new List<DomainValue>();
        }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Application")]
        [AuditableCollectionReference("ContactLinks")]
        public virtual Application Application { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Contact")]
        public virtual Contact Contact { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "RolesInApp")]
        public virtual IEnumerable<DomainValue> RolesInApp { get { return _rolesInApp; } }

        public virtual void AddRoleInApp(DomainValue roleInApp)
        {
            if (roleInApp.Category != DomainValueCategory.ContactRoleInApp)
                throw new ArgumentException("roleInApp");

            _rolesInApp.Add(roleInApp);
        }

        public virtual void SetRoleInApp(IEnumerable<DomainValue> rolesInApp)
        {
            var missings = rolesInApp.Where(r => !_rolesInApp.Contains(r)).ToList();
            foreach (var missing in missings)
                _rolesInApp.Add(missing);

            var toRemoves = _rolesInApp.Where(r => !rolesInApp.Contains(r)).ToList();
            foreach (var toRemove in toRemoves)
                _rolesInApp.Remove(toRemove);
        }

        public virtual void RemoveRoleInApp(int roleInAppId)
        {
            var found = _rolesInApp.FirstOrDefault(r => r.Id == roleInAppId);
            if (found != null)
                _rolesInApp.Remove(found);
        }

        public override string ToString()
        {
            return string.Format("{0} => {1}", Application, Contact);
        }
    }
}
