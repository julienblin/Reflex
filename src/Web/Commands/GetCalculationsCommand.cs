// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCalculationsCommand.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Calculation;
using CGI.Reflex.Core.Commands;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Commands
{
    public class GetCalculationsCommand : AbstractCommand<IDictionary<string, string>>
    {
        public QuestionType? Type { get; set; }

        protected override IDictionary<string, string> ExecuteImpl()
        {
            var type = typeof(ICalculation);

            var types = type.Assembly.GetTypes().Where(type.IsAssignableFrom);

            var ret = new Dictionary<string, string>();

            foreach (var t in types)
            {
                if (!t.IsInterface && !t.IsAbstract)
                {
                    var calculation = (ICalculation)Activator.CreateInstance(t);

                    if (!Type.HasValue || Type.Value == calculation.Type)
                        ret.Add(calculation.Name, t.FullName);
                }
            }

            return ret;
        }
    }
}