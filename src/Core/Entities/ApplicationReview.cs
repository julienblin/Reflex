// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationReview.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [ForwardAudit("Application", "ReviewAnswers")]
    public class ApplicationReview : BaseEntity
    {
        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Application")]
        public virtual Application Application { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Answer")]
        public virtual QuestionAnswer Answer { get; set; }

        public override string ToString()
        {
            if (Answer != null)
                return string.Format("{0} : {1}", Answer.Question, Answer.Name);
            
            return string.Format("{0}", Answer);
        }
    }
}
