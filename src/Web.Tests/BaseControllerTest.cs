// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Infra.Controllers;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;
using NUnit.Framework;
using Rhino.Mocks;

namespace CGI.Reflex.Web.Tests
{
    [Category("Controller")]
    public abstract class BaseControllerTest : BaseDbTest
    {
        protected BaseControllerTest()
            : base(sessionContextType: typeof(AsyncThreadStaticSessionContext))
        {
        }

        protected MockRepository Mocks { get; set; }

        public override void SetUp()
        {
            base.SetUp();
            Mocks = new MockRepository();
        }

        protected void StubRequest(Controller controller, WebHeaderCollection headers)
        {
            var request = Mocks.Stub<HttpRequestBase>();
            SetupResult.For(request.Headers).Return(headers);
            Mocks.Replay(request);

            var httpServerUtilityBase = Mocks.Stub<HttpServerUtilityBase>();
            Expect.Call(httpServerUtilityBase.HtmlEncode(null))
                .IgnoreArguments()
                .Return(string.Empty);

            var context = Mocks.Stub<HttpContextBase>();
            SetupResult.For(context.Request).Return(request);
            SetupResult.For(context.Server).Return(httpServerUtilityBase);
            Mocks.Replay(context);

            var controllerContext = Mocks.Stub<ControllerContext>();
            controllerContext.HttpContext = context;
            Mocks.Replay(controllerContext);

            controller.ControllerContext = controllerContext;
        }

        public class MockInputStreamHttpPostedFileBase : HttpPostedFileBase
        {
            private readonly Stream _inputStream;

            public MockInputStreamHttpPostedFileBase(Stream inputStream)
            {
                _inputStream = inputStream;
            }

            public override Stream InputStream
            {
                get
                {
                    return _inputStream;
                }
            }
        }

        /// <summary>
        /// Equivalent of <see cref="CGI.Reflex.Web.Infra.Controllers.AsyncWebSessionContext"/> for ThreadStaticSessionContext.
        /// </summary>
        private class AsyncThreadStaticSessionContext : ThreadStaticSessionContext
        {
            public AsyncThreadStaticSessionContext(ISessionFactoryImplementor factory)
                : base(factory)
            {
            }

            public override ISession CurrentSession()
            {
                try
                {
                    return base.CurrentSession();
                }
                catch (HibernateException)
                {
                    var asyncScopedSession = AsyncScope.GetCurrentAsyncNHSession();
                    if (asyncScopedSession != null)
                        return asyncScopedSession;

                    throw;
                }
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class BaseControllerTest<T> : BaseControllerTest
        where T : BaseController, new()
    {
        protected T Controller { get; set; }

        public override void SetUp()
        {
            base.SetUp();
            Controller = new T();
        }

        protected void StubStandardRequest()
        {
            StubRequest(Controller, new WebHeaderCollection());
            Controller.Url = new UrlHelper(new RequestContext(Controller.ControllerContext.HttpContext, new RouteData()));
        }

        protected void StubAjaxRequest()
        {
            StubRequest(Controller, new WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
        }
    }
}
