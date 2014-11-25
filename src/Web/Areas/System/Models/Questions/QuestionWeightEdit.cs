// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionWeightEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Mvc;

namespace CGI.Reflex.Web.Areas.System.Models.Questions
{
    public class QuestionWeightEdit
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Weight { get; set; }

        public bool ToDelete { get; set; }
    }
}