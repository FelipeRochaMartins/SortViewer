using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class RadixSort : BaseSortAlgorithm
    {
        public override string Name => "Radix Sort";
        public override string Description => "A non-comparison sorting algorithm that sorts numbers digit by digit, " +
            "using a stable subroutine like Counting Sort. " +
            "Time complexity: O(nk), where k is the number of digits";

        public override List<SortingStep> Sort(int[] data)
        {
            throw new NotImplementedException();
        }

    }
}
