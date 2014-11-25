using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Organizations.Models
{
    public class SectorHierarchy
    {
        public IEnumerable<Sector> RootSectors { get; set; }
    }
}