using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Importers
{
    public interface IImporter
    {
        string DisplayName { get; }

        void GetTemplate(Stream stream);
    }
}
