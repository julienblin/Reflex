// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationSeriesResult.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Queries.Series
{
    public enum LineMultiplicities
    {
        OneToOne,
        ManyToOne
    }

    public class ApplicationSeriesResult
    {
        public ApplicationSeriesResult()
        {
            Columns = new List<object>();
            Lines = new List<ApplicationSeriesResultLine>();
        }

        public string LineCriteria { get; set; }

        public string ColumnCriteria { get; set; }

        public IList<object> Columns { get; set; }

        public IList<ApplicationSeriesResultLine> Lines { get; set; }

        public LineMultiplicities LineMultiplicities { get; set; }

        public bool AreLinesHierarchical
        {
            get
            {
                if (Lines.Count == 0)
                    return false;

                return Lines.First().LineCriteria is IHierarchicalEntity;
            }
        }

        public int GetTotalCount(int columnIndex)
        {
            return Lines.Sum(line => line.GetCount(columnIndex));
        }

        public int GetGrandTotal()
        {
            if (Columns.Count == 0)
                return GetTotalCount(0);

            return Columns.Select((t, i) => GetTotalCount(i)).Sum();
        }

        public int GetPercent(int value)
        {
            if (LineMultiplicities == LineMultiplicities.ManyToOne) throw new NotSupportedException();
            return Convert.ToInt32(Math.Round(Convert.ToDouble(value * 100) / Convert.ToDouble(GetGrandTotal()), MidpointRounding.ToEven));
        }

        public IEnumerable<ApplicationSeriesResultNode> GetRootNodes()
        {
            if (!AreLinesHierarchical)
                throw new NotSupportedException();

            var allNodes = new List<ApplicationSeriesResultNode>();

            foreach (var line in Lines)
            {
                var nodeCriteria = (IHierarchicalEntity)line.LineCriteria;
                var currentNode = new ApplicationSeriesResultNode(nodeCriteria, line.Total, line.Counts);
                allNodes.Add(currentNode);
                AddParents(allNodes, currentNode, line.Total, line.Counts);
            }

            return allNodes.Where(n => n.Parent == null);
        }

        private static void AddParents(List<ApplicationSeriesResultNode> allNodes, ApplicationSeriesResultNode currentNode, int nodeTotal, IList<int> counts)
        {
            if (currentNode.NodeCriteria.Parent == null) return;

            var nodeToAdd = allNodes.FirstOrDefault(node => node.NodeCriteria == currentNode.NodeCriteria.Parent);
            if (nodeToAdd == null)
            {
                nodeToAdd = new ApplicationSeriesResultNode(currentNode.NodeCriteria.Parent, 0, Enumerable.Empty<int>());
                allNodes.Add(nodeToAdd);
            }

            nodeToAdd.AddChild(currentNode);
            nodeToAdd.Total += nodeTotal;
            nodeToAdd.AddToCounts(counts);

            AddParents(allNodes, nodeToAdd, nodeTotal, counts);
        }
    }
}
