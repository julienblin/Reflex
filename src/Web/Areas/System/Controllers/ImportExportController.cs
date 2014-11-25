// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportExportController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Importers;
using CGI.Reflex.Web.Areas.System.Models.ImportExport;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.System.Controllers
{
    public class ImportExportController : BaseController
    {
        [IsAllowed("/System/ImportExport")]
        public ActionResult Index(ImportConfig model)
        {
            model.ImportersList = new[] { new KeyValuePair<string, string>() }.Concat(FileImporters.GetImporters());
            return View(model);
        }

        [HttpPost]
        [IsAllowed("/System/ImportExport/Import")]
        public ActionResult GetTemplate(string name)
        {
            var result = FileImporters.GetTemplate(name);
            return File(result.Stream, result.ContentType, result.SuggestedFileName);
        }

        [HttpPost]
        [IsAllowed("/System/ImportExport/Export")]
        [NoAsyncTimeout]
        [CanBeSlow]
        public void ExportAsync(string name)
        {
            if (!string.IsNullOrEmpty(name))
                AsyncTaskExecute(() => FileImporters.Export(name));
        }

        [IsAllowed("/System/ImportExport/Export")]
        public ActionResult ExportCompleted(FileImporterResult result)
        {
            if (result == null)
                return RedirectToAction("Index");

            return File(result.Stream, result.ContentType, result.SuggestedFileName);
        }

        [HttpPost]
        [IsAllowed("/System/ImportExport/Import")]
        [NoAsyncTimeout]
        [CanBeSlow]
        public void ResultAsync(string name, HttpPostedFileBase file)
        {
            if (!string.IsNullOrEmpty(name) && file != null)
                AsyncTaskExecute(() => FileImporters.Import(name, file.InputStream));
        }

        [IsAllowed("/System/ImportExport/Import")]
        public ActionResult ResultCompleted(IEnumerable<ImportOperationLineResult> result)
        {
            if (result == null)
                return RedirectToAction("Index");

            return View(result);
        }
    }
}
