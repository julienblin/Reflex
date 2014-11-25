// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValue.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    public static class DomainValueExtensions
    {
        public static DomainValue ToDomainValue(this int value)
        {
            return References.NHSession.Load<DomainValue>(value);
        }

        public static DomainValue ToDomainValue(this int? value)
        {
            if (!value.HasValue)
                return null;

            return References.NHSession.Load<DomainValue>(value.Value);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    [Auditable(typeof(CoreResources), "DomainValue")]
    public class DomainValue : BaseConcurrentEntity
    {
        [NotAuditable]
        [Display(ResourceType = typeof(CoreResources), Name = "Category")]
        public virtual DomainValueCategory Category { get; set; }

        [NotAuditable]
        [Display(ResourceType = typeof(CoreResources), Name = "DisplayOrder")]
        public virtual int DisplayOrder { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(25)]
        public virtual string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Color")]
        public virtual Color Color { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}