using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class CocktailSort : BaseSortAlgorithm
    {
        public override string Name => "Cocktail Sort";
        public override string Description => "A variation of Bubble Sort that sorts in both directions on each pass, " +
            "moving large values to the end and small values to the beginning. " +
            "It helps reduce the number of swaps needed. " +
            "Time complexity: O(n²)";

        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();

            int n = array.Length;

            steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state"));

            bool swapped;
            int start = 0;
            int end = n - 1;

            do
            {
                swapped = false;
                steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), 
                    $"Forward pass from index {start} to {end}"));

                for (int i = start; i < end; i++)
                {
                    int comparisonResult = array[i].CompareTo(array[i + 1]);
                    steps.Add(CreateStep(array, OperationType.Comparison, [i, i + 1], Array.Empty<int>(), $"Comparing {array[i]} and {array[i + 1]}"));
                    RaiseComparisonEvent(i, i + 1, comparisonResult, array);

                    if (comparisonResult > 0)
                    {
                        Swap(array, i, i + 1);
                        steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), [i, i + 1], $"Swapping {array[i + 1]} and {array[i]}"));
                        RaiseSwapEvent(i, i + 1, array);
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    break;
                }

                // Decrease the end boundary as the largest element is now at the end
                end--;

                swapped = false;
                steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), 
                    $"Backward pass from index {end} to {start}"));

                for (int i = end; i > start; i--)
                {
                    int comparisonResult = array[i].CompareTo(array[i - 1]);
                    steps.Add(CreateStep(array, OperationType.Comparison, [i, i - 1], Array.Empty<int>(), $"Comparing {array[i]} and {array[i - 1]}"));
                    RaiseComparisonEvent(i, i - 1, comparisonResult, array);

                    if (comparisonResult < 0)
                    {
                        Swap(array, i, i - 1);
                        steps.Add(CreateStep(array, OperationType.Swap,  Array.Empty<int>(), [i, i - 1], $"Swapping {array[i - 1]} and {array[i]}"));
                        RaiseSwapEvent(i, i - 1, array);
                        swapped = true;
                    }
                }

                // Increase the start boundary as the smallest element is now at the beginning
                start++;

            } while (swapped && start <= end);

            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));

            return steps;
        }
    }
}
