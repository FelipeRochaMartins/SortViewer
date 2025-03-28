using SortViewer.Models;
using SortViewer.Algorithms.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SortViewer.Algorithms
{
    public class BinarySort : BaseSortAlgorithm
    {
        public override string Name => "Binary Sort";
        public override string Description => "A search algorithm that finds the position of a target value within a sorted array. " +
            "It works by repeatedly dividing the search interval in half. " +
            "Time complexity: O(log n)";

        public override List<SortingStep> Sort(int[] data)
        {
            throw new NotImplementedException();
        }
    }
}
