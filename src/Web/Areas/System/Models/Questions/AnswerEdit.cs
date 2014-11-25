// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnswerEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Areas.System.Models.Questions
{
    public class AnswerEdit
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public int Value { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Answer { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Explanation { get; set; }
    }
}