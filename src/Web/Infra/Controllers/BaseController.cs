// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Commands;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Configuration;
using NHibernate;

namespace CGI.Reflex.Web.Infra.Controllers
{
    public abstract class BaseController : AsyncController
    {
        protected ISession NHSession { get { return References.NHSession; } }

        protected User CurrentUser { get { return References.CurrentUser; } }

        protected ReflexConfigurationSection Config { get { return ReflexConfigurationSection.GetConfigurationSection(); } }

        protected void Flash(FlashLevel level, string text, bool disableHtmlEscaping = false)
        {
            string heading = null;
            switch (level)
            {
                case FlashLevel.Info:
                    heading = "Information";
                    break;
                case FlashLevel.Success:
                    break;
                case FlashLevel.Warning:
                    heading = "Avertissement";
                    break;
                case FlashLevel.Error:
                    heading = "Erreur";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }

            Flash(level, heading, text, disableHtmlEscaping);
        }

        protected void Flash(FlashLevel level, string heading, string text, bool disableHtmlEscaping = false)
        {
            var flashList = (IList<FlashData>)TempData["Flashes"] ?? new List<FlashData>();
            flashList.Add(new FlashData { Level = level, Heading = heading, Text = text, DisableHtmlEscaping = disableHtmlEscaping });
            TempData["Flashes"] = flashList;
        }

        protected bool IsAllowed(string operation)
        {
            if (CurrentUser == null)
                return false;

            return CurrentUser.IsAllowed(operation);
        }

        protected TResult Execute<TCommand, TResult>()
            where TCommand : AbstractCommand<TResult>, new()
        {
            return new TCommand().Execute();
        }

        protected TResult Execute<TCommand, TResult>(Action<TCommand> cmd)
            where TCommand : AbstractCommand<TResult>, new()
        {
            var command = new TCommand();
            cmd(command);
            return command.Execute();
        }

        /// <summary>
        /// Create and execute a task, making sure that References.NHSession & References.CurrentUser still works in async context.
        /// Handles Asyncmanager and register result with a parameter named result.
        /// </summary>
        /// <typeparam name="TResult">Type of result returned by the tasl</typeparam>
        /// <param name="function">Function to execute asynchronously</param>
        /// <returns>The started task.</returns>
        /// <see cref="AsyncScope"/>
        protected Task<TResult> AsyncTaskExecute<TResult>(Func<TResult> function)
        {
            var task = new Task<TResult>(
                taskArgs =>
                {
                    var realTaskArgs = (AsyncTaskArgs)taskArgs;
                    Thread.CurrentThread.CurrentCulture = realTaskArgs.CurrentCulture;
                    Thread.CurrentThread.CurrentUICulture = realTaskArgs.CurrentUICulture;
                    using (new AsyncScope(realTaskArgs.CurrentSession, realTaskArgs.CurrentUser))
                    {
                        return function();
                    }
                },
                new AsyncTaskArgs { CurrentSession = NHSession, CurrentUser = CurrentUser, CurrentCulture = CultureInfo.CurrentCulture, CurrentUICulture = CultureInfo.CurrentUICulture });
            
            AsyncManager.OutstandingOperations.Increment();
            task.ContinueWith(t =>
            {
                AsyncManager.Parameters["result"] = t.Result;
                AsyncManager.OutstandingOperations.Decrement();
            });
            
            task.Start();
            return task;
        }

        protected JavaScriptResult JSRedirect(string dest)
        {
            if (string.IsNullOrEmpty(dest))
            {
                return JavaScript("window.top.location.href = '';");
            }

            return JavaScript(string.Format("window.top.location.href = '{0}';", dest.Replace("'", "\\'")));
        }

        protected JavaScriptResult JSRefresh()
        {
            return JavaScript("window.top.location.href = window.top.location.href;");
        }

        private class AsyncTaskArgs
        {
            public ISession CurrentSession { get; set; }

            public User CurrentUser { get; set; }

            public CultureInfo CurrentCulture { get; set; }

            public CultureInfo CurrentUICulture { get; set; }
        }
    }
}