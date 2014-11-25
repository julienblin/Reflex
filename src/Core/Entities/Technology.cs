// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technology.cs" company="CGI">
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
    [Auditable(typeof(CoreResources), "Technology")]
    public class Technology : BaseHierarchicalEntity<Technology>
    {
        private ICollection<AppTechnoLink> _applicationLinks;

        private ICollection<IntegrationTechnoLink> _integrationLinks;

        private ICollection<ServerTechnoLink> _serverLinks;

        public Technology()
        {
            _applicationLinks = new List<AppTechnoLink>();
            _integrationLinks = new List<IntegrationTechnoLink>();
            _serverLinks = new List<ServerTechnoLink>();
        }

        public enum EndOfSupportStatus
        {
            [Display(ResourceType = typeof(CoreResources), Name = "EndOfSupportUnknown")]
            Unknown,

            [Display(ResourceType = typeof(CoreResources), Name = "UpToDate")]
            UpToDate,

            [Display(ResourceType = typeof(CoreResources), Name = "AlmostOutdated")]
            AlmostOutdated,

            [Display(ResourceType = typeof(CoreResources), Name = "Outdated")]
            Outdated
        }

        [Display(ResourceType = typeof(CoreResources), Name = "EndOfSupport")]
        [DataType(DataType.Date)]
        public virtual DateTime? EndOfSupport { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "TechnologyType")]
        [DomainValue(DomainValueCategory.TechnologyType)]
        public virtual DomainValue TechnologyType { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Contact")]
        public virtual Contact Contact { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        public virtual string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Applications")]
        public virtual IEnumerable<AppTechnoLink> ApplicationLinks { get { return _applicationLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Integrations")]
        public virtual IEnumerable<IntegrationTechnoLink> IntegrationLinks { get { return _integrationLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Servers")]
        public virtual IEnumerable<ServerTechnoLink> ServerLinks { get { return _serverLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "FullName")]
        public virtual string FullName
        {
            get
            {
                if (Parent == null)
                    return Name;
                if (Name.StartsWith("."))
                    return string.Format("{0}{1}", Parent.FullName, Name);

                return string.Format("{0} {1}", Parent.FullName, Name);
            }
        }

        public virtual string GetEscapedFullName(string spaceReplacement = "_")
        {
            if (Parent == null)
                return Name.Replace(" ", spaceReplacement);
            return string.Format("{0} {1}", Parent.GetEscapedFullName(spaceReplacement), Name.Replace(" ", spaceReplacement));
        }

        public virtual EndOfSupportStatus GetEndOfSupportStatus(DateTime? date = null)
        {
            if (date == null)
                date = DateTime.Now;

            if (!EndOfSupport.HasValue)
                return EndOfSupportStatus.Unknown;
            if (EndOfSupport > date.Value.AddYears(1))
                return EndOfSupportStatus.UpToDate;
            if (EndOfSupport <= date.Value.AddYears(1) && EndOfSupport > date.Value)
                return EndOfSupportStatus.AlmostOutdated;
            return EndOfSupportStatus.Outdated;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
