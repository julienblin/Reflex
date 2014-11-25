// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CGI.Reflex.Commands.Commands;

using ManyConsole;

namespace CGI.Reflex.Commands
{
    internal static class Utils
    {
        public static void SetUp(string[] args)
        {
            if (args.Length == 0)
                args = new[] { "run-console" };

            var commands = GetCommands();
            commands = commands.Concat(new[] { new ListCommands(commands) });
            ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        public static bool SafeSetClipboard(string value, TextDataFormat format = TextDataFormat.Text)
        {
            try
            {
                Clipboard.SetText(value, format);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }
    }
}
