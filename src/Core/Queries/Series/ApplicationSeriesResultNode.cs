// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationSeriesResultNode.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Queries.Series
{
    public class ApplicationSeriesResultNode
    {
        private readonly List<int> _counts = new List<int>();

        public ApplicationSeriesResultNode(IHierarchicalEntity nodeCriteria, int total, IEnumerable<int> counts)
        {
            NodeCriteria = nodeCriteria;
            Total = total;
            Children = new List<ApplicationSeriesResultNode>();
            _counts.AddRange(counts);
        }

        public IHierarchicalEntity NodeCriteria { get; set; }

        public int Total { get; set; }

        public ApplicationSeriesResultNode Parent { get; set; }

        public IList<ApplicationSeriesResultNode> Children { get; set; }

        public T GetCriteria<T>()
            where T : IHierarchicalEntity
        {
            return (T)NodeCriteria;
        }

        public int GetCount(int columnIndex)
        {
            return _counts[columnIndex];
        }

        public void AddToCounts(IList<int> counts)
        {
            if (_counts.Count == 0)
            {
                _counts.AddRange(counts);
            }
            else
            {
                for (var i = 0; i < counts.Count(); i++)
                    _counts[i] += counts[i];
            }
        }

        public void AddChild(ApplicationSeriesResultNode child)
        {
            if (Children.Contains(child)) return;

            child.Parent = this;
            Children.Add(child);
        }
    }
}