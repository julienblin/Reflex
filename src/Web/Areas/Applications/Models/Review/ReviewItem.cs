// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItem.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Applications.Models.Review
{
    public class ReviewItem
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public QuestionType Type { get; set; }

        public int Weight { get; set; }

        public string Question { get; set; }

        public string Description { get; set; }

        public int? SelectedAnswer { get; set; }

        public string CurAnswer { get; set; }

        public string CurAnswerExplanation { get; set; }

        public bool IsCalculation { get; set; }

        public IDictionary<int, string> Answers { get; set; }
    }
}