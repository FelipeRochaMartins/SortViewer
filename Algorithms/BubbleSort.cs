using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class BubbleSort : BaseSortAlgorithm
    {
        public override string Name => "Bubble Sort";
        
        public override string Description => "A simple sorting algorithm that repeatedly steps through the list, " +
            "compares adjacent elements and swaps them if they are in the wrong order.\\n\\n" +
            "Time complexity: O(n²)";

        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();
            
            int n = array.Length;
            
            steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), "Initial state of the array"));
            
            for (int i = 0; i < n - 1; i++)
            {
                bool swapped = false;
                
                for (int j = 0; j < n - i - 1; j++)
                {
                    int comparisonResult = array[j].CompareTo(array[j + 1]);
                    steps.Add(CreateStep(array, OperationType.Comparison, new int[] { j, j + 1 }, Array.Empty<int>(), $"Comparing elements at positions {j} [{array[j]}] and {j+1} [{array[j+1]}]"));
                    RaiseComparisonEvent(j, j + 1, comparisonResult, array);
                    
                    if (comparisonResult > 0)
                    {
                        Swap(array, j, j + 1);
                        steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { j, j + 1 }, $"Swapping elements: {array[j+1]} ↔ {array[j]}"));
                        RaiseSwapEvent(j, j + 1, array);
                        swapped = true;
                    }
                }
                
                if (!swapped)
                    break;
            }
            
            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Array is now sorted"));
            
            return steps;
        }
    }
}
