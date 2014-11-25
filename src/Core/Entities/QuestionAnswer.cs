// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionAnswer.cs" company="CGI">
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
    [ForwardAudit("Question", "Answers")]
    public class QuestionAnswer : BaseEntity
    {
        private ICollection<ApplicationReview> _reviews;

        public QuestionAnswer()
        {
            _reviews = new List<ApplicationReview>();
        }

        [Display(ResourceType = typeof(CoreResources), Name = "Question")]
        public virtual Question Question { get; set; }

        [Required]
        [StringLength(255)]
        [Display(ResourceType = typeof(CoreResources), Name = "Answer")]
        public virtual string Name { get; set; }

        [StringLength(255)]
        [Display(ResourceType = typeof(CoreResources), Name = "Explanation")]
        public virtual string Explanation { get; set; }

        [Required]
        [Display(ResourceType = typeof(CoreResources), Name = "Value")]
        public virtual int Value { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Answers")]
        public virtual IEnumerable<ApplicationReview> Reviews { get { return _reviews; } }

        public override string ToString()
        {
            return string.Format("{0} ({1}) {2}", Name, Value, Explanation);
        }
    }
}
