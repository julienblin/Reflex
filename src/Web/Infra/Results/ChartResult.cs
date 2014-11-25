// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartResult.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

namespace CGI.Reflex.Web.Infra.Results
{
    public class ChartResult : FileResult
    {
        private readonly Chart _chart;

        public ChartResult(Chart chart)
            : base(@"image/png")
        {
            _chart = chart;
        }

        public Chart Chart { get { return _chart; } }

        [ExcludeFromCodeCoverage]
        protected override void WriteFile(HttpResponseBase response)
        {
            using (var imageStream = new MemoryStream())
            {
                _chart.SaveImage(imageStream, ChartImageFormat.Png);
                imageStream.WriteTo(response.OutputStream);
            }
        }
    }
}