using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    class BucketSort : BaseSortAlgorithm
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
