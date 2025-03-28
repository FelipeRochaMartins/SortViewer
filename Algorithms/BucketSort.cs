using SortViewer.Models;
using SortViewer.Algorithms.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SortViewer.Algorithms
{
    public class BucketSort : BaseSortAlgorithm
    {
        public override string Name => "Bucket Sort";
        public override string Description => "A sorting algorithm that distributes elements into buckets, sorts each bucket individually, " +
            "and then merges them. Works best with uniformly distributed data. " +
            "Time complexity: O(n) in the best case, O(n²) in the worst case";

        public override List<SortingStep> Sort(int[] data)
        {
            throw new NotImplementedException();
        }
    }
}
