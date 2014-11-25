// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Log;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Binders;
using CGI.Reflex.Web.Configuration;
using CGI.Reflex.Web.Controllers;
using CGI.Reflex.Web.Infra;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Infra.Log;
using log4net;
using NHibernate.Context;

namespace CGI.Reflex.Web
{
    [ExcludeFromCodeCoverage]
    public class MvcApplication : HttpApplication
    {
        public const string CurrentUserContextKey = @"CurrentUser";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        public static ReflexConfigurationSection Configuration
        {
            get { return ReflexConfigurationSection.GetConfigurationSection(); }
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LoggingContextActionFilterAttribute());
            filters.Add(new SlowRequestAlertActionFilterAttribute());
            filters.Add(new NHibernateSessionActionFilterAttribute());
            filters.Add(new CurrentUserActionFilterAttribute());
            filters.Add(new ViewBagDataActionFilterAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Error - 404",
                "NotFound",
                new { controller = "Error", action = "NotFound" });

            routes.MapRoute(
                "Error - 500",
                "ServerError",
                new { controller = "Error", action = "ServerError" });

            routes.MapRoute(
                "Login", 
                "login", 
                new { controller = "UserSessions", action = "Login", returnUrl = UrlParameter.Optional });

            routes.MapRoute(
                "Logout", 
                "logout", 
                new { controller = "UserSessions", action = "Logout" });

            routes.MapRoute(
                "Boarding", 
                "boarding", 
                new { controller = "UserSessions", action = "Boarding" });

            routes.MapRoute(
                "History", 
                "History/{entity}/{id}/{propertyName}", 
                new { controller = "History", action = "View", propertyName = UrlParameter.Optional });

            routes.MapRoute(
               "SummaryDetails", 
               "SummaryDetails", 
               new { controller = "Home", action = "SummaryDetails", propertyName = UrlParameter.Optional });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        protected static void RegisterBundles()
        {
            BundleTable.Bundles.IgnoreList.Clear();
            var javascriptBundle = new Bundle("~/Content/bundles/js").Include(
                "~/Content/js/jquery.js",
                "~/Content/js/jquery.validate.js",
                "~/Content/js/jquery.validate.unobtrusive.js",
                "~/Content/js/jquery.unobtrusive-ajax.js",
                "~/Content/js/bootstrap.js",
                "~/Content/js/bootstrap-datepicker.js",
                "~/Content/js/select2.js",
                "~/Content/js/jquery.jstree.js",
                "~/Content/js/reflex.js");

            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("fr", StringComparison.InvariantCultureIgnoreCase))
            {
                javascriptBundle.Include("~/Content/js/jquery.validate.messages_fr.js", "~/Content/js/select2_locale_fr.js");
            }

            BundleTable.Bundles.Add(javascriptBundle);

            BundleTable.Bundles.Add(
                new Bundle("~/Content/bundles/css").Include(
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/bootstrap-responsive.min.css",
                "~/Content/css/datepicker.css",
                "~/Content/css/select2.css",
                "~/Content/css/reflex.css",
                "~/Content/css/reflex-style.css"));
        }

        protected void Application_Start()
        {
            ConfigureLogging();
            try
            {
                Logger.Info("Starting Reflex application...");
                HtmlHelper.ClientValidationEnabled = true;
                HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(new RazorViewEngine());

                DataAnnotationsModelValidatorProvider.RegisterDefaultAdapter(typeof(ReflexDataAnnotationsModelValidator));

                AreaRegistration.RegisterAllAreas();

                RegisterGlobalFilters(GlobalFilters.Filters);
                RegisterRoutes(RouteTable.Routes);
                RegisterCustomBinders();
                RegisterBundles();

                var connectionString = ConfigurationManager.ConnectionStrings["reflex"];
                var reflexConfigSection = ReflexConfigurationSection.GetConfigurationSection();

                References.Configure(new ReferencesConfiguration
                {
                    ConnectionString = connectionString.ConnectionString, 
                    CurrentSessionContextType = typeof(AsyncWebSessionContext), 
                    CurrentUserCallback = CurrentUserCallback, 
                    DatabaseType = connectionString.ProviderName.Contains("SQLite") ? DatabaseType.SQLite : DatabaseType.SqlServer2008, 
                    EnableAudit = true, 
                    FormatSql = Configuration.Environment == ConfigEnvironment.Development, 
                    EncryptionKey = Configuration.EncryptionKey, 
                    CacheProviderType = typeof(NHibernate.Caches.SysCache2.SysCacheProvider), 
                    EnableQueryCache = reflexConfigSection.EnableASPNetCache, 
                    EnableSecondLevelCache = reflexConfigSection.EnableASPNetCache
                });

                DBLogAppender.ApplyConfigurationFromReferences();

                if (Configuration.RecreateDatabase && Configuration.Environment != ConfigEnvironment.Production)
                {
                    Logger.Info("Creating database...");
                    References.DatabaseOperations.ExportSchema();
                    References.DatabaseOperations.Seed();
                    Logger.Info("Database created.");
                }

                using (var session = References.SessionFactory.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    if (session.Get<ReflexConfiguration>(ReflexConfiguration.OnlyId) == null)
                        session.Save(new ReflexConfiguration());
                    tx.Commit();
                }

                Logger.Info("Reflex application started.");
            }
            catch (Exception ex)
            {
                Logger.Error("There has been an error while starting application.", ex);
                throw;
            }
        }

        private static User CurrentUserCallback()
        {
            if (HttpContext.Current == null)
                return AsyncScope.GetCurrentAsyncUser();

            return (User)HttpContext.Current.Items[CurrentUserContextKey];
        }

        private static void ConfigureLogging()
        {
            log4net.Config.XmlConfigurator.Configure();
            GlobalContext.Properties[@"CorrelationId"] = new CorrelationIdLogProvider();
            GlobalContext.Properties[@"User"] = new UserLogProvider();
            GlobalContext.Properties[@"Context"] = new ContextLogProvider();
            GlobalContext.Properties[@"Environment"] = new ReflexConfigurationEnvironmentLogProvider();
        }

        private static void RegisterCustomBinders()
        {
            var customBinders = typeof(MvcApplication).Assembly.GetTypes()
                .Where(t => typeof(BaseModelBinder).IsAssignableFrom(t)
                            && !t.IsAbstract
                            && !t.IsInterface)
                .Select(t => (BaseModelBinder)Activator.CreateInstance(t))
                .ToList();

            foreach (var customBinder in customBinders)
                foreach (var bindingType in customBinder.BindingTypes)
                    ModelBinders.Binders[bindingType] = customBinder;
        }
    }
}