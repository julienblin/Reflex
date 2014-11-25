// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICalculation.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Calculation
{
    public interface ICalculation
    {
        string Name { get; }

        QuestionType Type { get; }

        int MaxValue { get; }

        int? Calculate(Application application);
    }
}
