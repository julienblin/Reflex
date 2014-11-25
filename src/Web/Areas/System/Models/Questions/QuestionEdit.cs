// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.System.Models.Questions
{
    public class QuestionEdit
    {
        public QuestionEdit()
        {
            Answers = new List<AnswerEdit>();
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ConcurrencyVersion { get; set; }

        [HiddenInput(DisplayValue = false)]
        public QuestionType Type { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Question")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        public string Description { get; set; }

        public List<AnswerEdit> Answers { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "AutoCalculation")]
        public string Calculation { get; set; }

        public string FormAction
        {
            get { return Id == 0 ? "Create" : "Edit"; }
        }
    }
}