// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.System.Controllers;
using CGI.Reflex.Web.Areas.System.Models.Logs;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.System.Controllers
{
    public class LogsControllerTest : BaseControllerTest<LogsController>
    {
        [Test]
        public void Index_should_return_view()
        {
            Factories.LogEntry.Save();
            Factories.LogEntry.Save();

            StubStandardRequest();

            var result = Controller.Index(new LogsIndex());
            result.Should().BeDefaultView();
            result.Model<LogsIndex>().Should().NotBeNull();
            result.Model<LogsIndex>().Levels.Count().Should().BeGreaterThan(0);
            result.Model<LogsIndex>().Loggers.Count().Should().BeGreaterThan(0);
            result.Model<LogsIndex>().SearchDefined.Should().BeFalse();
        }

        [Test]
        public void Index_should_return_view_with_ajax()
        {
            StubAjaxRequest();

            var result = Controller.Index(new LogsIndex());
            result.Should().BePartialView("_List");
            result.Model<LogsIndex>().Should().NotBeNull();
        }

        [Test]
        public void DeleteSearch_should_delete_search_results()
        {
            var logEntry1 = Factories.LogEntry.Save();
            Factories.LogEntry.Save();
            Factories.LogEntry.Save();

            StubStandardRequest();

            var result = Controller.DeleteSearch(new LogsIndex { CorrelationId = logEntry1.CorrelationId });
            result.Should().BeRedirectToActionName("Index");
            NHSession.QueryOver<LogEntry>().RowCount().Should().Be(2);
        }

        [Test]
        public void DeleteModal_should_return_view()
        {
            var logEntry = Factories.LogEntry.Save();

            Controller.DeleteModal(logEntry.Id + 1).Should().BeHttpNotFound();
            var result = Controller.DeleteModal(logEntry.Id);

            result.Should().BePartialView("_DeleteModal");
            NHSession.Get<LogEntry>(logEntry.Id).Should().NotBeNull();
        }

        [Test]
        public void Delete_should_delete_log()
        {
            var logEntry = Factories.LogEntry.Save();

            Controller.Delete(logEntry.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Delete(logEntry.Id);

            result.Should().BeRedirectToActionName("Index");
            NHSession.Get<LogEntry>(logEntry.Id).Should().BeNull();
        }
    }
}
