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
            if (array[mid] < array[low]) {
                Swap(array, low, mid);
                _steps.Add(CreateStep(array, OperationType.Swap, 
                    Array.Empty<int>(), new int[] { low, mid }, $"Swapping {array[mid]} and {array[low]}"));
                RaiseSwapEvent(low, mid, array);
            }
                
            if (array[high] < array[low]) {
                Swap(array, low, high);
                _steps.Add(CreateStep(array, OperationType.Swap, 
                    Array.Empty<int>(), new int[] { low, high }, $"Swapping {array[high]} and {array[low]}"));
                RaiseSwapEvent(low, high, array);
            }
                
            if (array[mid] < array[high]) {
                Swap(array, mid, high);
                _steps.Add(CreateStep(array, OperationType.Swap, 
                    Array.Empty<int>(), new int[] { mid, high }, $"Swapping {array[high]} and {array[mid]}"));
                RaiseSwapEvent(mid, high, array);
            }
            
            // Now array[high] is the median of the three
            int pivot = array[high];
            
            // Record reading the pivot
            RaiseComparisonEvent(high, high, 0, array);
            _steps.Add(CreateStep(array, OperationType.Read, 
                new int[] { high }, Array.Empty<int>(), $"Choosing pivot: {pivot}"));
            
            int i = low - 1;
            
            for (int j = low; j < high; j++)
            {
                // Compare with pivot and record comparison
                int comparisonResult = array[j].CompareTo(pivot);
                RaiseComparisonEvent(j, high, comparisonResult, array);
                _steps.Add(CreateStep(array, OperationType.Comparison, 
                    new int[] { j, high }, Array.Empty<int>(), $"Comparing {array[j]} with pivot {pivot}"));
                
                if (comparisonResult <= 0)
                {
                    i++;
                    
                    // Swap elements if needed and record swap
                    if (i != j)
                    {
                        Swap(array, i, j);
                        RaiseSwapEvent(i, j, array);
                        _steps.Add(CreateStep(array, OperationType.Swap, 
                            Array.Empty<int>(), new int[] { i, j }, $"Swapping {array[i]} and {array[j]}"));
                    }
                }
            }
            
            // Place pivot in correct position
            if (i + 1 != high)
            {
                Swap(array, i + 1, high);
                RaiseSwapEvent(i + 1, high, array);
                _steps.Add(CreateStep(array, OperationType.Swap, 
                    Array.Empty<int>(), new int[] { i + 1, high }, $"Placing pivot {pivot} in position {i + 1}"));
            }
            
            return i + 1;
        }
    }
}
