// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Application.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "Application")]
    public class Application : BaseConcurrentEntity
    {
        private AppInfo _appInfo;

        private ICollection<AppContactLink> _contactLinks;

        private ICollection<AppTechnoLink> _technologyLinks;

        private ICollection<AppDbInstanceLink> _dbInstanceLinks;

        private ICollection<AppServerLink> _serverLinks;

        private ICollection<Integration> _integrationsAsSource;

        private ICollection<Integration> _integrationsAsDest;

        private ICollection<Event> _events;

        private ICollection<ApplicationReview> _reviewAnswers;

        private bool _isReviewed;

        private double? _businessValue;

        private double? _technologyValue;

        public Application()
        {
            _appInfo = new AppInfo();
            _contactLinks = new List<AppContactLink>();
            _technologyLinks = new List<AppTechnoLink>();
            _dbInstanceLinks = new List<AppDbInstanceLink>();
            _serverLinks = new List<AppServerLink>();
            _integrationsAsSource = new List<Integration>();
            _integrationsAsDest = new List<Integration>();
            _events = new List<Event>();
            _reviewAnswers = new List<ApplicationReview>();
            _isReviewed = false;
        }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Unique]
        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public virtual string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Code")]
        [StringLength(20)]
        public virtual string Code { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationType")]
        [Required]
        [DomainValue(DomainValueCategory.ApplicationType)]
        public virtual DomainValue ApplicationType { get; set; }

        [Required]
        [NotAuditable]
        [Display(ResourceType = typeof(CoreResources), Name = "AppInfo")]
        public virtual AppInfo AppInfo { get { return _appInfo; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Technologies")]
        public virtual IEnumerable<AppTechnoLink> TechnologyLinks { get { return _technologyLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Contacts")]
        public virtual IEnumerable<AppContactLink> ContactLinks { get { return _contactLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Servers")]
        public virtual IEnumerable<AppServerLink> ServerLinks { get { return _serverLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "DbInstance")]
        public virtual IEnumerable<AppDbInstanceLink> DbInstanceLinks { get { return _dbInstanceLinks; } }

        [Display(ResourceType = typeof(CoreResources), Name = "ReviewAnswers")]
        public virtual IEnumerable<ApplicationReview> ReviewAnswers { get { return _reviewAnswers; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Integrations")]
        public virtual IEnumerable<Integration> IntegrationsAsSource { get { return _integrationsAsSource; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Integrations")]
        public virtual IEnumerable<Integration> IntegrationsAsDest { get { return _integrationsAsDest; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Events")]
        public virtual IEnumerable<Event> Events { get { return _events; } }

        public virtual void AddContactLinks(IEnumerable<Contact> contacts)
        {
            foreach (var contact in contacts)
                AddContactLink(contact);
        }

        public virtual bool AddContactLink(Contact contact)
        {
            if (ContactLinks.Any(c => c.Contact == contact))
                return false;

            var link = new AppContactLink { Application = this, Contact = contact };
            _contactLinks.Add(link);

            return true;
        }

        public virtual bool RemoveContactLink(Contact contact)
        {
            var link = ContactLinks.FirstOrDefault(c => c.Contact == contact);
            if (link == null)
                return false;

            _contactLinks.Remove(link);

            return true;
        }

        public virtual void AddTechnologyLinks(IEnumerable<Technology> technologies)
        {
            foreach (var technology in technologies)
                AddTechnologyLink(technology);
        }

        public virtual bool AddTechnologyLink(Technology technology)
        {
            if (TechnologyLinks.Any(tl => tl.Technology == technology))
                return false;
            
            var link = new AppTechnoLink { Application = this, Technology = technology };
            _technologyLinks.Add(link);

            return true;
        }

        public virtual bool RemoveTechnologyLink(Technology technology)
        {
            var link = TechnologyLinks.FirstOrDefault(tl => tl.Technology == technology);
            if (link == null)
                return false;

            _technologyLinks.Remove(link);
            return true;
        }

        public virtual void AddServerLinks(IEnumerable<Server> servers)
        {
            foreach (Server server in servers)
                AddServerLink(server);
        }

        public virtual bool AddServerLink(Server server)
        {
            if (ServerLinks.Any(sl => sl.Server == server))
                return false;

            var link = new AppServerLink { Application = this, Server = server };
            _serverLinks.Add(link);

            return true;
        }

        public virtual bool RemoveServerLink(Server server)
        {
            AppServerLink link = ServerLinks.FirstOrDefault(sl => sl.Server == server);
            if (link == null)
                return false;

            _serverLinks.Remove(link);
            
            var dbInstancesToRemove = DbInstanceLinks.Where(l => l.DbInstances.Server.Id == server.Id).Select(l => l.DbInstances).ToList();

            foreach (var dbInstanceToRemove in dbInstancesToRemove)
                RemoveDbInstanceLink(dbInstanceToRemove);

            return true;
        }

        public virtual void AddDbInstanceLinks(IEnumerable<DbInstance> dbInstances)
        {
            foreach (var dbInstance in dbInstances)
                AddDbInstanceLink(dbInstance);
        }

        public virtual bool AddDbInstanceLink(DbInstance dbInstance)
        {
            if (DbInstanceLinks.Any(dl => dl.DbInstances == dbInstance))
                return false;

            var link = new AppDbInstanceLink { Application = this, DbInstances = dbInstance };
            _dbInstanceLinks.Add(link);

            return true;
        }

        public virtual bool RemoveDbInstanceLink(DbInstance dbInstance)
        {
            var link = DbInstanceLinks.FirstOrDefault(dl => dl.DbInstances == dbInstance);
            if (link == null)
                return false;

            _dbInstanceLinks.Remove(link);
            return true;
        }

        public virtual void AnswerQuestion(Question question, QuestionAnswer answer)
        {
            var curAnswer = ReviewAnswers.SingleOrDefault(ra => ra.Answer.Question.Id == question.Id);

            if (curAnswer == null && answer == null)
                return;
            
            if (curAnswer == null)
            {
                AddAnswer(answer);
            }
            else if (answer == null)
            {
                RemoveAnswer(curAnswer);
            }
// ReSharper disable RedundantCheckBeforeAssignment
            else if (curAnswer.Answer != answer)
// ReSharper restore RedundantCheckBeforeAssignment
            {
                curAnswer.Answer = answer;
            }
        }

        public virtual void AddAnswer(QuestionAnswer answer)
        {
            _reviewAnswers.Add(new ApplicationReview
            {
                Application = this,
                Answer = answer
            });
        }

        public virtual void RemoveAnswer(ApplicationReview reviewToRemove)
        {
            _reviewAnswers.Remove(reviewToRemove);
        }

        public virtual IEnumerable<Integration> GetIntegrations()
        {
            return IntegrationsAsSource.Union(IntegrationsAsDest).Distinct();
        }

        [Display(ResourceType = typeof(CoreResources), Name = "BusinessValue")]
        public virtual double? GetBusinessValue(IEnumerable<Question> questions, bool useCache = true)
        {
            if (!_isReviewed || !useCache)
                InitReviewResult(questions);
            return _businessValue;
        }

        [Display(ResourceType = typeof(CoreResources), Name = "TechnologyValue")]
        public virtual double? GetTechnologyValue(IEnumerable<Question> questions, bool useCache = true)
        {
            if (!_isReviewed || !useCache)
                InitReviewResult(questions);
            return _technologyValue;
        }

        public virtual void InitReviewResult(IEnumerable<Question> questions)
        {
            double curBusiness = 0;
            double maxBusiness = 0;

            double curTechno = 0;
            double maxTechno = 0;

            foreach (var question in questions)
            {
                var weightedValue = question.GetWeightedAnswerValueByApplication(this);

                if (weightedValue.HasValue)
                {
                    if (question.Type == QuestionType.BusinessValue)
                    {
                        curBusiness += weightedValue.Value;
                        maxBusiness += question.Weight;
                    }
                    else
                    {
                        curTechno += weightedValue.Value;
                        maxTechno += question.Weight;
                    }
                }
            }

            if (maxBusiness > 0)
                _businessValue = curBusiness / maxBusiness * 100;
            else
                _businessValue = null;

            if (maxTechno > 0)
                _technologyValue = curTechno / maxTechno * 100;
            else
                _technologyValue = null;

            _isReviewed = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}