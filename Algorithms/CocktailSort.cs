using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class CocktailSort : BaseSortAlgorithm
    {
        public override string Name => "Cocktail Sort";
        public override string Description => "A variation of Bubble Sort that sorts in both directions, also known as Bidirectional Bubble Sort.\n" +
            "It traverses the array from front to back and then back to front repeatedly.\n" +
            "This bidirectional approach helps to eliminate turtles (small values near the end) faster.\n\n" +
            "Time complexity: O(n²) in worst and average cases";

        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();

            int n = array.Length;

            steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state of the array"));

            bool swapped;
            int start = 0;
            int end = n - 1;

            do
            {
                swapped = false;
                steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), $"Forward pass from position {start} to position {end}"));

                for (int i = start; i < end; i++)
                {
                    int comparisonResult = array[i].CompareTo(array[i + 1]);
                    steps.Add(CreateStep(array, OperationType.Comparison, [i, i + 1], Array.Empty<int>(), $"Comparing elements at positions {i} [{array[i]}] and {i+1} [{array[i+1]}]"));
                    RaiseComparisonEvent(i, i + 1, comparisonResult, array);

                    if (comparisonResult > 0)
                    {
                        Swap(array, i, i + 1);
                        steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), [i, i + 1], $"Swapping elements: {array[i+1]} ↔ {array[i]}"));
                        RaiseSwapEvent(i, i + 1, array);
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    break;
                }

                end--;

                swapped = false;
                steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), 
                    $"Backward pass from position {end} to position {start}"));

                for (int i = end; i > start; i--)
                {
                    int comparisonResult = array[i].CompareTo(array[i - 1]);
                    steps.Add(CreateStep(array, OperationType.Comparison, [i, i - 1], Array.Empty<int>(), $"Comparing elements at positions {i} [{array[i]}] and {i-1} [{array[i-1]}]"));
                    RaiseComparisonEvent(i, i - 1, comparisonResult, array);

                    if (comparisonResult < 0)
                    {
                        Swap(array, i, i - 1);
                        steps.Add(CreateStep(array, OperationType.Swap,  Array.Empty<int>(), [i, i - 1], $"Swapping elements: {array[i-1]} ↔ {array[i]}"));
                        RaiseSwapEvent(i, i - 1, array);
                        swapped = true;
                    }
                }

                start++;

            } while (swapped && start <= end);

            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Array is now sorted"));

            return steps;
        }
    }
}
