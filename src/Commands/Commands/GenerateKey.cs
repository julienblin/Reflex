// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateKey.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Utilities;
using ManyConsole;
using NDesk.Options;

namespace CGI.Reflex.Commands.Commands
{
    public class GenerateKey : ConsoleCommand
    {
        private const int DefaultLength = 32;

        public GenerateKey()
        {
            Length = DefaultLength;
            IsCommand("generate-key", "Generate a random key (for use in encryption or password, for example).");

            Options = new OptionSet
            {
                { "l|length=", "Length of the key", (int l) => Length = l }
            };
        }

        public int Length { get; set; }

        public override int Run(string[] remainingArguments)
        {
            var key = Encryption.GenerateRandomToken(Length);
            Console.WriteLine(key);
            if (Utils.SafeSetClipboard(key))
                Console.WriteLine(@"The key has been placed on the clipboard.");
            return 0;
        }
    }
}
