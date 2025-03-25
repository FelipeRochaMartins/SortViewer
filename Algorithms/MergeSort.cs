using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    class MergeSort : BaseSortAlgorithm
    {
        public override string Name => "Merge Sort";
        public override string Description => "A divide-and-conquer sorting algorithm that splits the list into halves, sorts them recursively, " +
            "and then merges them back together in order. " +
            "Time complexity: O(n log n)";

        public override List<SortingStep> Sort(int[] data)
        {
            throw new NotImplementedException();
        }
    }
}
