// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileImporter.cs" company="CGI">
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
    internal interface IFileImporter
    {
        string TechnicalName { get; }

        string DisplayName { get; }

        int Priority { get; }

        FileImporterResult GetTemplate(ISession session);

        FileImporterResult Export(ISession session);

        IEnumerable<ImportOperationLineResult> Import(ISession session, Stream inputStream);
    }
}
