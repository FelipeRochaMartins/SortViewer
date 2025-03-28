using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class SelectionSort : BaseSortAlgorithm
    {
        public override string Name => "Selection Sort";
        public override string Description => "A simple sorting algorithm that repeatedly finds the minimum element from the unsorted part\n" +
            "and puts it at the beginning of the unsorted section.\n" +
            "The algorithm maintains two subarrays: a sorted subarray and an unsorted subarray.\n\n" +
            "Time complexity: O(n²) in all cases";

        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();

            int n = array.Length;

            steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state of the array"));

            for (int i = 0; i < n; i++)
            {
                var lower = i;

                for (int j = i + 1; j < n; j++)
                {
                    int comparisonResult = array[j].CompareTo(array[lower]);
                    steps.Add(CreateStep(array, OperationType.Comparison, [lower, j], Array.Empty<int>(), $"Comparing elements at positions {lower} [{array[lower]}] and {j} [{array[j]}]"));
                    RaiseComparisonEvent(lower, j, comparisonResult, array);
                    
                    if (array[j] < array[lower])
                    {
                        lower = j;
                    }
                }

                if (i != lower)
                {
                    steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), [lower, i], $"Swapping elements: {array[lower]} ↔ {array[i]}"));
                    RaiseSwapEvent(lower, i, array);
                    Swap(array, lower, i);
                }
            }

            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Array is now sorted"));

            return steps;
        }
    }
}
