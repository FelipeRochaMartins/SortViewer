using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class ShellSort : BaseSortAlgorithm
    {
        public override string Name => "Shell Sort";
        public override string Description => "An optimization of Insertion Sort that allows exchanges of far-apart elements, " +
            "by sorting elements at specific gaps and progressively reducing the gap until it becomes 1. " +
            "Time complexity: O(n log² n) (depends on the gap sequence used)";

        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();

            int n = array.Length;

            steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state"));

            int gap = 1;
            while (gap < n / 3)
            {
                gap = 3 * gap + 1;
            }

            steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), $"Starting with gap: {gap}"));

            while (gap > 0)
            {
                for (int i = gap; i < n; i++)
                {
                    int temp = array[i];
                    steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"Selecting element {temp} at position {i} with gap {gap}"));

                    int j = i;
                    
                    while (j >= gap)
                    {
                        int comparisonResult = array[j - gap].CompareTo(temp);
                        steps.Add(CreateStep(array, OperationType.Comparison, new int[] { j - gap, i }, Array.Empty<int>(), $"Comparing {array[j - gap]} with key {temp} (gap: {gap})"));
                        RaiseComparisonEvent(j - gap, i, comparisonResult, array);
                        
                        if (comparisonResult > 0)
                        {
                            array[j] = array[j - gap];
                            steps.Add(CreateStep(array, OperationType.Write, new int[] { j }, new int[] { j - gap }, $"Shifting {array[j]} to position {j} (gap: {gap})"));
                            RaiseSwapEvent(j - gap, j, array);
                            j -= gap;
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    if (j != i)
                    {
                        array[j] = temp;
                        steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { i, j }, $"Placing {temp} at position {j} (gap: {gap})"));
                        RaiseSwapEvent(i, j, array);
                    }
                }
                
                gap = (gap - 1) / 3;
                
                if (gap > 0)
                {
                    steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), $"Reducing gap to: {gap}"));
                }
            }

            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));
            
            return steps;
        }
    }
}
