// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Landscape.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "Landscape")]
    public class Landscape : BaseConcurrentEntity
    {
        private ICollection<Server> _servers;

        public Landscape()
        {
            _servers = new List<Server>();
        }
        
        [Unique]
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(30)]
        public virtual string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Servers")]
        public virtual IEnumerable<Server> Servers { get { return _servers; } }

        public virtual void AddServersToLandscape(IEnumerable<Server> servers)
        {
            foreach (var server in servers)
                AddServerToLandscape(server);
        }

        public virtual void SetServers(IEnumerable<Server> servers)
        {
            foreach (var server in _servers)
            {
                if (!servers.Any(s => s == server))
                    RemoveServerFromLandscape(server);
            }

            foreach (var server in servers)
            {
                if (server.Landscape != this)
                {
                    AddServerToLandscape(server);
                }
            }
        }

        public virtual void AddServerToLandscape(Server server)
        {
            server.Landscape = this;
        }

        public virtual void RemoveServerFromLandscape(Server server)
        {
            if (server.Landscape == this)
                server.Landscape = null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
