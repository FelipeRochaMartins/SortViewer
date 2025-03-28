using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class InsertionSort : BaseSortAlgorithm
    {
        public override string Name => "Insertion Sort";
        
        public override string Description => "A simple sorting algorithm that builds the sorted array one item at a time, " +
            "taking one element from the unsorted part and inserting it into its correct position in the sorted part. " +
            "Time complexity: O(n²)";
        
        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();
            
            int n = array.Length;
            
            steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state"));

            for (int i = 1; i < n; i++)
            {
                int key = array[i];
                steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"Selecting key element: {key}"));
                
                int j = i - 1;
                
                while (j >= 0)
                {
                    int comparisonResult = array[j].CompareTo(key);
                    steps.Add(CreateStep(array, OperationType.Comparison, new int[] { j, i }, Array.Empty<int>(), $"Comparing {array[j]} with key {key}"));
                    RaiseComparisonEvent(j, i, comparisonResult, array);
                    
                    if (comparisonResult > 0)
                    {
                        int temp = array[j + 1];
                        array[j + 1] = array[j];

                        steps.Add(CreateStep(array, OperationType.Write, new int[] { j + 1 }, new int[] { j }, $"Shifting {array[j+1]} to position {j + 1}"));

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
                    
                    steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { i, j + 1 }, $"Placing key {key} at position {j + 1}"));

                    RaiseSwapEvent(i, j + 1, array);
                }
            }

            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));
            
            return steps;
        }
    }
}
