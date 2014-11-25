// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferencesConfiguration.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Listeners;
using CGI.Reflex.Core.Utilities;

namespace CGI.Reflex.Core
{
    public enum DatabaseType
    {
        SqlServer2008,
        SQLite
    }

    public class ReferencesConfiguration
    {
        /// <summary>
        /// Type of database to use
        /// </summary>
        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// The connection string to the database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Callback function that can return the currently connected <see cref="User"/>.
        /// Returns null if no user is currently authenticated.
        /// </summary>
        public Func<User> CurrentUserCallback { get; set; }

        /// <summary>
        /// true to format SQL correctly, false otherwise
        /// </summary>
        public bool FormatSql { get; set; }

        /// <summary>
        /// true to enable audit of persisting changes
        /// </summary>
        /// <see cref="AuditableAttribute"/>
        /// <see cref="ForwardAuditAttribute"/>
        /// <see cref="AuditableCollectionReferenceAttribute"/>
        /// <see cref="AuditEventListener"/>
        /// <see cref="ForwardAuditEventListener"/>
        public bool EnableAudit { get; set; }

        /// <summary>
        /// Must provide a type that implements <see cref="NHibernate.Context.ICurrentSessionContext" /> for session context management.
        /// </summary>
        /// <remarks>
        /// <see href="http://nhforge.org/doc/nh/en/index.html#architecture-current-session" />
        /// </remarks>
        public Type CurrentSessionContextType { get; set; }

        /// <summary>
        /// If second level cache or query cache is enabled, must provide a type that implements <see cref="NHibernate.Cache.ICacheProvider"/>
        /// </summary>
        /// <remarks>
        /// <see href="http://nhforge.org/doc/nh/en/index.html#performance-cache" />
        /// </remarks>
        public Type CacheProviderType { get; set; }

        /// <summary>
        /// true to enable NHibernate query cache
        /// </summary>
        public bool EnableQueryCache { get; set; }

        /// <summary>
        /// true to enable NHibernate second level cache
        /// </summary>
        public bool EnableSecondLevelCache { get; set; }

        /// <summary>
        /// Encryption key used by the <see cref="Encryption"/> class methods.
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// In-Memory static cache for NHibernate configuration - primarily used for Unit testing scenarios.
        /// </summary>
        public bool CacheNHConfiguration { get; set; }
    }
}
