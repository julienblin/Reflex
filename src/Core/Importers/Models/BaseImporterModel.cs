// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseImporterModel.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using ClosedXML.Excel;
using NHibernate;

namespace CGI.Reflex.Core.Importers.Models
{
    public abstract class BaseImporterModel<T> : IImporterModel
    {
        public const int RowLimit = 10000;
        public const string DateFormat = @"yyyy-MM-dd";

        public abstract void Prepare(IXLWorksheet ws, string title = null);

        public abstract IImporterModel ToRow(IXLRow row);

        public abstract IImporterModel FromRow(IXLRow row);

        IImporterModel IImporterModel.ToEntity(ISession session, object entity)
        {
            return ToEntity(session, (T)entity);
        }

        IImporterModel IImporterModel.FromEntity(object entity)
        {
            return FromEntity((T)entity);
        }

        public abstract IImporterModel ToEntity(ISession session, T entity);

        public abstract IImporterModel FromEntity(T entity);

        public virtual IEnumerable<ValidationResult> Validate()
        {
            var result = new List<ValidationResult>();
            Validator.TryValidateObject(this, new ValidationContext(this, null, null), result);
            return result;
        }

        protected virtual void SetTitle(IXLWorksheet ws, string title)
        {
            ws.Cell(1, 1).Value = title;
            ws.Cell(1, 1).Style.Font.SetBold();
            ws.Cell(1, 1).Style.Font.SetFontSize(14);
        }

        protected virtual void SetAutoFilter(IXLWorksheet ws, int ci)
        {
            ws.Range(3, 1, 3, ci).SetAutoFilter();
        }

        protected virtual void SetColumnDataValidation(IXLWorksheet ws, int ci, string listName)
        {
            ws.Range(4, ci, RowLimit, ci).SetDataValidation().List(listName);
        }

        protected virtual void SetColumnDynamicRange(IXLWorksheet ws, string rangeName, string columnLetter)
        {
            ws.Workbook.NamedRanges.Add(rangeName, string.Format("OFFSET({0}!${1}$4,0,0,COUNTA({0}!${1}:${1})-2,1)", ws.Name, columnLetter));
        }

        protected virtual DomainValue GetDomainValueReference(ISession session, DomainValueCategory category, string refName, string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            var value = new DomainValueQuery { Category = category, Name = name }.SingleOrDefault(session);
            if (value == null)
                throw new ReferenceNotFoundException(refName, name);

            return value;
        }
    }
}
