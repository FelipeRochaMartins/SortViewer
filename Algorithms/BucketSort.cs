using SortViewer.Models;
using SortViewer.Algorithms.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SortViewer.Algorithms
{
    public class BucketSort : BaseSortAlgorithm
    {
        public override string Name => "Bucket Sort";
        public override string Description => "A sorting algorithm that distributes elements into a number of buckets based on their value ranges,\n" +
            "sorts each bucket individually using HeapSort, and then concatenates them. \n" +
            "Uses sqrt(n) buckets for optimal performance. \n\n" +
            "Time complexity: O(n + k) average case, O(n²) worst case";

        private List<SortingStep> _steps = new List<SortingStep>();
        private int[] _array = Array.Empty<int>();

        public override List<SortingStep> Sort(int[] data)
        {
            _steps = new List<SortingStep>();
            _array = (int[])data.Clone();
            int n = _array.Length;

            _steps.Add(CreateStep(_array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state of the array"));

            int min = _array[0], max = _array[0];
            for (int i = 0; i < n; i++)
            {
                _steps.Add(CreateStep(_array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"Reading element at position {i} [{_array[i]}] to find min/max values"));
                if (_array[i] < min) min = _array[i];
                if (_array[i] > max) max = _array[i];
            }

            int bucketCount = (int)Math.Sqrt(n);
            bucketCount = Math.Max(bucketCount, 1);
            int rangeSize = (max - min) / bucketCount + 1;
            
            int[] allIndices = Enumerable.Range(0, n).ToArray();
            _steps.Add(CreateStep(_array, OperationType.Comparison, allIndices, allIndices, $"Creating {bucketCount} buckets with range size {rangeSize} (sqrt({n}) ≈ {bucketCount})"));

            int[] originalArray = (int[])_array.Clone();
            List<int>[] buckets = new List<int>[bucketCount];
            
            for (int i = 0; i < bucketCount; i++)
            {
                buckets[i] = new List<int>();
                int bucketMin = min + (i * rangeSize);
                int bucketMax = Math.Min(max, min + ((i + 1) * rangeSize) - 1);
                _steps.Add(CreateStep(_array, OperationType.Comparison, allIndices, new int[] { i }, $"Bucket {i}: will contain values from {bucketMin} to {bucketMax}"));
            }

            int[] bucketIndices = new int[bucketCount];
            _steps.Add(CreateStep(_array, OperationType.Comparison, allIndices, allIndices, "PHASE 1: Distributing elements into buckets by value range"));
            
            Dictionary<int, int> originalPositions = new Dictionary<int, int>();
            bool[] movedToBucket = new bool[n];
            
            for (int i = 0; i < n; i++)
            {
                int value = _array[i];
                int bucketIndex = Math.Min(bucketCount - 1, (value - min) / rangeSize);
                
                if (!originalPositions.ContainsKey(value))
                {
                    originalPositions[value] = i;
                }
                
                _steps.Add(CreateStep(_array, OperationType.Read, new int[] { i }, new int[] { bucketIndex }, $"Reading element at position {i} [{value}] for bucket {bucketIndex}"));
                buckets[bucketIndex].Add(value);
                bucketIndices[bucketIndex]++;
                movedToBucket[i] = true;
                _steps.Add(CreateStep(_array, OperationType.Write, new int[] { i }, new int[] { bucketIndex }, $"Moving element {value} to bucket {bucketIndex}"));
            }
            
            _steps.Add(CreateStep(_array, OperationType.Comparison, allIndices, allIndices, "Visual representation: Elements grouped by buckets"));
            
            int currentPos = 0;
            int[][] bucketArrays = new int[bucketCount][];
            
            for (int i = 0; i < bucketCount; i++)
            {
                if (buckets[i].Count > 0)
                {
                    bucketArrays[i] = buckets[i].ToArray();
                    
                    for (int j = 0; j < bucketArrays[i].Length; j++)
                    {
                        int value = bucketArrays[i][j];
                        int originalPos = originalPositions[value];
                        
                        if (currentPos != originalPos)
                        {
                            int temp = _array[currentPos];
                            _array[currentPos] = value;
                            
                            _steps.Add(CreateStep(_array, OperationType.Swap, new int[] { originalPos, currentPos }, new int[] { originalPos, currentPos }, $"Grouping element {value} with other elements in bucket {i}"));
                            RaiseSwapEvent(originalPos, currentPos, _array);
                        }
                        
                        currentPos++;
                    }
                    
                    int[] bucketIndexRange = Enumerable.Range(currentPos - bucketArrays[i].Length, bucketArrays[i].Length).ToArray();
                    _steps.Add(CreateStep(_array, OperationType.Comparison, bucketIndexRange, bucketIndexRange, $"Bucket {i} contains {bucketArrays[i].Length} elements with range {min + (i * rangeSize)} to {Math.Min(max, min + ((i + 1) * rangeSize) - 1)}"));
                }
            }
            
            int[] allCurrentIndices = Enumerable.Range(0, n).ToArray();
            _steps.Add(CreateStep(_array, OperationType.Comparison, allCurrentIndices, allCurrentIndices, "PHASE 2: Sorting each bucket"));
            
            currentPos = 0;
            for (int i = 0; i < bucketCount; i++)
            {
                if (bucketArrays[i] != null && bucketArrays[i].Length > 0)
                {
                    int bucketSize = bucketArrays[i].Length;
                    int bucketStart = currentPos;
                    
                    int[] bucketIndexRange = Enumerable.Range(bucketStart, bucketSize).ToArray();
                    _steps.Add(CreateStep(_array, OperationType.Comparison, bucketIndexRange, bucketIndexRange, $"Sorting bucket {i} with {bucketSize} elements"));
                    
                    HeapSortBucket(bucketStart, bucketSize, i);
                    
                    _steps.Add(CreateStep(_array, OperationType.Comparison, bucketIndexRange, bucketIndexRange, $"Bucket {i} sorted successfully"));
                    currentPos += bucketSize;
                }
            }
            
            _steps.Add(CreateStep(_array, OperationType.Comparison, allIndices, allIndices, "PHASE 3: Buckets concatenated in sorted order"));
            _steps.Add(CreateStep(_array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Array is now sorted"));

            return _steps;
        }
        
        private void HeapSortBucket(int start, int size, int bucketIndex)
        {
            for (int i = size / 2 - 1; i >= 0; i--)
                Heapify(start, size, i, bucketIndex);

            for (int i = size - 1; i > 0; i--)
            {
                int temp = _array[start];
                _array[start] = _array[start + i];
                _array[start + i] = temp;
                
                _steps.Add(CreateStep(_array, OperationType.Swap, new int[] { start, start + i }, new int[] { start, start + i }, 
                    $"Moving largest element [{_array[start + i]}] to position {start + i} (end of bucket {bucketIndex})"));
                RaiseSwapEvent(start, start + i, _array);

                Heapify(start, i, 0, bucketIndex);
            }
        }

        private void Heapify(int start, int size, int root, int bucketIndex)
        {
            int largest = root;
            int left = 2 * root + 1;
            int right = 2 * root + 2;

            if (left < size && _array[start + left] > _array[start + largest])
            {
                _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { start + left, start + largest }, new int[] { start + left, start + largest },
                    $"Comparing elements at positions {start + left} [{_array[start + left]}] and {start + largest} [{_array[start + largest]}] in bucket {bucketIndex}"));
                largest = left;
            }

            if (right < size && _array[start + right] > _array[start + largest])
            {
                _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { start + right, start + largest }, new int[] { start + right, start + largest },
                    $"Comparing elements at positions {start + right} [{_array[start + right]}] and {start + largest} [{_array[start + largest]}] in bucket {bucketIndex}"));
                largest = right;
            }

            if (largest != root)
            {
                int swap = _array[start + root];
                _array[start + root] = _array[start + largest];
                _array[start + largest] = swap;
                
                _steps.Add(CreateStep(_array, OperationType.Swap, new int[] { start + root, start + largest }, new int[] { start + root, start + largest },
                    $"Swapping elements: {_array[start + root]} ↔ {_array[start + largest]} in bucket {bucketIndex}"));
                RaiseSwapEvent(start + root, start + largest, _array);

                Heapify(start, size, largest, bucketIndex);
            }
        }
    }
}
