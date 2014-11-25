// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileImporterResult.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Importers
{
    public class FileImporterResult : IDisposable
    {
        ~FileImporterResult()
        {
            Dispose(false);
        }

        public Stream Stream { get; internal set; }

        public string SuggestedFileName { get; internal set; }

        public string ContentType { get; internal set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Stream != null)
                    Stream.Dispose();
            }
        }
    }
}
