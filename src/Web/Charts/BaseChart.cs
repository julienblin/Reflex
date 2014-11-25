// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseChart.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Web.Charts
{
    public abstract class BaseChart
    {
        protected readonly int Width;

        protected readonly int Height;

        protected BaseChart(int width, int height)
        {
            if (width < 1) throw new ArgumentOutOfRangeException("width", width.ToString(CultureInfo.InvariantCulture));
            if (width > 1920) throw new ArgumentOutOfRangeException("width", width.ToString(CultureInfo.InvariantCulture));

            if (height < 1) throw new ArgumentOutOfRangeException("height", height.ToString(CultureInfo.InvariantCulture));
            if (height > 1080) throw new ArgumentOutOfRangeException("height", height.ToString(CultureInfo.InvariantCulture));

            Width = width;
            Height = height;
        }

        protected Chart Chart { get; set; }

        protected ISession NHSession { get { return References.NHSession; } }

        public abstract Chart Produce();

        protected static void SetPoints(Series series, IEnumerable dataSet, string xProperty, string yProperty)
        {
            MethodInfo xmethod = null;
            MethodInfo ymethod = null;
            var index = 0;
            foreach (var value in dataSet)
            {
                if (xmethod == null)
                {
                    xmethod = value.GetType().GetProperty(xProperty).GetGetMethod();
                    ymethod = value.GetType().GetProperty(yProperty).GetGetMethod();
                }

                var xvalue = xmethod.Invoke(value, null);
                var yvalue = ymethod.Invoke(value, null);

                var xDomainValue = xvalue as DomainValue;
                if (xDomainValue != null)
                {
                    series.Points.AddXY(xDomainValue.Name, Convert.ToDouble(yvalue));
                    if (!xDomainValue.Color.IsEmpty)
                        series.Points[index].Color = xDomainValue.Color;
                }
                else
                {
                    series.Points.AddXY(xvalue != null ? xvalue.ToString() : "N/A", Convert.ToDouble(yvalue));
                }

                ++index;
            }
        }

        protected void SetTitle(string title, Docking docking = Docking.Top)
        {
            Chart.Titles.Add(new Title(title, docking, new Font("Times New Roman", 14, FontStyle.Bold), Color.Black));
        }
    }
}