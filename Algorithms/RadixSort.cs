using SortViewer.Models;
using SortViewer.Algorithms.Base;
using System;
using System.Collections.Generic;

namespace SortViewer.Algorithms
{
    public class RadixSort : BaseSortAlgorithm
    {
        public override string Name => "Radix Sort (LSD)";
        public override string Description => "A non-comparative sorting algorithm that processes the digits of each number from least to most significant.\n" +
            "It distributes elements into buckets according to each digit's value and reconstructs the array after each pass.\n" +
            "Particularly efficient for integers with limited number of digits.\n\n" +
            "Time complexity: O(d * (n + k)) where d is the number of digits and k is the range of each digit";

        private List<SortingStep> _steps = new List<SortingStep>();
        private int[] _array = Array.Empty<int>();

        public override List<SortingStep> Sort(int[] data)
        {
            _steps = new List<SortingStep>();
            _array = (int[])data.Clone();

            int n = _array.Length;

            _steps.Add(CreateStep(_array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state"));

            int max = _array[0];
            foreach (int num in _array)
            {
                if (num > max)
                {
                    max = num;
                }
            }

            for (int exp = 1; max / exp > 0; exp *= 10)
            {
                string digitName = exp == 1 ? "ones" : exp == 10 ? "tens" : exp == 100 ? "hundreds" : "thousands";
                _steps.Add(CreateStep(_array, OperationType.Comparison, Array.Empty<int>(), Array.Empty<int>(), $"Starting to sort by {digitName} digit"));
                CountingSort(_array, exp);
            }

            _steps.Add(CreateStep(_array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));

            return _steps;
        }

        private void CountingSort(int[] array, int exp)
        {
            int n = array.Length;
            int[] output = new int[n];
            int[] count = new int[10]; // 0-9 digits

            for (int i = 0; i < n; i++)
            {
                int digit = (array[i] / exp) % 10;
                count[digit]++;
                
                _steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"Reading digit {digit} from number {array[i]}"));
            }

            _steps.Add(CreateStep(array, OperationType.Comparison, Array.Empty<int>(), Array.Empty<int>(), "Calculating cumulative count to determine positions"));
            
            for (int i = 1; i < 10; i++)
            {
                count[i] += count[i - 1];
                _steps.Add(CreateStep(array, OperationType.Comparison, new int[] { i, i-1 }, Array.Empty<int>(), $"Cumulative count for digit {i}: {count[i]}"));
            }

            for (int i = n - 1; i >= 0; i--)
            {
                int digit = (array[i] / exp) % 10;
                int outputIndex = count[digit] - 1;
                
                _steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"Processing number {array[i]} with {digit} in the current digit position"));
                
                output[outputIndex] = array[i];
                
                _steps.Add(CreateStep(array, OperationType.Write, new int[] { i }, new int[] { outputIndex }, $"Writing {array[i]} to temporary position {outputIndex}"));
                
                count[digit]--;
            }

            for (int i = 0; i < n; i++)
            {
                int previousValue = array[i];
                
                if (previousValue != output[i])
                {
                    int newPositionOfPrevious = Array.IndexOf(output, previousValue);
                    
                    _steps.Add(CreateStep(array, OperationType.Swap, new int[] { i, newPositionOfPrevious }, new int[] { i, newPositionOfPrevious }, $"Moving {output[i]} to position {i}"));
                    
                    array[i] = output[i];
                    RaiseSwapEvent(i, newPositionOfPrevious, array);
                }
                else
                {
                    _steps.Add(CreateStep(array, OperationType.Read, new int[] { i }, Array.Empty<int>(), $"Number {array[i]} stays at position {i}"));
                    
                    array[i] = output[i];
                }
            }
            
            _steps.Add(CreateStep(array, OperationType.Comparison, Array.Empty<int>(), Array.Empty<int>(), "Completed one digit pass"));
        }
    }
}
