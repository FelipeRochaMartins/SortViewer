using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class InsertionSort : BaseSortAlgorithm
    {
        public override string Name => "Insertion Sort";
        
        public override string Description => "A simple sorting algorithm that builds the final sorted array one item at a time.\n" +
            "It iterates through an array, consuming one input element each repetition, and growing a sorted output list.\n" +
            "Efficient for small or nearly sorted data sets.\n\n" +
            "Time complexity: O(n²) worst and average case, O(n) best case";
        
        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();
            
            int n = array.Length;
            
            steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state of the array"));

            for (int i = 1; i < n; i++)
            {
                int key = array[i];
                steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"Selecting key element at position {i} [{key}]"));
                
                int j = i - 1;
                
                while (j >= 0)
                {
                    int comparisonResult = array[j].CompareTo(key);
                    steps.Add(CreateStep(array, OperationType.Comparison, new int[] { j, i }, Array.Empty<int>(), $"Comparing elements at positions {j} [{array[j]}] and {i} [{key}]"));
                    RaiseComparisonEvent(j, i, comparisonResult, array);
                    
                    if (comparisonResult > 0)
                    {
                        int temp = array[j + 1];
                        array[j + 1] = array[j];

                        steps.Add(CreateStep(array, OperationType.Write, new int[] { j + 1 }, new int[] { j }, $"Shifting element {array[j+1]} to position {j + 1}"));

                        j--;
                    }
                    else
                    {
                        break;
                    }
                }
                
                if (j + 1 != i)
                {
                    array[j + 1] = key;
                    
                    steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { i, j + 1 }, $"Placing key element {key} at position {j + 1}"));

                    RaiseSwapEvent(i, j + 1, array);
                }
            }

            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Array is now sorted"));
            
            return steps;
        }
    }
}
