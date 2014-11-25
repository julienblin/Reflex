// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionType.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Entities
{
    public enum QuestionType
    {
        [Display(ResourceType = typeof(CoreResources), Name = "BusinessValue")]
        BusinessValue,

        [Display(ResourceType = typeof(CoreResources), Name = "TechnologyValue")]
        TechnologyValue
    }
}
