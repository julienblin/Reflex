// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportExportControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CGI.Reflex.Core.Importers;
using CGI.Reflex.Web.Areas.System.Controllers;
using CGI.Reflex.Web.Areas.System.Models.ImportExport;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.System.Controllers
{
    public class ImportExportControllerTest : BaseControllerTest<ImportExportController>
    {
        [Test]
        public void Index_should_return_view()
        {
            var result = Controller.Index(new ImportConfig());
            result.Should().BeDefaultView();
            result.Model<ImportConfig>().Should().NotBeNull();
            result.Model<ImportConfig>().ImportersList.Count().Should().BeGreaterThan(0);
        }

        [Test]
        public void GetTemplate_should_return_file()
        {
            var result = Controller.GetTemplate("DomainValues");
            result.Should().BeFileStreamResult();
        }

        [Test]
        public void Export_should_run()
        {
            var trigger = new AutoResetEvent(false);
            Controller.AsyncManager.Finished += (sender, ev) => trigger.Set();

            Controller.ExportAsync("DomainValues");
            trigger.WaitOne();
            Controller.AsyncManager.Parameters["result"].Should().BeOfType<FileImporterResult>();
        }

        [Test]
        public void Import_should_run()
        {
            using (var export = FileImporters.Export("DomainValues"))
            {
                var trigger = new AutoResetEvent(false);
                Controller.AsyncManager.Finished += (sender, ev) => trigger.Set();

                Controller.ResultAsync("DomainValues", new MockInputStreamHttpPostedFileBase(export.Stream));
                trigger.WaitOne();
                var asyncResult = Controller.AsyncManager.Parameters["result"] as IEnumerable<ImportOperationLineResult>;
                asyncResult.Should().NotBeNull();
            }
        }
    }
}
