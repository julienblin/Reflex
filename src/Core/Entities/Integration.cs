// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Integration.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "Integration")]
    public class Integration : BaseConcurrentEntity
    {
        private ICollection<IntegrationTechnoLink> _technologyLinks;

        public Integration()
        {
            _technologyLinks = new List<IntegrationTechnoLink>();
        }

        [Display(ResourceType = typeof(CoreResources), Name = "AppSource")]
        [Required]
        [AuditableCollectionReference("Integrations")]
        public virtual Application AppSource { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "AppDest")]
        [Required]
        [AuditableCollectionReference("Integrations")]
        public virtual Application AppDest { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Nature")]
        [Required]
        [DomainValue(DomainValueCategory.IntegrationNature)]
        public virtual DomainValue Nature { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public virtual string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "DataDescription")]
        [DataType(DataType.MultilineText)]
        public virtual string DataDescription { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Frequency")]
        [StringLength(255)]
        public virtual string Frequency { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public virtual string Comments { get; set; }

        #region TechnologyLinks

        [NotAuditable]
        [Display(ResourceType = typeof(CoreResources), Name = "Technologies")]
        public virtual IEnumerable<IntegrationTechnoLink> TechnologyLinks { get { return _technologyLinks; } }

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

            var link = new IntegrationTechnoLink { Integration = this, Technology = technology };
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
            return string.Format("{0} -> {1}", AppSource, AppDest);
        }
    }
}