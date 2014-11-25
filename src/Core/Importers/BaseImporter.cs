using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Importers
{
    public abstract class BaseImporter : IImporter
    {
        private readonly string _resName;

        protected BaseImporter(string resName)
        {
            _resName = resName;
        }

        public virtual string DisplayName
        {
            get
            {
                var resManager = new ResourceManager(typeof(Resources));
                return resManager.GetString(_resName);
            }
        }

        public abstract void GetTemplate(Stream stream);
    }
}
