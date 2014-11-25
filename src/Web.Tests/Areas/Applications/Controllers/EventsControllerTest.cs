// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models;
using CGI.Reflex.Web.Areas.Applications.Models.Events;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
   public class EventsControllerTest : BaseControllerTest<EventsController>
    {
       [Test]
       public void Details_should_return_view()
       {
           StubAjaxRequest();
           Controller.Details(Rand.Int(int.MaxValue), new EventsDetails()).Should().BeHttpNotFound();

           var application = Factories.Application.Save();
           var result = Controller.Details(application.Id, new EventsDetails());

           result.Should().BePartialView("_List");
           result.Model<EventsDetails>().SearchDefined.Should().BeFalse();
       }

       [Test]
       public void Details_should_return_default_view()
       {
           StubStandardRequest();
           Controller.Details(Rand.Int(int.MaxValue), new EventsDetails()).Should().BeHttpNotFound();

           var application = Factories.Application.Save();

           var result = Controller.Details(application.Id, new EventsDetails());

           result.Should().BeDefaultView();
       }

       [Test]
       public void Create_should_return_view()
       {
           var application = Factories.Application.Save();

           var result = Controller.Create(application.Id);
           result.Should().BeDefaultView();
           result.Model<EventEdit>().FormAction.Should().Be("Create");
       }

       [Test]
       public void Create_should_create_event()
       {
           var application = Factories.Application.Save();

           var eventEdit = new EventEdit
           {
              Date = Rand.DateTime(),
              Source = Rand.String(),
              Description = Rand.String(),
              Type = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType).Id  
           };

           StubStandardRequest();
           Controller.Create(application.Id, eventEdit);

           var evt = NHSession.QueryOver<Event>().SingleOrDefault();
           evt.Should().NotBeNull();
           evt.Date.Should().Be(eventEdit.Date.Value);
           evt.Description.Should().Be(eventEdit.Description);
           evt.Source.Should().Be(eventEdit.Source);
           evt.Type.Id.Should().Be(eventEdit.Type.Value);
           evt.Application.Should().Be(application);
       }

       [Test]
       public void Edit_should_return_model_based_on_id()
       {
           // Arrange
           var ev = Factories.Event.Save();

           // Act          
           var result = Controller.Edit(ev.Id);

           // Assert
           result.Should().BeDefaultView();
           result.Model<EventEdit>().Should().NotBeNull();
           result.Model<EventEdit>().Id.Should().Be(ev.Id);
           result.Model<EventEdit>().ConcurrencyVersion.Should().Be(ev.ConcurrencyVersion);
           result.Model<EventEdit>().Date.Should().Be(ev.Date);
           result.Model<EventEdit>().Description.Should().Be(ev.Description);
           result.Model<EventEdit>().Source.Should().Be(ev.Source);
           result.Model<EventEdit>().Type.Should().Be(ev.Type.Id);
           result.Model<EventEdit>().FormAction.Should().Be("Edit");
       }

       [Test]
       public void Edit_should_return_model_based_on_Event()
       {
           // Arrange
           var ev = Factories.Event.Save();

           var eventEdit = new EventEdit
           {
               Id = ev.Id,
               Date = Rand.DateTime(),
               Source = Rand.String(),
               Description = Rand.String(),
               Type = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType).Id,
               ConcurrencyVersion = ev.ConcurrencyVersion
           };

           // Act        
           StubStandardRequest();
           var result = Controller.Edit(ev.Id, eventEdit);
           
           // Assert
           ev.Date.Should().Be(eventEdit.Date.Value);
           ev.Source.Should().Be(eventEdit.Source);
           ev.Description.Should().Be(eventEdit.Description);
           ev.Type.Id.Should().Be(eventEdit.Type.Value);
       }

       [Test]
       public void Delete_based_on_id()
       {
           var ev = Factories.Event.Save();
           var result = Controller.Delete(ev.Id);

           result.Should().BePartialView("_DeleteModal");
       }

       [Test]
       public void Delete_based_on_Event()
       {
           var ev = Factories.Event.Save();

           var eventEdit = new EventEdit
           {
               Id = ev.Id,
           };

           var result = Controller.Delete(eventEdit);
           result.Should().BeOfType<RedirectToRouteResult>();
           NHSession.Get<Event>(ev.Id).Should().BeNull();
       }
    }
}
