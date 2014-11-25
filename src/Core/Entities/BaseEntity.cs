// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseEntity.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    public static class BaseEntityExtensions
    {
        public static int? ToId(this BaseEntity value)
        {
            if (value == null)
                return null;
            return value.Id;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class BaseEntity : ITimestamped
    {
#pragma warning disable 649
        private int _id;

        private DateTime _lastUpdatedAtUTC;
#pragma warning restore 649

        protected BaseEntity()
        {
            _lastUpdatedAtUTC = DateTime.Now;
        }

        public virtual int Id
        {
            get { return _id; }
        }

        [NotAuditable]
        public virtual DateTime LastUpdatedAtUTC
        {
            get { return _lastUpdatedAtUTC; }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "SetLastUpdatedAtUTC only called by NHibernate interceptor")]
        void ITimestamped.SetLastUpdatedAtUTC(DateTime value)
        {
            _lastUpdatedAtUTC = value;
        }

        public virtual ICollection<ValidationResult> Validate()
        {
            var result = new Collection<ValidationResult>();
            Validator.TryValidateObject(
                this,
                new ValidationContext(this, null, new Dictionary<object, object>()),
                result,
                true);
            return result;
        }

        public virtual bool IsValid()
        {
            return Validate().Count == 0;
        }
    }
}