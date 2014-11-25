// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerTechnoQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class ServerTechnoQuery
    {
        public int ServerId { get; set; }

        public ServerTechnoQueryResult Execute()
        {
            return Execute(References.NHSession);
        }

        private ServerTechnoQueryResult Execute(ISession session)
        {
            var result = new ServerTechnoQueryResult();

            var subServerApp = QueryOver.Of<AppServerLink>()
                .Where(sl => sl.Server.Id == ServerId)
                .SelectList(list => list.Select(sl => sl.Application.Id));

            var subAppTechno = QueryOver.Of<AppTechnoLink>()
                .WithSubquery.WhereProperty(tl => tl.Application.Id).In(subServerApp)
                .SelectList(
                    list =>
                    list.Select(Projections.Distinct(Projections.Property<AppTechnoLink>(tl => tl.Technology.Id))));

            result.ApplicationTechnologies = session.QueryOver<Technology>()
                .WithSubquery.WhereProperty(t => t.Id).In(subAppTechno)
                .Fetch(t => t.TechnologyType).Eager
                .Fetch(t => t.Parent).Eager
                .Fetch(t => t.Parent.Parent).Eager
                .Fetch(t => t.Parent.Parent.Parent).Eager
                .Future();

            var subServerTechQuery = QueryOver.Of<Server>()
                .Where(s => s.Id == ServerId)
                .SelectList(list => list.Select(s => s.Id));

            var subServerTechnoLinksQuery = QueryOver.Of<ServerTechnoLink>()
                .WithSubquery.WhereProperty(stl => stl.Server.Id).In(subServerTechQuery)
                .SelectList(
                    list =>
                    list.Select(Projections.Distinct(Projections.Property<ServerTechnoLink>(stl => stl.Technology.Id))));

            result.ServersTechnologies = session.QueryOver<Technology>()
                .WithSubquery.WhereProperty(t => t.Id).In(subServerTechnoLinksQuery)
                .Fetch(t => t.TechnologyType).Eager
                .Fetch(t => t.Parent).Eager
                .Fetch(t => t.Parent.Parent).Eager
                .Fetch(t => t.Parent.Parent.Parent).Eager
                .Future();

            var subDbInstanceQuery = QueryOver.Of<DbInstance>()
                .Where(db => db.Server.Id == ServerId)
                .SelectList(list => list.Select(db => db.Id));

            var subDbInstanceTechnoQuery = QueryOver.Of<DbInstanceTechnoLink>()
                .WithSubquery.WhereProperty(ditl => ditl.DbInstance.Id).In(subDbInstanceQuery)
                .SelectList(
                    list =>
                    list.Select(Projections.Distinct(Projections.Property<DbInstanceTechnoLink>(d => d.Technology.Id))));

            result.DbInstanceTechnologies = session.QueryOver<Technology>()
                .WithSubquery.WhereProperty(t => t.Id).In(subDbInstanceTechnoQuery)
                .Fetch(t => t.TechnologyType).Eager
                .Fetch(t => t.Parent).Eager
                .Fetch(t => t.Parent.Parent).Eager
                .Fetch(t => t.Parent.Parent.Parent).Eager
                .Future();

            return result;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ServerTechnoQueryResult
    {
        public IEnumerable<Technology> ServersTechnologies { get; set; }

        public IEnumerable<Technology> ApplicationTechnologies { get; set; }

        public IEnumerable<Technology> DbInstanceTechnologies { get; set; }
    }
}