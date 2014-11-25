// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Question.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Calculation;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "Question")]
    public class Question : BaseConcurrentEntity
    {
        private ICollection<QuestionAnswer> _answers;

        public Question()
        {
            _answers = new List<QuestionAnswer>();
        }

        [NotAuditable]
        [Display(ResourceType = typeof(CoreResources), Name = "QuestionType")]
        public virtual QuestionType Type { get; set; }

        [Required]
        [StringLength(255)]
        [Display(ResourceType = typeof(CoreResources), Name = "Question")]
        public virtual string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        public virtual string Description { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Weight")]
        public virtual int Weight { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Answers")]
        public virtual IEnumerable<QuestionAnswer> Answers { get { return _answers; } }

        [Display(ResourceType = typeof(CoreResources), Name = "Calculation")]
        public virtual ICalculation Calculation { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "MaxAnswerValue")]
        public virtual int MaxValue
        {
            get
            {
                if (Calculation != null)
                    return Calculation.MaxValue;
                return Answers.Max(a => a.Value) - Answers.Min(a => a.Value);
            }
        }

        public virtual void SetQuestionAnswers(IEnumerable<QuestionAnswer> qa)
        {
            var answersToRemove = Answers.Where(a => !qa.Any(q => q.Id == a.Id)).ToList();
            foreach (var answerToRemove in answersToRemove)
                RemoveAnswer(answerToRemove.Id);

            foreach (var answerToAdd in qa.Where(s => s.Id == 0))
                AddAnswer(answerToAdd);
        }

        public virtual void RemoveAnswer(int qaId)
        {
            QuestionAnswer qa = _answers.FirstOrDefault(a => a.Id == qaId);

            _answers.Remove(qa);
        }

        public virtual void AddAnswer(QuestionAnswer qa)
        {
            qa.Question = this;

            _answers.Add(qa);
        }

        public virtual int? GetAnswerIdByApplication(Application application)
        {
            if (Calculation != null)
                return null;

            var appAnswer = application.ReviewAnswers.Where(a => a.Answer.Question.Id == Id).Select(a => a.Answer).SingleOrDefault();

            if (appAnswer == null)
                return null;
            return appAnswer.Id;
        }

        public virtual string GetAnswerDisplayByApplication(Application application)
        {
            if (Calculation != null)
            {
                var result = Calculation.Calculate(application);
                return result.HasValue ? string.Format("{0}{1}", result.Value, MaxValue == 100 ? "%" : string.Empty) : null;
            }

            var appAnswer = application.ReviewAnswers.Where(a => a.Answer.Question.Id == Id).Select(a => a.Answer).SingleOrDefault();
            return appAnswer == null ? null : appAnswer.Name;
        }

        public virtual string GetAnswerExplanationByApplication(Application application)
        {
            if (Calculation != null)
                return null;

            var appAnswer = application.ReviewAnswers.Where(a => a.Answer.Question.Id == Id).Select(a => a.Answer).SingleOrDefault();
            return appAnswer == null ? null : appAnswer.Explanation;
        }

        public virtual double? GetAnswerValueByApplication(Application application)
        {
            if (Calculation != null)
                return Calculation.Calculate(application);

            var appAnswer = application.ReviewAnswers.Where(a => a.Answer.Question.Id == Id).Select(a => a.Answer).SingleOrDefault();
            if (appAnswer == null)
                return null;
            return appAnswer.Value - Answers.Min(a => a.Value);
        }

        public virtual double? GetWeightedAnswerValueByApplication(Application application)
        {
            var value = GetAnswerValueByApplication(application);

            if (!value.HasValue || MaxValue == 0)
                return null;
            return (value.Value / MaxValue) * Weight;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
