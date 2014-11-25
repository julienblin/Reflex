// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceNotFoundException.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CGI.Reflex.Core.Importers
{
    [Serializable]
    internal class ReferenceNotFoundException : Exception
    {
        public ReferenceNotFoundException(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; private set; }
    }
}
