// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionsIndex.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.System.Models.Questions
{
    public class QuestionsIndex
    {
        [Display(ResourceType = typeof(CoreResources), Name = "QuestionType")]
        public QuestionType Type { get; set; }

        public IEnumerable<object> QuestionTypeList { get; set; }

        public IList<QuestionWeightEdit> Items { get; set; }
    }
}