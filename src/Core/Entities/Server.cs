// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Server.cs" company="CGI">
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
    [Auditable(typeof(CoreResources), "Server")]
    public class Server : BaseConcurrentEntity
    {
        private ICollection<AppServerLink> _applicationLinks;

        private ICollection<ServerTechnoLink> _technologyLinks;

        private ICollection<DbInstance> _dbInstances;

        public Server()
        {
            _applicationLinks = new List<AppServerLink>();
            _technologyLinks = new List<ServerTechnoLink>();
            _dbInstances = new List<DbInstance>();
        }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(30)]
        public virtual string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Alias")]
        [StringLength(30)]
        public virtual string Alias { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public virtual string Comments { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "EvergreenDate")]
        [DataType(DataType.Date)]
        public virtual DateTime? EvergreenDate { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Environment")]
        [DomainValue(DomainValueCategory.Environment)]
        [Required]
        public virtual DomainValue Environment { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Role")]
        [DomainValue(DomainValueCategory.ServerRole)]
        public virtual DomainValue Role { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Status")]
        [DomainValue(DomainValueCategory.ServerStatus)]
        public virtual DomainValue Status { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Type")]
        [DomainValue(DomainValueCategory.ServerType)]
        public virtual DomainValue Type { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Landscape")]
        [AuditableCollectionReference("Servers", true)]
        public virtual Landscape Landscape { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Applications")]
        public virtual IEnumerable<AppServerLink> ApplicationLinks { get { return _applicationLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "DbInstance")]
        public virtual IEnumerable<DbInstance> DbInstances { get { return _dbInstances; } }

        #region TechnologyLinks

        [Display(ResourceType = typeof(CoreResources), Name = "Servers")]
        [NotAuditable]
        public virtual IEnumerable<ServerTechnoLink> TechnologyLinks { get { return _technologyLinks; } }

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

            var link = new ServerTechnoLink { Server = this, Technology = technology };
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

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
