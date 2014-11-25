// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendUserResetTokenCommand.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CGI.Reflex.Core.Commands;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Configuration;
using Postal;

namespace CGI.Reflex.Web.Commands
{
    public class SendUserResetTokenCommand : AbstractCommand<string>
    {
        public UserAuthentication UserAuthentication { get; set; }

        public UrlHelper UrlHelper { get; set; }

        protected override string ExecuteImpl()
        {
            var config = ReflexConfigurationSection.GetConfigurationSection();
            if (config.AuthenticationMode != AuthenticationMode.Forms)
                throw new NotSupportedException(config.AuthenticationMode.ToString());

            var sat = UserAuthentication.GenerateSingleAccessToken(TimeSpan.FromDays(2));
            dynamic email = new Email("SendUserResetToken");
            email.User = UserAuthentication.User;
            email.BoardingLink = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + UrlHelper.RouteUrl("Boarding", new { key = sat });
            email.Send();
            return sat;
        }
    }
}