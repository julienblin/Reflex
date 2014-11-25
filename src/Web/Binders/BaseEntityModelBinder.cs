// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseEntityModelBinder.cs" company="CGI">
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
using CGI.Reflex.Core.Entities;
using NHibernate.Context;

namespace CGI.Reflex.Web.Binders
{
    [ExcludeFromCodeCoverage]
    public class BaseEntityModelBinder : BaseModelBinder<BaseEntity>
    {
        public override IEnumerable<Type> BindingTypes
        {
            get
            {
                return typeof(BaseEntity).Assembly.GetTypes()
                    .Where(t => typeof(BaseEntity).IsAssignableFrom(t)
                                && !t.IsAbstract);
            }
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!CurrentSessionContext.HasBind(References.SessionFactory))
            {
                var session = References.SessionFactory.OpenSession();
                session.BeginTransaction();
                CurrentSessionContext.Bind(session);
            }

            var id = GetA<int>(bindingContext, "Id");
            if (!id.HasValue || id.Value == 0)
                return Activator.CreateInstance(bindingContext.ModelType);
            return References.NHSession.Load(bindingContext.ModelType, id.Value);
        }
    }
}