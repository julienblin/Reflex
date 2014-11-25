// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileImporters.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NHibernate;

namespace CGI.Reflex.Core.Importers
{
    public static class FileImporters
    {
        private static readonly object SyncRoot = new object();

        private static volatile IEnumerable<IFileImporter> _importers;

        private static IEnumerable<IFileImporter> Importers
        {
            get
            {
                if (_importers == null)
                {
                    lock (SyncRoot)
                    {
                        if (_importers == null)
                        {
                            _importers = typeof(FileImporters).Assembly.GetTypes()
                                .Where(t => typeof(IFileImporter).IsAssignableFrom(t)
                                            && !t.IsInterface
                                            && !t.IsAbstract)
                                .Select(t => (IFileImporter)Activator.CreateInstance(t))
                                .OrderBy(t => t.Priority);
                        }
                    }
                }

                return _importers;
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> GetImporters()
        {
            return Importers.Select(crit => new KeyValuePair<string, string>(crit.TechnicalName, crit.DisplayName));
        }

        public static FileImporterResult GetTemplate(string technicalName, ISession session = null)
        {
            var importer = GetImporter(technicalName);

            if (session == null)
                session = References.NHSession;

            return importer.GetTemplate(session);
        }

        public static FileImporterResult Export(string technicalName, ISession session = null)
        {
            var importer = GetImporter(technicalName);

            if (session == null)
                session = References.NHSession;

            return importer.Export(session);
        }

        public static IEnumerable<ImportOperationLineResult> Import(string technicalName, Stream inputStream, ISession session = null)
        {
            var importer = GetImporter(technicalName);

            if (session == null)
                session = References.NHSession;

            return importer.Import(session, inputStream);
        }

        private static IFileImporter GetImporter(string technicalName)
        {
            var importer = Importers.FirstOrDefault(i => i.TechnicalName == technicalName);
            if (importer == null) throw new NotSupportedException();
            return importer;
        }
    }
}
