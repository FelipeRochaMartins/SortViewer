using System;
using System.Collections.Generic;
using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class QuickSort : BaseSortAlgorithm
    {
        public override string Name => "Quick Sort";
        
        public override string Description => "An efficient sorting algorithm that uses a divide-and-conquer strategy. " +
            "It selects an element as a pivot and partitions the array, placing elements smaller than the pivot to the left and larger ones to the right. " +
            "Average time complexity: O(n log n)";

        private List<SortingStep> _steps;
        private Random _random = new Random();
        
        public override List<SortingStep> Sort(int[] data)
        {
            _steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();
            
            // Add initial state
            _steps.Add(CreateStep(array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), "Initial state"));
            
            QuickSortAlgorithm(array, 0, array.Length - 1);
            
            // Add final state
            _steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));
            
            return _steps;
        }
        
        private void QuickSortAlgorithm(int[] array, int low, int high)
        {
            if (low < high)
            {
                // Use a better pivot selection strategy to avoid worst-case with sorted/reversed arrays
                int pivotIndex = Partition(array, low, high);
                
                QuickSortAlgorithm(array, low, pivotIndex - 1);
                QuickSortAlgorithm(array, pivotIndex + 1, high);
            }
        }
        
        private int Partition(int[] array, int low, int high)
        {
            // Use median-of-three pivot selection to improve performance with nearly sorted/reversed arrays
            int mid = low + (high - low) / 2;
            
            // Sort low, mid, high elements to get a better pivot
            if (array[mid] < array[low])
                Swap(array, low, mid);
                
            if (array[high] < array[low])
                Swap(array, low, high);
                
            if (array[mid] < array[high])
                Swap(array, mid, high);
            
            // Now array[high] is the median of the three
            int pivot = array[high];
            
            _steps.Add(CreateStep(array, OperationType.Read, 
                new int[] { high }, new int[] { low, high }, $"Choosing pivot: {pivot}"));
            
            int i = low - 1;
            
            for (int j = low; j < high; j++)
            {
                // Compare with pivot and record comparison
                int comparisonResult = array[j].CompareTo(pivot);
                _steps.Add(CreateStep(array, OperationType.Comparison, 
                    new int[] { j, high }, new int[] { low, high }, $"Comparing {array[j]} with pivot {pivot}"));
                
                if (comparisonResult <= 0)
                {
                    i++;
                    
                    // Swap elements if needed and record swap
                    if (i != j)
                    {
                        Swap(array, i, j);
                        _steps.Add(CreateStep(array, OperationType.Swap, 
                            new int[] { i, j }, new int[] { low, high }, $"Swapping {array[i]} and {array[j]}"));
                    }
                }
            }
            
            // Place pivot in correct position
            if (i + 1 != high)
            {
                Swap(array, i + 1, high);
                _steps.Add(CreateStep(array, OperationType.Swap, 
                    new int[] { i + 1, high }, new int[] { low, high }, $"Placing pivot {pivot} in position {i + 1}"));
            }
            
            return i + 1;
        }
        
        private new void Swap(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
