// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniqueAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using NHibernate.Criterion;

namespace CGI.Reflex.Core.Attributes
{
    public enum UniqueMatchMode
    {
        CaseInsensitive,
        CaseSensitive
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    [ExcludeFromCodeCoverage]
    public sealed class UniqueAttribute : ValidationAttribute
    {
        private readonly Type _entityType;
        private readonly string _propertyName;
        private readonly string _idPropertyName;

        public UniqueAttribute()
        {
        }

        public UniqueAttribute(Type entityType)
        {
            _entityType = entityType;
        }

        public UniqueAttribute(Type entityType, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("propertyName");

            _entityType = entityType;
            _propertyName = propertyName;
        }

        public UniqueAttribute(Type entityType, string propertyName, string idPropertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("propertyName");
            if (string.IsNullOrEmpty(idPropertyName)) throw new ArgumentException("idPropertyName");

            _entityType = entityType;
            _propertyName = propertyName;
            _idPropertyName = idPropertyName;
        }

        public UniqueMatchMode MatchMode { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (validationContext.Items != null && validationContext.Items.ContainsKey("NHEvent"))
                return ValidationResult.Success;

            var targetObjectType = GetEntityType(validationContext);

            using (var session = References.SessionFactory.OpenStatelessSession())
            {
                var criteria = session.CreateCriteria(targetObjectType);
                switch (MatchMode)
                {
                    case UniqueMatchMode.CaseInsensitive:
                        criteria.Add(Restrictions.InsensitiveLike(GetMemberName(validationContext), value));
                        break;
                    case UniqueMatchMode.CaseSensitive:
                        criteria.Add(Restrictions.Eq(GetMemberName(validationContext), value));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var idPropertyName = GetIdPropertyName(validationContext);

                var idProperty = validationContext.ObjectType.GetProperty(idPropertyName);
                if (idProperty == null)
                    throw new Exception(string.Format("Property {0} doesn't exists on {1}. Cannot validate uniqueness of {2}", idPropertyName, targetObjectType, GetMemberName(validationContext)));

                var idValue = idProperty.GetGetMethod().Invoke(validationContext.ObjectInstance, new object[0]);
                criteria.Add(Restrictions.Not(Restrictions.IdEq(idValue)));

                var numberOfExisting = criteria.SetProjection(Projections.RowCount()).UniqueResult<int>();

                if (numberOfExisting == 1)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

                return ValidationResult.Success;
            }
        }

        private Type GetEntityType(ValidationContext validationContext)
        {
            return _entityType ?? validationContext.ObjectType;
        }

        private string GetMemberName(ValidationContext validationContext)
        {
            return _propertyName ?? validationContext.MemberName;
        }

        private string GetIdPropertyName(ValidationContext validationContext)
        {
            return string.IsNullOrEmpty(_idPropertyName) ? References.SessionFactory.GetClassMetadata(GetEntityType(validationContext)).IdentifierPropertyName : _idPropertyName;
        }
    }
}