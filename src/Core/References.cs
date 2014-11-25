// --------------------------------------------------------------------------------------------------------------------
// <copyright file="References.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Listeners;
using CGI.Reflex.Core.Mappings;
using CGI.Reflex.Core.Mappings.Conventions;
using CGI.Reflex.Core.Mappings.ExceptionConverters;
using CGI.Reflex.Core.Seed;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Tool.hbm2ddl;

namespace CGI.Reflex.Core
{
    public static class References
    {
        private static object _syncRoot = new object();

        public static ReferencesConfiguration ReferencesConfiguration { get; private set; }

        public static Configuration NHConfiguration { get; private set; }

        public static ISessionFactory SessionFactory { get; private set; }

        public static ISession NHSession
        {
            get
            {
                return SessionFactory == null ? null : SessionFactory.GetCurrentSession();
            }
        }

        public static User CurrentUser
        {
            get
            {
                if (ReferencesConfiguration == null)
                    return null;

                if (ReferencesConfiguration.CurrentUserCallback == null)
                    return null;

                return ReferencesConfiguration.CurrentUserCallback();
            }
        }

        private static MemoryStream CachedNHConfiguration { get; set; }

        public static void Configure(ReferencesConfiguration config)
        {
            lock (_syncRoot)
            {
                if (SessionFactory != null)
                    SessionFactory.Dispose();

                ReferencesConfiguration = config;

                if (config.CacheNHConfiguration && (CachedNHConfiguration != null))
                {
                    var serializer = new BinaryFormatter();
                    CachedNHConfiguration.Seek(0, SeekOrigin.Begin);
                    NHConfiguration = (Configuration)serializer.Deserialize(CachedNHConfiguration);
                }
                else
                {
                    NHConfiguration = BuildNHConfiguration(config);
                }

                if (config.CacheNHConfiguration && (CachedNHConfiguration == null))
                {
                    var serializer = new BinaryFormatter();
                    CachedNHConfiguration = new MemoryStream();
                    serializer.Serialize(CachedNHConfiguration, NHConfiguration);
                }

                SessionFactory = NHConfiguration.BuildSessionFactory();
            }
        }

