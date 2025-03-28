using System;
using System.Collections.Generic;
using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class QuickSort : BaseSortAlgorithm
    {
        public override string Name => "Quick Sort";
        
        public override string Description => "A divide-and-conquer algorithm that selects a 'pivot' element and partitions the array around it.\n" +
            "Elements smaller than the pivot go to the left, larger elements go to the right.\n" +
            "This process is repeated recursively for both partitions.\n\n" +
            "Time complexity: O(n log n) average case, O(n²) worst case";

        private List<SortingStep> _steps;
        
        public override List<SortingStep> Sort(int[] data)
        {
            _steps = new List<SortingStep>();
            int[] array = (int[])data.Clone();
            
            _steps.Add(CreateStep(array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state of the array"));
            
            QuickSortAlgorithm(array, 0, array.Length - 1);
            
            _steps.Add(CreateStep(array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Array is now sorted"));
            
            return _steps;
        }
        
        private void QuickSortAlgorithm(int[] array, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(array, low, high);
                
                QuickSortAlgorithm(array, low, pivotIndex - 1);
                QuickSortAlgorithm(array, pivotIndex + 1, high);
            }
        }
        
        private int Partition(int[] array, int low, int high)
        {
            int mid = low + (high - low) / 2;
            
            if (array[mid] < array[low]) {
                Swap(array, low, mid);
                _steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { low, mid }, $"Swapping elements: {array[mid]} ↔ {array[low]}"));
                RaiseSwapEvent(low, mid, array);
            }
                
            if (array[high] < array[low]) {
                Swap(array, low, high);
                _steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { low, high }, $"Swapping elements: {array[high]} ↔ {array[low]}"));
                RaiseSwapEvent(low, high, array);
            }
                
            if (array[mid] < array[high]) {
                Swap(array, mid, high);
                _steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { mid, high }, $"Swapping elements: {array[high]} ↔ {array[mid]}"));
                RaiseSwapEvent(mid, high, array);
            }
            
            int pivot = array[high];
            
            RaiseComparisonEvent(high, high, 0, array);
            _steps.Add(CreateStep(array, OperationType.Read, new int[] { high }, Array.Empty<int>(), $"Choosing pivot element at position {high} [{pivot}]"));
            
            int i = low - 1;
            
            for (int j = low; j < high; j++)
            {
                int comparisonResult = array[j].CompareTo(pivot);
                RaiseComparisonEvent(j, high, comparisonResult, array);
                _steps.Add(CreateStep(array, OperationType.Comparison, new int[] { j, high }, Array.Empty<int>(), $"Comparing elements at positions {j} [{array[j]}] and {high} [{pivot}]"));
                
                if (comparisonResult <= 0)
                {
                    i++;
                    
                    if (i != j)
                    {
                        Swap(array, i, j);
                        RaiseSwapEvent(i, j, array);
                        _steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { i, j }, $"Swapping elements: {array[i]} ↔ {array[j]}"));
                    }
                }
            }
            
            if (i + 1 != high)
            {
                Swap(array, i + 1, high);
                RaiseSwapEvent(i + 1, high, array);
                _steps.Add(CreateStep(array, OperationType.Swap, Array.Empty<int>(), new int[] { i + 1, high }, $"Placing pivot element {pivot} at final position {i + 1}"));
            }
            
            return i + 1;
        }
    }
}
