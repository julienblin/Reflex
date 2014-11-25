// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstance.cs" company="CGI">
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
    [Auditable(typeof(CoreResources), "DbInstance")]
    public class DbInstance : BaseConcurrentEntity
    {
        private ICollection<DbInstanceTechnoLink> _technologyLinks;

        private ICollection<AppDbInstanceLink> _appDbInstanceLinks;

        public DbInstance()
        {
            _technologyLinks = new List<DbInstanceTechnoLink>();
            _appDbInstanceLinks = new List<AppDbInstanceLink>();
        }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public virtual string Name { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Server")]
        public virtual Server Server { get; set; }

        [NotAuditable]
        [Display(ResourceType = typeof(CoreResources), Name = "Technologies")]
        public virtual IEnumerable<DbInstanceTechnoLink> TechnologyLinks { get { return _technologyLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "AppDbInstanceLinks")]
        public virtual IEnumerable<AppDbInstanceLink> AppDbInstanceLinks { get { return _appDbInstanceLinks; } }

        public virtual void AddTechnologyLinks(IEnumerable<Technology> technologies)
        {
            foreach (var technology in technologies)
                AddTechnologyLink(technology);
        }

        public virtual void SetTechnologyLinks(IEnumerable<Technology> technologies)
        {
            var technosToRemove = TechnologyLinks.Where(tl => !technologies.Any(t => t == tl.Technology)).ToList();
            foreach (var technoToRemove in technosToRemove)
                RemoveTechnologyLink(technoToRemove.Technology);
            AddTechnologyLinks(technologies);
        }

        public virtual bool AddTechnologyLink(Technology technology)
        {
            if (TechnologyLinks.Any(tl => tl.Technology == technology))
                return false;

            var link = new DbInstanceTechnoLink { DbInstance = this, Technology = technology };
            _technologyLinks.Add(link);

            return true;
        }

        public virtual bool RemoveTechnologyLink(Technology technology)
        {
            var link = TechnologyLinks.FirstOrDefault(tl => tl.Technology == technology);
            if (link == null)
                return false;

            _technologyLinks.Remove(link);
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