        private static Configuration BuildNHConfiguration(ReferencesConfiguration config)
        {
            IPersistenceConfigurer dbConfig;
            Type exceptionConverterType;

            switch (config.DatabaseType)
            {
                case DatabaseType.SQLite:
                    dbConfig = string.IsNullOrEmpty(config.ConnectionString) ? SQLiteConfiguration.Standard.InMemory() : SQLiteConfiguration.Standard.ConnectionString(config.ConnectionString);
                    exceptionConverterType = typeof(SQLiteSQLExceptionConverter);
                    break;
                case DatabaseType.SqlServer2008:
                    dbConfig = MsSqlConfiguration.MsSql2008;
                    if (!string.IsNullOrEmpty(config.ConnectionString))
                        dbConfig = ((MsSqlConfiguration)dbConfig).ConnectionString(config.ConnectionString);
                    exceptionConverterType = typeof(SQLServerExceptionConverter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var nhConfig = Fluently.Configure()
                .Database(dbConfig)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Role>().Conventions.AddFromAssemblyOf<DataAnnotationsPropertyConvention>())
                .Cache(csb =>
                {
                    if (config.CacheProviderType != null)
                        csb.ProviderClass(config.CacheProviderType.AssemblyQualifiedName);

                    if (config.EnableQueryCache)
                        csb.UseQueryCache();

                    if (config.EnableSecondLevelCache)
                        csb.UseSecondLevelCache();
                })
                .BuildConfiguration();

            if (config.CurrentSessionContextType != null)
                nhConfig.SetProperty(NHibernate.Cfg.Environment.CurrentSessionContextClass, config.CurrentSessionContextType.AssemblyQualifiedName);

            nhConfig.SetProperty(NHibernate.Cfg.Environment.SqlExceptionConverter, exceptionConverterType.AssemblyQualifiedName);
            if (config.FormatSql)
                nhConfig.SetProperty(NHibernate.Cfg.Environment.FormatSql, "true");

            var dataAnnotationsEventListener = new DataAnnotationsEventListener();
            var domainValueCategoryEventListener = new DomainValueCategoryEventListener();
            var timestampedEventListener = new TimestampedEventListener();

            nhConfig.EventListeners.PreInsertEventListeners = nhConfig.EventListeners.PreInsertEventListeners
                    .Concat(new IPreInsertEventListener[] { dataAnnotationsEventListener, domainValueCategoryEventListener, timestampedEventListener })
                    .ToArray();

            nhConfig.EventListeners.PreUpdateEventListeners = nhConfig.EventListeners.PreUpdateEventListeners
                    .Concat(new IPreUpdateEventListener[] { dataAnnotationsEventListener, domainValueCategoryEventListener, timestampedEventListener })
                    .ToArray();

            if (config.EnableAudit)
            {
                var auditEventListener = new AuditEventListener();
                var forwardAuditEventListener = new ForwardAuditEventListener();

                nhConfig.EventListeners.PostInsertEventListeners = nhConfig.EventListeners.PostInsertEventListeners
                    .Concat(new IPostInsertEventListener[] { auditEventListener, forwardAuditEventListener })
                    .ToArray();

                nhConfig.EventListeners.PostUpdateEventListeners = nhConfig.EventListeners.PostUpdateEventListeners
                    .Concat(new IPostUpdateEventListener[] { auditEventListener, forwardAuditEventListener })
                    .ToArray();

                nhConfig.EventListeners.PostDeleteEventListeners = nhConfig.EventListeners.PostDeleteEventListeners
                    .Concat(new IPostDeleteEventListener[] { auditEventListener, forwardAuditEventListener })
                    .ToArray();
            }

            return nhConfig;
        }

        [ExcludeFromCodeCoverage]
        public static class DatabaseOperations
        {
            public static void ExportSchema()
            {
                if (ReferencesConfiguration == null)
                    throw new Exception("You must configure the References first");

                new SchemaExport(NHConfiguration).Execute(true, true, false);
            }

            public static string ExportSchema(ReferencesConfiguration config)
            {
                ReferencesConfiguration = config;
                var nhConfig = BuildNHConfiguration(config);

                var builder = new StringBuilder();
                new SchemaExport(nhConfig).Create(sch => builder.Append(sch), false);
                return builder.ToString();
            }

            public static void ExportSchema(ReferencesConfiguration config, string filename)
            {
                ReferencesConfiguration = config;
                var nhConfig = BuildNHConfiguration(config);

                if (File.Exists(filename))
                    File.Delete(filename);

                new SchemaExport(nhConfig).SetOutputFile(filename).Create(false, false);
            }

            public static void DropSchema()
            {
                if (ReferencesConfiguration == null)
                    throw new Exception("You must configure the References first");

                new SchemaExport(NHConfiguration).Execute(true, true, true);
            }

            public static void UpdateSchema()
            {
                if (ReferencesConfiguration == null)
                    throw new Exception("You must configure the References first");

                new SchemaUpdate(NHConfiguration).Execute(false, true);
            }

            public static string UpdateSchema(ReferencesConfiguration config)
            {
                ReferencesConfiguration = config;
                var nhConfig = BuildNHConfiguration(config);

                var builder = new StringBuilder();
                new SchemaUpdate(nhConfig).Execute(sch => builder.Append(sch), false);
                return builder.ToString();
            }

            public static void UpdateSchema(ReferencesConfiguration config, string filename)
            {
                using (var writer = new StreamWriter(filename, false))
                {
                    writer.Write(UpdateSchema(config));
                }
            }

            public static void Seed(ISession session = null, bool minimalSeed = false)
            {
                if (ReferencesConfiguration == null)
                    throw new Exception("You must configure the References first");

                var seeders = typeof(DomainValuesSeeder).Assembly.GetTypes()
                                 .Where(t => typeof(ISeeder).IsAssignableFrom(t)
                                    && !t.IsInterface
                                    && !t.IsAbstract)
                                 .Select(t => (ISeeder)Activator.CreateInstance(t))
                                 .OrderBy(s => s.Priority)
                                 .ToList();

                if (minimalSeed)
                {
                    seeders = seeders.Where(s => s.IsMinimal).ToList();
                }

                var sessionInitialized = session != null;
                if (!sessionInitialized)
                    session = SessionFactory.OpenSession();
                try
                {
                    foreach (var seeder in seeders)
                    {
                        using (var tx = session.BeginTransaction())
                        {
                            seeder.Seed(session);
                            tx.Commit();
                        }
                    }
                }
                finally
                {
                    if (!sessionInitialized)
                        session.Dispose();
                }
            }

            public static void ClearAllNHCaches()
            {
                SessionFactory.EvictQueries();
                foreach (var collectionMetadata in SessionFactory.GetAllCollectionMetadata())
                    SessionFactory.EvictCollection(collectionMetadata.Key);
                foreach (var classMetadata in SessionFactory.GetAllClassMetadata())
                    SessionFactory.EvictEntity(classMetadata.Key);
            }
        }
    }
}
