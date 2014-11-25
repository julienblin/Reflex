// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationSeriesChart.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries.Series;
using CGI.Reflex.Web.Helpers;

namespace CGI.Reflex.Web.Charts
{
    public class ApplicationSeriesChart : BaseChart
    {
        public ApplicationSeriesChart(int width, int height)
            : base(width, height)
        {
        }

        public ApplicationSeriesResult Result { get; set; }

        public override Chart Produce()
        {
            Chart = new Chart
            {
                Width = Width, 
                Height = Height, 
                RenderType = RenderType.ImageTag, 
                Palette = ChartColorPalette.None, 
                PaletteCustomColors = ColorPalette.Palette, 
                AntiAliasing = AntiAliasingStyles.All
            };

            Chart.ChartAreas.Add("defaultChartArea");

            if (Result.Lines.Count > 0)
            {
                Chart.Legends.Add("defaultLegend");
                Chart.Legends["defaultLegend"].BackGradientStyle = GradientStyle.TopBottom;
                Chart.Legends["defaultLegend"].BackColor = Color.White;
                Chart.Legends["defaultLegend"].BackSecondaryColor = ColorTranslator.FromHtml("#E7E7E7");

                Chart.ChartAreas["defaultChartArea"].Area3DStyle.Enable3D = true;

                Chart.Series.Add("LineSeries");
                Chart.Series["LineSeries"].ChartType = SeriesChartType.Pie;
                Chart.Series["LineSeries"]["PieLabelStyle"] = "Inside";
                Chart.Series["LineSeries"].Label = "#PERCENT{P0}";
                Chart.Series["LineSeries"].LegendText = "#VALX - #VALY";
                Chart.Series["LineSeries"].BorderWidth = 1;
                Chart.Series["LineSeries"].BorderColor = Color.Black;
                Chart.Series["LineSeries"].ChartArea = "defaultChartArea";

                SetPoints(Chart.Series["LineSeries"], Result.Lines.Where(l => l.Total > 0), "LineCriteria", "Total");
            }
            else
            {
                var title = Chart.Titles.Add("Aucun résultat.");
                title.Font = new Font("Times New Roman", 14, FontStyle.Regular);
                title.Docking = Docking.Top;
                title.DockingOffset = 10;
                title.Alignment = ContentAlignment.MiddleCenter;
            }

            return Chart;
        }
    }
}