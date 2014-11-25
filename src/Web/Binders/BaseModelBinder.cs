// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseModelBinder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Binders
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseModelBinder : IModelBinder
    {
        public abstract IEnumerable<Type> BindingTypes { get; }

        public abstract object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext);

        protected T? GetA<T>(ModelBindingContext bindingContext, string key)
            where T : struct
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + key);

            // Didn't work? Try without the prefix if needed...
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix)
                valueResult = bindingContext.ValueProvider.GetValue(key);

            return valueResult == null ? null : (T?)valueResult.ConvertTo(typeof(T));
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    [ExcludeFromCodeCoverage]
    public abstract class BaseModelBinder<T> : BaseModelBinder
    {
        public override IEnumerable<Type> BindingTypes
        {
            get { yield return typeof(T); }
        }
    }
}