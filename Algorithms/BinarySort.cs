using SortViewer.Models;
using SortViewer.Algorithms.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SortViewer.Algorithms
{
    public class BinarySort : BaseSortAlgorithm
    {
        public override string Name => "Binary Sort";
        public override string Description => "A variation of Insertion Sort that uses binary search to find the correct position to insert elements.\n" +
            "Reduces the number of comparisons but still requires shifting elements.\n\n" +
            "Time complexity: O(n²) for swaps, O(n log n) for comparisons";

        private List<SortingStep> _steps = new List<SortingStep>();
        private int[] _array = Array.Empty<int>();

        public override List<SortingStep> Sort(int[] data)
        {
            _steps = new List<SortingStep>();
            
            _array = (int[])data.Clone();
            int n = _array.Length;
            
            _steps.Add(CreateStep(_array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state of the array"));

            for (int i = 1; i < n; i++)
            {
                int key = _array[i];
                
                _steps.Add(CreateStep(_array, OperationType.Read, new int[] { i }, new int[] { i }, $"Selecting element at position {i} [{key}] for insertion"));
                
                int pos = BinarySearch(0, i - 1, key, i);
                
                if (pos == i)
                {
                    _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { i }, new int[] { i }, $"Element at position {i} [{key}] is already in the correct position"));
                    continue;
                }
                
                int j = i;
                while (j > pos)
                {
                    _steps.Add(CreateStep(_array, OperationType.Swap, new int[] { j, j - 1 }, new int[] { j, j - 1 }, 
                        $"Shifting element at position {j-1} [{_array[j-1]}] one position right"));
                    
                    _array[j] = _array[j - 1];
                    
                    RaiseSwapEvent(j, j - 1, _array);
                    j--;
                }
                
                _array[pos] = key;
                
                _steps.Add(CreateStep(_array, OperationType.Write, new int[] { pos }, new int[] { i }, $"Inserted element {key} at position {pos}"));

                int[] sortedPart = Enumerable.Range(0, i + 1).ToArray();
                _steps.Add(CreateStep(_array, OperationType.Comparison, sortedPart, new int[] { pos }, $"Subarray after insertion: elements 0 to {i} are now sorted"));
            }
            
            _steps.Add(CreateStep(_array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Array is now sorted"));
            
            return _steps;
        }
        
        private int BinarySearch(int left, int right, int key, int currentPos)
        {
            if (right <= left)
            {
                if (key < _array[left])
                {
                    _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { left }, new int[] { currentPos }, $"Element {key} < element at position {left} [{_array[left]}], insert at position {left}"));
                    return left;
                }
                else
                {
                    _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { left }, new int[] { currentPos }, $"Element {key} >= element at position {left} [{_array[left]}], insert at position {left + 1}"));
                    return left + 1;
                }
            }
            
            int mid = left + (right - left) / 2;
            
            int[] searchInterval = Enumerable.Range(left, right - left + 1).ToArray();
            _steps.Add(CreateStep(_array, OperationType.Comparison, searchInterval, new int[] { mid }, $"Searching for insertion position in interval [{left}-{right}], checking position {mid} [{_array[mid]}]"));
            
            if (key == _array[mid])
            {
                _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { mid }, new int[] { currentPos }, $"Element {key} == element at position {mid} [{_array[mid]}], insert after position {mid}"));
                return mid + 1;
            }
            
            if (key > _array[mid])
            {
                _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { mid }, new int[] { currentPos }, $"Element {key} > element at position {mid} [{_array[mid]}], search in right half"));
                return BinarySearch(mid + 1, right, key, currentPos);
            }
            
            _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { mid }, new int[] { currentPos }, $"Element {key} < element at position {mid} [{_array[mid]}], search in left half"));
            return BinarySearch(left, mid - 1, key, currentPos);
        }
    }
}
