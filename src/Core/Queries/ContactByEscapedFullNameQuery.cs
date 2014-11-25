// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactByEscapedFullNameQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Core.Queries
{
    public class ContactByEscapedFullNameQuery : SingleResultQuery<Contact>
    {
        public ContactByEscapedFullNameQuery()
        {
            EscapeToken = @"_";
        }

        public string EscapedFullName { get; set; }

        public string EscapeToken { get; set; }

        public override Contact Execute(ISession session)
        {
            var query = session.QueryOver<Contact>();

            var tokens = EscapedFullName.Split(new[] { " " }, StringSplitOptions.None);

            if (tokens.Length > 0)
                query.Where(c => c.FirstName == tokens[0].Replace(EscapeToken, " "));

            if (tokens.Length > 1)
                query.Where(c => c.LastName == tokens[1].Replace(EscapeToken, " "));

            if (tokens.Length > 2)
                query.Where(c => c.Company == tokens[2].Replace(EscapeToken, " "));

            return query.SingleOrDefault();
        }
    }
}
