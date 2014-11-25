// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReviewEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGI.Reflex.Web.Areas.Applications.Models.Review
{
    public class ReviewEdit
    {
        public ReviewEdit()
        {
            BusinessQuestions = new List<ReviewItem>();
            TechnoQuestions = new List<ReviewItem>();
        }

        public int AppId { get; set; }

        public string AppName { get; set; }

        public List<ReviewItem> BusinessQuestions { get; set; }

        public List<ReviewItem> TechnoQuestions { get; set; }

        public double? CurBusinessValue { get; set; }

        public double? CurTechnoValue { get; set; }
    }
}