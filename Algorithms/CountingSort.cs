using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class CountingSort : BaseSortAlgorithm
    {
        public override string Name => "Counting Sort";
        public override string Description => "A non-comparison sorting algorithm that works by counting occurrences of each element and " +
            "placing them in the correct order. Suitable for integers within a fixed range. " +
            "Time complexity: O(n + k), where k is the range of input values";

        public override List<SortingStep> Sort(int[] data)
        {
            var steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();

            int n = array.Length;

            steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state"));
            
            if (array.Length == 0)
            {
                return steps;
            }

            int max = array[0];
            int min = array[0];
            
            steps.Add(CreateStep(array, OperationType.Read, new int[] { 0 }, Array.Empty<int>(), "Reading first element"));

            for (int i = 1; i < n; i++)
            {
                steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"Reading element at index {i}"));
                
                int comparisonResultMax = array[i].CompareTo(max);
                RaiseComparisonEvent(i, 0, comparisonResultMax, array);
                steps.Add(CreateStep(array, OperationType.Comparison, new int[] { i, 0 }, Array.Empty<int>(), $"Comparing {array[i]} with current max {max}"));
                
                if (array[i] > max)
                {
                    max = array[i];
                    steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"New max found: {max}"));
                }
                
                int comparisonResultMin = array[i].CompareTo(min);
                RaiseComparisonEvent(i, 0, comparisonResultMin, array);
                steps.Add(CreateStep(array, OperationType.Comparison, new int[] { i, 0 }, Array.Empty<int>(), $"Comparing {array[i]} with current min {min}"));
                
                if (array[i] < min)
                {
                    min = array[i];
                    steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"New min found: {min}"));
                }
            }

            int range = max - min + 1;
            int[] count = new int[range];
            int[] output = new int[array.Length];
            
            steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), 
                $"Created count array with size {range} for range {min} to {max}"));

            for (int i = 0; i < n; i++)
            {
                int countIndex = array[i] - min;
                count[countIndex]++;
                
                steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), 
                    $"Counting element {array[i]}, count: {count[countIndex]}"));
            }

            for (int i = 1; i < count.Length; i++)
            {
                count[i] += count[i - 1];
                
                steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), 
                    $"Calculating cumulative count for value {i + min}: {count[i]}"));
            }

            for (int i = n - 1; i >= 0; i--)
            {
                int countIndex = array[i] - min;
                int outputIndex = count[countIndex] - 1;
                output[outputIndex] = array[i];
                count[countIndex]--;
                
                int[] indices = { i, outputIndex };
                steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), 
                    $"Reading element {array[i]} at index {i}"));
                
                int[] tempArray = (int[])array.Clone();
                tempArray[outputIndex] = array[i];
                
                steps.Add(new SortingStep
                {
                    CurrentState = tempArray,
                    OperationType = OperationType.Swap,
                    SwappedIndices = new int[] { i, outputIndex },
                    Description = $"Placing {array[i]} at position {outputIndex} in output array",
                    AlgorithmName = this.Name
                });
                
                RaiseSwapEvent(i, outputIndex, tempArray);
            }

            for (int i = 0; i < n; i++)
            {
                array[i] = output[i];
                
                steps.Add(CreateStep(array, OperationType.Write, new int[] { i }, Array.Empty<int>(), 
                    $"Copying sorted element {array[i]} to index {i}"));
            }

            steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));

            return steps;
        }
    }
}
