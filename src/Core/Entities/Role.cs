// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Role.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "Role")]
    public class Role : BaseConcurrentEntity
    {
        public const string OperationSegmentSeparator = @"/";
        public const string OperationWildCard = @"*";

        private ICollection<string> _allowedOperations;

        private ICollection<string> _deniedOperations;

        public Role()
        {
            _allowedOperations = new List<string>();
            _deniedOperations = new List<string>();
        }

        [Unique]
        [Required]
        [StringLength(20)]
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public virtual string Name { get; set; }

        [StringLength(255)]
        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        public virtual string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "AllowedOperations")]
        [DataType(DataType.MultilineText)]
        public virtual IEnumerable<string> AllowedOperations { get { return _allowedOperations; } }

        [Display(ResourceType = typeof(CoreResources), Name = "DeniedOperations")]
        [DataType(DataType.MultilineText)]
        public virtual IEnumerable<string> DeniedOperations { get { return _deniedOperations; } }

        public virtual void SetAllowedOperations(IEnumerable<string> operations)
        {
            _allowedOperations = new List<string>();
            if (operations == null) return;

            foreach (var operation in operations)
                _allowedOperations.Add(operation);
        }

        public virtual void SetDeniedOperations(IEnumerable<string> operations)
        {
            _deniedOperations = new List<string>();
            if (operations == null) return;

            foreach (var operation in operations)
                _deniedOperations.Add(operation);
        }

        public virtual bool IsAllowed(string operation)
        {
            if (operation == null) throw new ArgumentNullException("operation");

            var allowedOperation = Match(operation, _allowedOperations, true);
            
            if (allowedOperation == null)
                return false;

            var deniedOperation = Match(operation, _deniedOperations, false);

            if (deniedOperation == null)
                return true;

            if (deniedOperation.Equals(operation, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (allowedOperation.Length < deniedOperation.Length)
                return true;

            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        private static string Match(string match, IEnumerable<string> operations, bool inclusive)
        {
            var matchList = new List<string>();
            foreach (var operation in operations)
            {
                if (inclusive)
                {
                    if (operation.StartsWith(match))
                    {
                        matchList.Add(operation);
                        continue;
                    }

                    if (operation.EndsWith(OperationWildCard))
                    {
                        if (match.StartsWith(operation.Replace(OperationWildCard, string.Empty)))
                            matchList.Add(operation);
                    }
                }
                else
                {
                    if (match.StartsWith(operation))
                    {
                        matchList.Add(operation);
                        continue;
                    }

                    if (operation.EndsWith(OperationWildCard))
                    {
                        if (operation.Replace(OperationWildCard, string.Empty).StartsWith(match))
                            matchList.Add(operation);
                    }                    
                }
            }

            if (matchList.Count == 0)
                return null;

            return matchList.OrderBy(s => s.Length - s.Replace(OperationSegmentSeparator, string.Empty).Length).First();
        }
    }
}