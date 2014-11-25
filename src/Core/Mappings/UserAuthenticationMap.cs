// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserAuthenticationMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class UserAuthenticationMap : BaseEntityMap<UserAuthentication>
    {
        public UserAuthenticationMap()
        {
            Map(x => x.PasswordDigest).Access.CamelCaseField(Prefix.Underscore);
            Map(x => x.LastPasswordChangedAt).Access.CamelCaseField(Prefix.Underscore);
            Map(x => x.SingleAccessToken).Access.CamelCaseField(Prefix.Underscore);
            Map(x => x.SingleAccessTokenValidUntil).Access.CamelCaseField(Prefix.Underscore);

            Map(x => x.LastLoginDate);
            Map(x => x.FailedPasswordAttemptCount);

            References(x => x.User).Cascade.None();
        }
    }
}
