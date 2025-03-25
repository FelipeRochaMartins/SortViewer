using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class SelectionSort : BaseSortAlgorithm
    {
        public override string Name => "Selection Sort";
        public override string Description => "An in-place comparison sorting algorithm that divides the list into a sorted and an unsorted region, " +
            "and repeatedly selects the smallest element from the unsorted region and moves it to the sorted region. " +
            "Time complexity: O(n²)";

        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();

            int n = array.Length;

            steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial State"));

            for (int i = 0; i < n; i++)
            {
                var lower = i;

                for (int j = i + 1; j < n; j++)
                {
                    int comparisonResult = array[j].CompareTo(array[lower]);
                    steps.Add(CreateStep(array, OperationType.Comparison, [lower, j], Array.Empty<int>(), $"Comparing {array[lower]} and {array[j]}"));
                    RaiseComparisonEvent(lower, j, comparisonResult, array);
                    
                    if (array[j] < array[lower])
                    {
                        lower = j;
                    }
                }

                if (i != lower)
                {
                    steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), [lower, i], $"Swapping {array[lower]} and {array[i]}"));
                    RaiseSwapEvent(lower, i, array);
                    Swap(array, lower, i);
                }
            }

            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));

            return steps;
        }
    }
}
