// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseFileImporter.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Importers.Models;
using CGI.Reflex.Core.Queries;
using ClosedXML.Excel;
using NHibernate;

namespace CGI.Reflex.Core.Importers
{
    internal abstract class BaseFileImporter : IFileImporter
    {
        protected const string XlsxContentType = @"application/vnd.ms-excel.12";

        private readonly string _resName;

        protected BaseFileImporter(string resName, int priority)
        {
            _resName = resName;
            Priority = priority;
        }

        public virtual string DisplayName
        {
            get
            {
                var resManager = new ResourceManager(typeof(CoreResources));
                return resManager.GetString(_resName);
            }
        }

        public int Priority { get; private set; }

        public virtual string TechnicalName { get { return _resName; } }

        public virtual FileImporterResult GetTemplate(ISession session)
        {
            var stream = new MemoryStream();
            try
            {
                var result = GetTemplateImpl(session, stream);
                result.Stream.Seek(0, SeekOrigin.Begin);
                return result;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        public virtual FileImporterResult Export(ISession session)
        {
            var stream = new MemoryStream();
            try
            {
                var result = ExportImpl(session, stream);
                result.Stream.Seek(0, SeekOrigin.Begin);
                return result;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        public virtual IEnumerable<ImportOperationLineResult> Import(ISession session, Stream inputStream)
        {
            return ImportImpl(session, inputStream);
        }

        protected virtual void CreateDomainValuesReferences(ISession session, XLWorkbook workbook, params DomainValueCategory[] categories)
        {
            var ws = workbook.Worksheets.Add("DVReferences");
            var ci = 1;
            foreach (var category in categories)
            {
                var domainValues = new DomainValueQuery { Category = category }.Over(session).OrderBy(dv => dv.DisplayOrder).Asc.List();
                var row = ws.Row(1);
                foreach (var domainValue in domainValues)
                {
                    row.Cell(ci).SetValue(domainValue.Name);
                    row = row.RowBelow();
                }

                if (row.RowNumber() == 1)
                    row = row.RowBelow();
                ws.Workbook.NamedRanges.Add(category.ToString(), ws.Range(1, ci, row.RowNumber() - 1, ci));
                ++ci;
            }

            ws.Hide();
        }

        protected virtual void CreateTechnologiesReferences(ISession session, XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("TechnoReferences");

            var technoNames = session.QueryOver<Technology>()
                                     .Fetch(t => t.Parent).Eager
                                     .Fetch(t => t.Parent.Parent).Eager
                                     .Fetch(t => t.Parent.Parent).Eager
                                     .List()
                                     .Where(t => !t.HasChildren())
                                     .Select(t => t.GetEscapedFullName())
                                     .OrderBy(s => s);
            var row = ws.Row(1);
            foreach (var technoName in technoNames)
            {
                row.Cell(1).SetValue(technoName);
                row = row.RowBelow();
            }

            if (row.RowNumber() == 1)
                row = row.RowBelow();
            ws.Workbook.NamedRanges.Add("Technologies", ws.Range(1, 1, row.RowNumber() - 1, 1));
            ws.Hide();
        }

        protected virtual IXLRow ExportChilds<TEntity, TModel>(TEntity entity, IXLRow row)
            where TEntity : BaseHierarchicalEntity<TEntity>
            where TModel : BaseImporterModel<TEntity>, new()
        {
            foreach (var child in entity.Children.OrderBy(c => c.Name))
            {
                var model = new TModel();
                model.FromEntity(child).ToRow(row);
                row = row.RowBelow();
                row = ExportChilds<TEntity, TModel>(child, row);
            }

            return row;
        }

        protected void Import<TEntity, TModel>(ISession session, ICollection<ImportOperationLineResult> result, IXLWorksheet ws, Func<ISession, TModel, TEntity> existingEntity)
            where TEntity : BaseEntity, new()
            where TModel : BaseImporterModel<TEntity>, new()
        {
            var row = ws.Row(4);
            while (!row.IsEmpty(false))
            {
                var lineResult = new ImportOperationLineResult { Section = ws.Name, LineNumber = row.RowNumber() };

                try
                {
                    var model = new TModel();
                    ComplementaryModelInfo(model, row, ws);
                    model.FromRow(row);
                    var validationResults = model.Validate();
                    if (validationResults.Any())
                    {
                        lineResult.Status = LineResultStatus.Rejected;
                        lineResult.Message = string.Join(", ", validationResults.First().ErrorMessage);
                        result.Add(lineResult);
                        row = row.RowBelow();
                        continue;
                    }

                    using (var tx = session.BeginTransaction())
                    {
                        var entity = existingEntity(session, model);
                        if (entity == null)
                        {
                            entity = new TEntity();
                            lineResult.Status = LineResultStatus.Created;
                        }
                        else
                        {
                            lineResult.Status = LineResultStatus.Merged;
                        }

                        model.ToEntity(session, entity);
                        lineResult.Message = entity.ToString();

                        session.SaveOrUpdate(entity);
                        tx.Commit();
                    }
                }
                catch (ReferenceNotFoundException refEx)
                {
                    lineResult.Status = LineResultStatus.Rejected;
                    lineResult.Message = string.Format(
                        "Reference error for {0}: value {1} not found.",
                        refEx.Name,
                        refEx.Value);
                    lineResult.Exception = refEx;
                }
                catch (Exception ex)
                {
                    lineResult.Status = LineResultStatus.Rejected;
                    lineResult.Message = ex.Message;
                    lineResult.Exception = ex;
                    session.Clear();
                }

                result.Add(lineResult);
                row = row.RowBelow();
            }
        }

        protected virtual void ComplementaryModelInfo(object model, IXLRow row, IXLWorksheet ws)
        {
        }

        protected abstract FileImporterResult GetTemplateImpl(ISession session, Stream stream);

        protected abstract FileImporterResult ExportImpl(ISession session, Stream stream);

        protected abstract IEnumerable<ImportOperationLineResult> ImportImpl(ISession session, Stream inputStream);
    }
}
