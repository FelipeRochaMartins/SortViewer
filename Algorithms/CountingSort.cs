using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    class CountingSort : BaseSortAlgorithm
    {
        public override string Name => "Counting Sort";
        public override string Description => "A non-comparison sorting algorithm that works by counting occurrences of each element and " +
            "placing them in the correct order. Suitable for integers within a fixed range. " +
            "Time complexity: O(n + k), where k is the range of input values";

        public override List<SortingStep> Sort(int[] data)
        {
            throw new NotImplementedException();
        }
    }
}
