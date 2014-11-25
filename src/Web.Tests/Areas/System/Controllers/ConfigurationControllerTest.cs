// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Areas.System.Controllers;
using CGI.Reflex.Web.Areas.System.Models.Configuration;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.System.Controllers
{
    public class ConfigurationControllerTest : BaseControllerTest<ConfigurationController>
    {
        [Test]
        public void Index_should_return_view()
        {
            var oneStatus = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);
            var result = Controller.Index();
            result.Should().BeDefaultView();
            result.Model<ReflexConfig>().ActiveAppStatusDVIds.Should().BeEquivalentTo(ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds);
            result.Model<ReflexConfig>().ApplicationStatuses.Should().Contain(oneStatus);
        }

        [Test]
        public void Index_should_save_config()
        {
            var status1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);
            var status2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);

            var config = ReflexConfiguration.GetCurrent();

            var result = Controller.Index(new ReflexConfig { ActiveAppStatusDVIds = new[] { status1.Id, status2.Id } });
            result.Should().BeRedirectToActionName("Index");

            NHSession.Flush();
            NHSession.Clear();
            config = ReflexConfiguration.GetCurrent();
            config.ActiveAppStatusDVIds.Should().OnlyContain(i => i == status1.Id || i == status2.Id);
        }
    }
}
