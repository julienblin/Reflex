// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexDataAnnotationsModelValidator.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Infra
{
    /// <summary>
    /// Custom <see cref="DataAnnotationsModelValidator"/> that fill <see cref="ValidationContext.MemberName"/> correctly.
    /// </summary>
    /// <remarks>
    /// <see href="http://stackoverflow.com/questions/7447932/mvc-3-model-validation-issue-oversight-or-by-design" />
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class ReflexDataAnnotationsModelValidator : DataAnnotationsModelValidator
    {
        public ReflexDataAnnotationsModelValidator(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            var context = new ValidationContext(container ?? Metadata.Model, null, null)
            {
                DisplayName = Metadata.GetDisplayName(),
                MemberName = Metadata.PropertyName
            };

            var result = Attribute.GetValidationResult(Metadata.Model, context);
            if (result != null && result != ValidationResult.Success)
            {
                yield return new ModelValidationResult
                {
                    Message = result.ErrorMessage
                };
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    [ExcludeFromCodeCoverage]
    public class ReflexDataAnnotationsModelValidator<TAttribute> : ReflexDataAnnotationsModelValidator
        where TAttribute : ValidationAttribute
    {
        public ReflexDataAnnotationsModelValidator(ModelMetadata metadata, ControllerContext context, TAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        protected new TAttribute Attribute
        {
            get { return (TAttribute)base.Attribute; }
        }
    }
}