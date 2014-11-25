// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries.Criterions;
using NHibernate;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class ServerQuery : BaseQueryOver<Server>
    {
        public string NameOrAliasLike { get; set; }

        public string NameLike { get; set; }

        public string AliasLike { get; set; }

        public DateTime? EvergreenDateTo { get; set; }

        public int? EnvironmentId { get; set; }

        public IEnumerable<int> EnvironmentIds { get; set; }

        public int? RoleId { get; set; }

        public IEnumerable<int> RoleIds { get; set; }

        public int? StatusId { get; set; }

        public IEnumerable<int> StatusIds { get; set; }

        public int? TypeId { get; set; }

        public IEnumerable<int> TypeIds { get; set; }

        public int? LandscapeId { get; set; }

        public int? LinkedTechnologyId { get; set; }

        public bool? WithDbInstances { get; set; }

        protected override IQueryOver<Server, Server> OverImpl(ISession session)
        {
            Server serverAlias = null;
            var query = session.QueryOver(() => serverAlias);

            if (!string.IsNullOrEmpty(NameOrAliasLike))
                query.Where(
                    Restrictions.Or(
                        Restrictions.InsensitiveLike(Projections.Property<Server>(s => s.Name), NameOrAliasLike, MatchMode.Anywhere),
                        Restrictions.InsensitiveLike(Projections.Property<Server>(s => s.Alias), NameOrAliasLike, MatchMode.Anywhere)));

            if (!string.IsNullOrEmpty(NameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Server>(s => s.Name), NameLike, MatchMode.Anywhere));
            
            if (!string.IsNullOrEmpty(AliasLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Server>(s => s.Alias), AliasLike, MatchMode.Anywhere));

            if (EvergreenDateTo.HasValue)
                query.Where(s => s.EvergreenDate <= EvergreenDateTo.Value); 

            if (EnvironmentId.HasValue)
                query.Where(s => s.Environment.Id == EnvironmentId.Value);

            if ((EnvironmentIds != null) && EnvironmentIds.Any())
                query.WhereRestrictionOn(s => s.Environment.Id).IsIn(EnvironmentIds.ToArray());

            if (RoleId.HasValue)
                query.Where(s => s.Role.Id == RoleId.Value);

            if ((RoleIds != null) && RoleIds.Any())
                query.WhereRestrictionOn(s => s.Role.Id).IsIn(RoleIds.ToArray());

            if (StatusId.HasValue)
                query.Where(s => s.Status.Id == StatusId.Value);

            if ((StatusIds != null) && StatusIds.Any())
                query.WhereRestrictionOn(s => s.Status.Id).IsIn(StatusIds.ToArray());

            if (TypeId.HasValue)
                query.Where(s => s.Type.Id == TypeId.Value);

            if ((TypeIds != null) && TypeIds.Any())
                query.WhereRestrictionOn(s => s.Type.Id).IsIn(TypeIds.ToArray());

            if (LandscapeId.HasValue)
                query.Where(s => s.Landscape.Id == LandscapeId.Value);

            if (LinkedTechnologyId.HasValue)
            {
                var allTechnoIds = new[] { LinkedTechnologyId.Value };
                var techno = new TechnologyHierarchicalQuery { RootId = LinkedTechnologyId }.SingleOrDefault(session);
                if (techno != null)
                    allTechnoIds = techno.AllIds.ToArray();

                var subServerTechnoLinksIds = session.QueryOver<ServerTechnoLink>()
                                                        .WhereRestrictionOn(stl => stl.Technology.Id).IsIn(allTechnoIds)
                                                        .SelectList(list => list.Select(stl => stl.Server.Id))
                                                        .Future<int>();

                DbInstance dbInstanceAlias = null;
                var subDbInstanceTechnoLinksIds = session.QueryOver<DbInstanceTechnoLink>()
                                                           .WhereRestrictionOn(dtl => dtl.Technology.Id).IsIn(allTechnoIds)
                                                           .JoinQueryOver(dtl => dtl.DbInstance, () => dbInstanceAlias)
                                                           .SelectList(list => list.Select(dtl => dbInstanceAlias.Server.Id))
                                                           .Future<int>();

                var serverIds = subServerTechnoLinksIds.Concat(subDbInstanceTechnoLinksIds)
                                                       .Distinct()
                                                       .ToList();

                query.Where(new XmlIn("Id", serverIds));
            }

            if (WithDbInstances.HasValue)
            {
                var subQueryInstances = QueryOver.Of<DbInstance>()
                                                 .Where(db => db.Server.Id == serverAlias.Id)
                                                 .Select(db => db.Id);
                if (WithDbInstances.Value)
                    query.WithSubquery.WhereExists(subQueryInstances);
                else
                    query.WithSubquery.WhereNotExists(subQueryInstances);
            }

            return query;
        }
    }
}
