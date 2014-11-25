// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseHierarchicalEntity.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    public abstract class BaseHierarchicalEntity<T> : BaseEntity, IHierarchicalEntity, IValidatableObject
        where T : BaseHierarchicalEntity<T>
    {
        private ICollection<T> _children;

        protected BaseHierarchicalEntity()
        {
            _children = new List<T>();
        }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        [Indexed]
        public virtual string Name { get; set; }

        public virtual IEnumerable<int> AllParentIds
        {
            get
            {
                return Parent == null ? new[] { Id } : Parent.AllParentIds.Concat(new[] { Id });
            }
        }

        public virtual IEnumerable<int> AllIds
        {
            get
            {
                var result = new List<int> { Id };
                if (HasChildren())
                {
                    foreach (var child in Children)
                        result.AddRange(child.AllIds);
                }

                return result.Distinct();
            }
        }

        [Display(ResourceType = typeof(CoreResources), Name = "Parent")]
        public virtual T Parent { get; set; }

        IHierarchicalEntity IHierarchicalEntity.Parent
        {
            get
            {
                return Parent;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Just to prevent Stackoverflow. Should never be raised.")]
        public virtual int HierarchicalLevel
        { 
            get
            {
                if (Parent == null)
                    return 0;

                // avoid stackoverflow
                if (Parent.Id == Id || Parent.AllParentIds.Contains(Id))
                    throw new Exception("An entity cannot be in the parent list");
                return Parent.HierarchicalLevel + 1;
            }
        }

        [Display(ResourceType = typeof(CoreResources), Name = "Children")]
        public virtual IEnumerable<T> Children { get { return _children; } }

        public virtual bool HasChildren()
        {
            return Children.Count() != 0;
        }

        public virtual void AddChild(T child)
        {
            child.Parent = (T)this;
            _children.Add(child);
        }

        public virtual void RemoveChild(T child)
        {
            _children.Remove(child);
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Parent != null && (Parent.Id == Id || Parent.AllParentIds.Contains(Id)))
                yield return new ValidationResult("An entity cannot be in its parent list", new[] { "Parent" });
        }
    }
}
