// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserAuthentication.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Utilities;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Entities
{
    /// <summary>
    /// This entity holds user authentication information when Forms Authentication is used in web, for example.
    /// </summary>
    public class UserAuthentication : BaseEntity
    {
        public const int MaxFailedAttemptPassword = 3;

#pragma warning disable 649
        private string _passwordDigest;

        private DateTime? _lastPasswordChangedAt;

        private string _singleAccessToken;

        private DateTime? _singleAccessTokenValidUntil;
#pragma warning restore 649

        [Required]
        public virtual User User { get; set; }

        [StringLength(256)]
        public virtual string PasswordDigest { get { return _passwordDigest;  } }

        public virtual DateTime? LastPasswordChangedAt { get { return _lastPasswordChangedAt; } }

        public virtual DateTime? LastLoginDate { get; set; }

        public virtual int FailedPasswordAttemptCount { get; set; }

        [StringLength(128)]
        public virtual string SingleAccessToken { get { return _singleAccessToken; } }

        public virtual DateTime? SingleAccessTokenValidUntil { get { return _singleAccessTokenValidUntil; } }

        public static UserAuthentication Authenticate(string nameOrEmail, string password)
        {
            var userAuth = References.NHSession.QueryOver<UserAuthentication>()
                                               .JoinQueryOver(a => a.User)
                                                    .Where(
                                                        Restrictions.Or(
                                                            Restrictions.InsensitiveLike(Projections.Property<User>(u => u.UserName), nameOrEmail, MatchMode.Exact),
                                                            Restrictions.InsensitiveLike(Projections.Property<User>(u => u.Email), nameOrEmail, MatchMode.Exact)))
                                                .SingleOrDefault();
            if (userAuth == null)
                return null;

            if (userAuth.User.IsLockedOut)
                return null;

            if (!userAuth.VerifyPassword(password))
            {
                userAuth.FailedPasswordAttemptCount++;
                if (userAuth.FailedPasswordAttemptCount > MaxFailedAttemptPassword)
                    userAuth.User.IsLockedOut = true;
                return null;
            }

            userAuth.LastLoginDate = DateTime.Now;
            userAuth.FailedPasswordAttemptCount = 0;
            userAuth.ClearSingleAccessToken();
            return userAuth;
        }

        public virtual void SetPassword(string clearPassword)
        {
            _passwordDigest = Encryption.Hash(clearPassword);
            FailedPasswordAttemptCount = 0;
            _lastPasswordChangedAt = DateTime.Now;
            _singleAccessToken = string.Empty;
        }

        public virtual void ClearPassword()
        {
            _passwordDigest = string.Empty;
            _lastPasswordChangedAt = DateTime.Now;
            _singleAccessToken = string.Empty;
        }

        public virtual bool VerifyPassword(string clearPassword)
        {
            if (string.IsNullOrEmpty(_passwordDigest))
                return false;

            return _passwordDigest.Equals(Encryption.Hash(clearPassword), StringComparison.InvariantCulture);
        }

        public virtual string GenerateSingleAccessToken(TimeSpan validity)
        {
            return GenerateSingleAccessToken(DateTime.Now.Add(validity));
        }

        public virtual string GenerateSingleAccessToken(DateTime validity)
        {
            _singleAccessToken = Encryption.GenerateRandomToken(128);
            _singleAccessTokenValidUntil = validity;
            return _singleAccessToken;
        }

        public virtual void ClearSingleAccessToken()
        {
            _singleAccessToken = string.Empty;
            _singleAccessTokenValidUntil = null;
        }
    }
}
