// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDbTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests.Factories;
using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests
{
    [TestFixture]
    public abstract class BaseDbTest
    {
        private readonly bool _enableAudit;
        
        private readonly Type _sessionContextType;

        private ITransaction _transaction;

        protected BaseDbTest(Type sessionContextType = null, bool enableAudit = true)
        {
            _sessionContextType = sessionContextType ?? typeof(ThreadStaticSessionContext);
            _enableAudit = enableAudit;
        }

        protected AllFactories Factories { get; private set; }

        protected ISession NHSession
        {
            get { return References.NHSession; }
        }

        [SetUp]
        public virtual void SetUp()
        {
            References.Configure(
                new ReferencesConfiguration
                {
                    DatabaseType = DatabaseType.SQLite,
                    FormatSql = true,
                    EnableAudit = _enableAudit,
                    CurrentSessionContextType = _sessionContextType,
                    EncryptionKey = Rand.String(),
                    CacheNHConfiguration = true
                });

            var session = References.SessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);

            _transaction = session.BeginTransaction();

            var schemaExport = new SchemaExport(References.NHConfiguration);
            schemaExport.Execute(false, true, false, session.Connection, null);
            
            session.Save(new ReflexConfiguration());
            session.Flush();
            session.Clear();

            Factories = new AllFactories();
        }

        [TearDown]
        public void TearDown()
        {
            if (_transaction != null && _transaction.IsActive)
                _transaction.Commit();

            var session = CurrentSessionContext.Unbind(References.SessionFactory);
            if (session != null)
                session.Dispose();

            if (References.SessionFactory != null)
                References.SessionFactory.Dispose();
        }
    }
}
