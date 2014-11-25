// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAllOperationsCommand.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CGI.Reflex.Core.Commands;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Commands
{
    public class GetAllOperationsCommand : AbstractCommand<IEnumerable<string>>
    {
        public const string UserRoleAssignmentPrefix = @"/System/Users/RoleAssignment/";

        public GetAllOperationsCommand()
        {
            AddWildCards = true;
            AddUserRoleAssignement = true;
        }

        public bool AddWildCards { get; set; }

        public bool AddUserRoleAssignement { get; set; }

        protected override IEnumerable<string> ExecuteImpl()
        {
            var allTypes = GetType().Assembly.GetTypes().ToList();

            var isAllowedAttributes = new List<IsAllowedAttribute>();

            foreach (var type in allTypes)
            {
                isAllowedAttributes.AddRange(type.GetCustomAttributes(typeof(IsAllowedAttribute), true).Cast<IsAllowedAttribute>());
                isAllowedAttributes.AddRange(type.GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(IsAllowedAttribute), true).Cast<IsAllowedAttribute>()));
            }

            var functions = new List<string>();
            foreach (var isAllowedAttribute in isAllowedAttributes)
            {
                foreach (var op in isAllowedAttribute.Operations)
                {
                    if (!functions.Contains(op))
                        functions.Add(op);
                }
            }

            if (functions.Contains(Role.OperationSegmentSeparator))
                functions.Remove(Role.OperationSegmentSeparator);

            if (AddWildCards)
            {
                var wildCards = new List<string>();
                foreach (var function in functions)
                {
                    var segmentsArray = function.Split(new[] { Role.OperationSegmentSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    var currentPrefix = Role.OperationSegmentSeparator;
                    foreach (var segment in segmentsArray.Take(segmentsArray.Length - 1))
                    {
                        var wildcard = currentPrefix + segment + Role.OperationSegmentSeparator +
                                       Role.OperationWildCard;
                        if (!wildCards.Contains(wildcard))
                            wildCards.Add(wildcard);
                        currentPrefix += segment + Role.OperationSegmentSeparator;
                    }
                }

                if (!wildCards.Contains(Role.OperationSegmentSeparator + Role.OperationWildCard))
                    wildCards.Add(Role.OperationSegmentSeparator + Role.OperationWildCard);

                functions.AddRange(wildCards);
            }

            if (AddUserRoleAssignement)
            {
                functions.AddRange(new RoleQuery().List().Select(x => UserRoleAssignmentPrefix + x.Name));
                if (AddWildCards)
                    functions.Add(UserRoleAssignmentPrefix + Role.OperationWildCard);
            }

            functions.Sort();

            return functions;
        }
    }
}