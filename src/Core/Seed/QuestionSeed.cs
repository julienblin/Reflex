// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionSeed.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Calculation;
using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public class QuestionSeed : BaseSeeder
    {
        public override int Priority { get { return 5; } }

        protected override void SeedImpl()
        {
            var q = new Question
            {
                Name = "Rencontre les besoins d'affaires",
                Description = string.Empty,
                Weight = 30,
                Type = QuestionType.BusinessValue
            };
            q.AddAnswer(new QuestionAnswer { Value = 10, Name = "Totalement" });
            q.AddAnswer(new QuestionAnswer { Value = 5, Name = "Normalement" });
            q.AddAnswer(new QuestionAnswer { Value = 3, Name = "Partiellement" });
            q.AddAnswer(new QuestionAnswer { Value = 1, Name = "Pas du tout" });
            Session.Save(q);

            q = new Question
            {
                Name = "Importance des processus d'affaires supportées",
                Description = "Description de la question",
                Weight = 70,
                Type = QuestionType.BusinessValue
            };

            q.AddAnswer(new QuestionAnswer { Value = 10, Name = "Très important" });
            q.AddAnswer(new QuestionAnswer { Value = 5, Name = "Important" });
            q.AddAnswer(new QuestionAnswer { Value = 3, Name = "Support" });
            q.AddAnswer(new QuestionAnswer { Value = 1, Name = "Négligeable" });
            Session.Save(q);

            Session.Save(new Question
            {
                Name = "Technologies de l'application",
                Description = string.Empty,
                Weight = 60,
                Type = QuestionType.TechnologyValue,
                Calculation = new Technologies()
            });

            Session.Save(new Question
            {
                Name = "Technologies des serveurs",
                Description = string.Empty,
                Weight = 20,
                Type = QuestionType.TechnologyValue,
                Calculation = new ServersTechnologies()
            });

            Session.Save(new Question
            {
                Name = "Technologies des instances",
                Description = string.Empty,
                Weight = 10,
                Type = QuestionType.TechnologyValue,
                Calculation = new DbInstancesTechnologies()
            });

            Session.Save(new Question
            {
                Name = "Technologies des intégrations",
                Description = string.Empty,
                Weight = 10,
                Type = QuestionType.TechnologyValue,
                Calculation = new IntegrationsTechnologies()
            });
        }
    }
}
