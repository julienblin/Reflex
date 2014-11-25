// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllFactories.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Tests.Factories
{
    public class AllFactories
    {
        public AllFactories()
        {
            Application = new ApplicationFactory(this);
            AuditInfo = new AuditInfoFactory(this);
            Contact = new ContactFactory(this);
            DomainValue = new DomainValueFactory(this);
            Event = new EventFactory(this);
            Integration = new IntegrationFactory(this);
            Role = new RoleFactory(this);
            Sector = new SectorFactory(this);
            Technology = new TechnologyFactory(this);
            User = new UserFactory(this);
            Server = new ServerFactory(this);
            Landscape = new LandscapeFactory(this);
            LogEntry = new LogEntryFactory(this);
            AppContactLink = new AppContactLinkFactory(this);
            Question = new QuestionFactory(this);
            QuestionAnswer = new QuestionAnswerFactory(this);
            DbInstance = new DbInstanceFactory(this);
        }

        public ApplicationFactory Application { get; private set; }

        public AuditInfoFactory AuditInfo { get; private set; }

        public ContactFactory Contact { get; private set; }

        public DomainValueFactory DomainValue { get; private set; }

        public EventFactory Event { get; private set; }

        public IntegrationFactory Integration { get; private set; }

        public RoleFactory Role { get; private set; }

        public SectorFactory Sector { get; private set; }

        public TechnologyFactory Technology { get; private set; }

        public UserFactory User { get; private set; }

        public ServerFactory Server { get; private set; }

        public LandscapeFactory Landscape { get; private set; }

        public LogEntryFactory LogEntry { get; private set; }

        public AppContactLinkFactory AppContactLink { get; private set; }

        public QuestionFactory Question { get; private set; }

        public QuestionAnswerFactory QuestionAnswer { get; private set; }

        public DbInstanceFactory DbInstance { get; private set; }
    }
}
