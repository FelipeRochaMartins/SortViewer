using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Algorithms
{
    public class HeapSort : BaseSortAlgorithm
    {
        public override string Name => "Heap Sort";
        public override string Description => "A comparison-based sorting algorithm that uses a binary heap data structure, " +
            "turning the input into a heap and repeatedly extracting the maximum element. " +
            "Time complexity: O(n log n)";

        private List<SortingStep> _steps = new List<SortingStep>();
        private int[] _array = Array.Empty<int>();

        public override List<SortingStep> Sort(int[] data)
        {
            _steps = new List<SortingStep>();
            _array = (int[])data.Clone();

            int n = _array.Length;

            _steps.Add(CreateStep(_array, OperationType.Initial, Array.Empty<int>(), Array.Empty<int>(), "Initial state"));

            _steps.Add(CreateStep(_array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), "Building max heap"));
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                Heapify(n, i);
            }

            _steps.Add(CreateStep(_array, OperationType.Read, Array.Empty<int>(), Array.Empty<int>(), "Heap built, extracting elements"));
            for (int i = n - 1; i > 0; i--)
            {
                _steps.Add(CreateStep(_array, OperationType.Swap, Array.Empty<int>(), [0, i], $"Moving max element {_array[0]} to position {i}"));
                Swap(_array, 0, i);
                RaiseSwapEvent(0, i, _array);

                Heapify(i, 0);
            }

            _steps.Add(CreateStep(_array, OperationType.Final, Array.Empty<int>(), Array.Empty<int>(), "Sorting completed"));

            return _steps;
        }

        private void Heapify(int heapSize, int rootIndex)
        {
            int largest = rootIndex;
            int leftChild = 2 * rootIndex + 1;
            int rightChild = 2 * rootIndex + 2;

            if (leftChild < heapSize)
            {
                int comparisonResult = _array[leftChild].CompareTo(_array[largest]);
                _steps.Add(CreateStep(_array, OperationType.Comparison, [leftChild, largest], Array.Empty<int>(), $"Comparing left child {_array[leftChild]} with largest {_array[largest]}"));
                RaiseComparisonEvent(leftChild, largest, comparisonResult, _array);
                
                if (comparisonResult > 0)
                {
                    largest = leftChild;
                }
            }

            if (rightChild < heapSize)
            {
                int comparisonResult = _array[rightChild].CompareTo(_array[largest]);
                _steps.Add(CreateStep(_array, OperationType.Comparison, [rightChild, largest], Array.Empty<int>(), $"Comparing right child {_array[rightChild]} with largest {_array[largest]}"));
                RaiseComparisonEvent(rightChild, largest, comparisonResult, _array);
                
                if (comparisonResult > 0)
                {
                    largest = rightChild;
                }
            }

            if (largest != rootIndex)
            {
                _steps.Add(CreateStep(_array, OperationType.Swap, Array.Empty<int>(), [rootIndex, largest], $"Swapping {_array[rootIndex]} with {_array[largest]}"));
                Swap(_array, rootIndex, largest);
                RaiseSwapEvent(rootIndex, largest, _array);

                Heapify(heapSize, largest);
            }
        }
    }
}
