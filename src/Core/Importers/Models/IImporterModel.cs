// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImporterModel.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using ClosedXML.Excel;
using NHibernate;

namespace CGI.Reflex.Core.Importers.Models
{
    public interface IImporterModel
    {
        void Prepare(IXLWorksheet ws, string title = null);

        IImporterModel ToRow(IXLRow row);

        IImporterModel FromRow(IXLRow row);

        IImporterModel ToEntity(ISession session, object entity);

        IImporterModel FromEntity(object entity);

        IEnumerable<ValidationResult> Validate();
    }
}
