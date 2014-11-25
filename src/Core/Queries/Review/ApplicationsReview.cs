// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsReview.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using NHibernate;
using NHibernate.Transform;

namespace CGI.Reflex.Core.Queries.Review
{
    public class ApplicationsReview : SingleResultQuery<ApplicationsReviewResult>
    {
        public int? AppId { get; set; }

        public bool OnlyActiveApplications { get; set; }

        public override ApplicationsReviewResult Execute(ISession session)
        {
            var result = new ApplicationsReviewResult();
            IQueryOver<Application, Application> query;

            // Fetching strategy is different depending on the scope
            // Global: we issue multiple queries in parallel and let NHibernate reconcile
            // Local: we fecth everything since the scope is tight.
            if (!AppId.HasValue)
            {
                session.QueryOver<Technology>().Future();
                session.QueryOver<Server>()
                       .Fetch(s => s.TechnologyLinks).Eager
                       .Future();
                session.QueryOver<DbInstance>()
                       .Fetch(dbi => dbi.TechnologyLinks).Eager
                       .Future();
                session.QueryOver<Integration>()
                       .Fetch(i => i.TechnologyLinks).Eager
                       .Future();
                session.QueryOver<ApplicationReview>().Future();
                query = session.QueryOver<Application>()
                               .Fetch(a => a.TechnologyLinks).Eager
                               .Fetch(a => a.ServerLinks).Eager
                               .Fetch(a => a.ReviewAnswers).Eager;

                if (OnlyActiveApplications)
                {
                    var activeAppStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                    query.JoinQueryOver(ai => ai.AppInfo).WhereRestrictionOn(ai => ai.Status.Id).IsIn(activeAppStatuses.ToArray());
                }
            }
            else
            {
                query = session.QueryOver<Application>()
                               .Where(a => a.Id == AppId)
                               .Fetch(a => a.TechnologyLinks).Eager
                               .Fetch(a => a.TechnologyLinks.First().Technology).Eager
                               .Fetch(a => a.ServerLinks).Eager
                               .Fetch(a => a.ServerLinks.First().Server).Eager
                               .Fetch(a => a.ServerLinks.First().Server.TechnologyLinks).Eager
                               .Fetch(a => a.ServerLinks.First().Server.TechnologyLinks.First().Technology).Eager
                               .Fetch(a => a.IntegrationsAsDest).Eager
                               .Fetch(a => a.IntegrationsAsDest.First().TechnologyLinks).Eager
                               .Fetch(a => a.IntegrationsAsDest.First().TechnologyLinks.First().Technology).Eager
                               .Fetch(a => a.IntegrationsAsSource).Eager
                               .Fetch(a => a.IntegrationsAsSource.First().TechnologyLinks).Eager
                               .Fetch(a => a.IntegrationsAsSource.First().TechnologyLinks.First().Technology).Eager
                               .Fetch(a => a.ReviewAnswers).Eager
                               .Fetch(a => a.ReviewAnswers.First().Answer).Eager
                               .Fetch(a => a.ReviewAnswers.First().Answer.Question).Eager
                               .Fetch(a => a.DbInstanceLinks).Eager
                               .Fetch(a => a.DbInstanceLinks.First().DbInstances).Eager
                               .Fetch(a => a.DbInstanceLinks.First().DbInstances.TechnologyLinks).Eager
                               .Fetch(a => a.DbInstanceLinks.First().DbInstances.TechnologyLinks.First().Technology).Eager;
            }

            var questionsList = session
                                .QueryOver<Question>()
                                .Fetch(q => q.Answers).Eager
                                .TransformUsing(Transformers.DistinctRootEntity)
                                .List();

            result.Resulsts = query.TransformUsing(Transformers.DistinctRootEntity).Future<Application>().Select(a => new ApplicationsReviewResultLine
            {
                BusinessValue = a.GetBusinessValue(questionsList),
                TechnologyValue = a.GetTechnologyValue(questionsList)
            });

            return result;
        }
    }
}
