// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListCommands.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ManyConsole;

namespace CGI.Reflex.Commands.Commands
{
    public class ListCommands : ConsoleModeCommand
    {
        private readonly IEnumerable<ConsoleCommand> _commands;

        public ListCommands(IEnumerable<ConsoleCommand> commands)
        {
            _commands = commands;
        }

        public override IEnumerable<ConsoleCommand> GetNextCommands()
        {
            return _commands;
        }
    }
}
