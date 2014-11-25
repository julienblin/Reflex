// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapesServersDisplay.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Models
{
    public class LandscapesServersDisplay
    {
        private ICollection<LandscapeResult> _landscapes;

        private ICollection<Server> _selectedServer;

        private ICollection<DomainValue> _environments;

        private LandscapeResult _emptyLandscape;

        public LandscapesServersDisplay()
        {
            _landscapes = new List<LandscapeResult>();
            _selectedServer = new List<Server>();
            _environments = new List<DomainValue>();

            ShowLandscape = true;
            ShowLinkToLandscape = true;
            AllowRemoveLandscape = true;
            AllowRemoveServer = true;
            AllowAddServer = true;
        }

        public bool ShowLandscape { get; set; }

        public bool ShowLinkToLandscape { get; set; }

        public bool AllowRemoveLandscape { get; set; }

        public bool AllowRemoveServer { get; set; }

        public bool AllowAddServer { get; set; }

        public IEnumerable<LandscapeResult> Landscapes { get { return _landscapes; } }

        public IEnumerable<LandscapeResult> OrderedLandscapes { get { return _landscapes.OrderBy(l => (l.Landscape != null ? l.Landscape.Name : "ZZZZZZ")); } }

        public IEnumerable<Server> SelectedServer { get { return _selectedServer; } }

        public IEnumerable<DomainValue> Environments { get { return _environments; } }

        public IEnumerable<DomainValue> OrderedEnvironments { get { return _environments.OrderBy(e => e.DisplayOrder); } }

        public void AddServers(IEnumerable<Server> servers)
        {
            foreach (var server in servers)
            {
                AddServer(server);
            }
        }

        public void AddServer(Server server)
        {
            AddServer(server, server.Landscape);
        }

        public void AddServers(IEnumerable<Server> servers, Landscape landscape)
        {
            foreach (var server in servers)
            {
                AddServer(server, landscape);
            }
        }

        public void AddServer(Server server, Landscape landscape)
        {
            if (!SelectedServer.Contains(server))
            {
                AddServerToLandscape(server, landscape);
                _selectedServer.Add(server);
                AddEnvironment(server.Environment);
            }
        }

        private void AddServerToLandscape(Server server, Landscape landscape)
        {
            var result = Landscapes.SingleOrDefault(r => r.Landscape == landscape) ?? AddLandscape(landscape);
            result.AddServer(server);
        }

        private LandscapeResult AddLandscape(Landscape landscape)
        {
            if (landscape != null)
            {
                var landscapeResult = new LandscapeResult();
                landscapeResult.SetLandscape(landscape);
                _landscapes.Add(landscapeResult);

                foreach (Server server in landscape.Servers)
                {
                    AddEnvironment(server.Environment);
                }

                return landscapeResult;
            }
            
            if (_emptyLandscape == null)
            {
                _emptyLandscape = new LandscapeResult();
                _landscapes.Add(_emptyLandscape);
            }

            return _emptyLandscape;
        }

        private void AddEnvironment(DomainValue env)
        {
            if (!Environments.Contains(env))
            {
                _environments.Add(env);
            }
        }

        public class LandscapeResult
        {
            private Landscape _landscape;

            private ICollection<Server> _servers;

            public LandscapeResult()
            {
                _servers = new List<Server>();
            }

            public Landscape Landscape { get { return _landscape; } }

            public IEnumerable<Server> Servers { get { return _servers; } }

            public void SetLandscape(Landscape landscape)
            {
                _landscape = landscape;
                _servers = landscape.Servers.ToList();
            }

            public void AddServer(Server server)
            {
                if (!_servers.Contains(server))
                    _servers.Add(server);
            }

            public int GetMaxEnvServerCount()
            {
                return Servers.Select(server => GetEnvServerList(server.Environment).Count()).Concat(new[] { 0 }).Max();
            }

            public int GetNumberOfRenderedRows(IEnumerable<DomainValue> environments)
            {
                return environments.Select(e => GetEnvServerList(e).Count()).FindLeastCommonMultiple();
            }

            public IEnumerable<Server> GetEnvServerList(DomainValue env)
            {
                return Servers.Where(s => s.Environment == env).OrderBy(s => s.Name).ToList();
            }
        }
    }
}