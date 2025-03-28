using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class MergeSort : BaseSortAlgorithm
    {
        public override string Name => "Merge Sort";
        public override string Description => "A divide-and-conquer sorting algorithm that splits the list into halves, sorts them recursively, " +
            "and then merges them back together in order. " +
            "Time complexity: O(n log n)";

        private List<SortingStep> _steps = new List<SortingStep>();
        private int[] _array = Array.Empty<int>();

        public override List<SortingStep> Sort(int[] data)
        {
            _steps = new List<SortingStep>();
            _array = (int[])data.Clone();

            int n = _array.Length;

            _steps.Add(CreateStep(_array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state"));

            MergeSortRecursive(0, n - 1);

            _steps.Add(CreateStep(_array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));

            return _steps;
        }

        private void MergeSortRecursive(int left, int right)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;

                _steps.Add(CreateStep(_array, OperationType.Read, new int[] { middle }, Array.Empty<int>(), $"Dividing array from index {left} to {right} with middle at {middle}"));

                MergeSortRecursive(left, middle);
                MergeSortRecursive(middle + 1, right);

                Merge(left, middle, right);
            }
        }

        private void Merge(int left, int middle, int right)
        {
            _steps.Add(CreateStep(_array, OperationType.Read, new int[] { left, right }, Array.Empty<int>(), $"Merging subarrays from {left} to {middle} and from {middle + 1} to {right}"));

            int n1 = middle - left + 1;
            int n2 = right - middle;

            int[] leftArray = new int[n1];
            int[] rightArray = new int[n2];

            for (int leftIdx = 0; leftIdx < n1; leftIdx++)
            {
                leftArray[leftIdx] = _array[left + leftIdx];
                
                if (leftIdx == 0 || leftIdx == n1-1)
                {
                    _steps.Add(CreateStep(_array, OperationType.Read, new int[] { left + leftIdx }, Array.Empty<int>(), $"Copying elements to left temporary array"));
                }
            }

            for (int rightIdx = 0; rightIdx < n2; rightIdx++)
            {
                rightArray[rightIdx] = _array[middle + 1 + rightIdx];
                
                if (rightIdx == 0 || rightIdx == n2-1)
                {
                    _steps.Add(CreateStep(_array, OperationType.Read, new int[] { middle + 1 + rightIdx }, Array.Empty<int>(), $"Copying elements to right temporary array"));
                }
            }

            int i = 0, j = 0;
            int k = left;

            while (i < n1 && j < n2)
            {
                int comparisonResult = leftArray[i].CompareTo(rightArray[j]);

                _steps.Add(CreateStep(_array, OperationType.Comparison, new int[] { left + i, middle + 1 + j }, Array.Empty<int>(), $"Comparing {leftArray[i]} and {rightArray[j]}"));
                
                RaiseComparisonEvent(left + i, middle + 1 + j, comparisonResult, _array);

                if (comparisonResult <= 0)
                {
                    int originalValue = _array[k];
                    _array[k] = leftArray[i];
                    
                    _steps.Add(CreateStep(_array, OperationType.Write, new int[] { left + i }, new int[] { k }, $"Writing {leftArray[i]} at position {k}"));
                    
                    RaiseSwapEvent(left + i, k, _array);
                    i++;
                }
                else
                {
                    int originalValue = _array[k];
                    _array[k] = rightArray[j];
                    
                    _steps.Add(CreateStep(_array, OperationType.Write, new int[] { middle + 1 + j }, new int[] { k }, $"Writing {rightArray[j]} at position {k}"));
                    
                    RaiseSwapEvent(middle + 1 + j, k, _array);
                    j++;
                }
                k++;
            }

            while (i < n1)
            {
                int originalValue = _array[k];
                _array[k] = leftArray[i];
                
                _steps.Add(CreateStep(_array, OperationType.Write, new int[] { left + i }, new int[] { k }, $"Writing remaining left element {leftArray[i]} at position {k}"));
                
                RaiseSwapEvent(left + i, k, _array);
                i++;
                k++;
            }

            while (j < n2)
            {
                int originalValue = _array[k];
                _array[k] = rightArray[j];
                
                _steps.Add(CreateStep(_array, OperationType.Write, new int[] { middle + 1 + j }, new int[] { k }, $"Writing remaining right element {rightArray[j]} at position {k}"));
                
                RaiseSwapEvent(middle + 1 + j, k, _array);
                j++;
                k++;
            }

            _steps.Add(CreateStep(_array, OperationType.Read, new int[] { left, right }, Array.Empty<int>(), $"Finished merging subarray from {left} to {right}"));
        }
    }
}
