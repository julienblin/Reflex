// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsReviewsChart.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries.Review;

namespace CGI.Reflex.Web.Charts
{
    public class ApplicationsReviewsChart : BaseChart
    {
        public ApplicationsReviewsChart(int width, int height)
            : base(width, height)
        {
        }

        public ApplicationsReviewResult Result { get; set; }

        public override Chart Produce()
        {
            Chart = new Chart
            {
                Width = Width, 
                Height = Height, 
                RenderType = RenderType.ImageTag, 
                Palette = ChartColorPalette.None, 
                PaletteCustomColors = ColorPalette.Palette
            };

            var area = new ChartArea
                {
                    Name = Chart.ChartAreas.NextUniqueName(),
                    AxisX =
                        {
                            Title = "Valeur d'affaires",
                            TitleFont = new Font("Trebuchet MS", 10F, FontStyle.Bold),
                            MajorGrid = { LineColor = Color.Black }
                        },
                };

            area.AxisX.RoundAxisValues();
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = 100;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.Crossing = 50;
            area.AxisX.LabelStyle = new LabelStyle { Enabled = false };
            area.AxisX.MajorTickMark = new TickMark { Enabled = false };

            area.AxisY.Title = "Valeur technologique";
            area.AxisY.TitleFont = new Font("Trebuchet MS", 10F, FontStyle.Bold);
            area.AxisY.MajorGrid.LineColor = Color.Black;
            area.AxisY.RoundAxisValues();
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 100;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisY.Crossing = 50;
            area.AxisY.LabelStyle = new LabelStyle { Enabled  = false };
            area.AxisY.MajorTickMark = new TickMark { Enabled = false };

            Chart.ChartAreas.Add(area);

            var series = new Series
            {
                Name = "Points", 
                ChartType = SeriesChartType.Point, 
                MarkerStyle = MarkerStyle.Circle, 
                MarkerSize = 10, 
                MarkerColor = Color.Red
            };

            foreach (var point in Result.Resulsts.Where(r => r.BusinessValue.HasValue && r.TechnologyValue.HasValue))
            {
                if (point.BusinessValue.HasValue && point.TechnologyValue.HasValue)
                    series.Points.AddXY(point.BusinessValue.Value, point.TechnologyValue.Value);
            }

            if (series.Points.Count == 0)
            {
                series.MarkerSize = 0;
                series.Points.AddXY(0, 0);
            }

            Chart.Series.Add(series);

            return Chart;
        }
    }
}