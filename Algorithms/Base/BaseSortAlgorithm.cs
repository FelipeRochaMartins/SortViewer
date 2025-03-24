using System;
using System.Collections.Generic;
using SortViewer.Models;

namespace SortViewer.Algorithms.Base
{
    /// <summary>
    /// Base class for implementing sorting algorithms
    /// </summary>
    public abstract class BaseSortAlgorithm : ISortAlgorithm
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public event EventHandler<ComparisonEventArgs> ComparisonPerformed;
        public event EventHandler<SwapEventArgs> SwapPerformed;

        public abstract List<SortingStep> Sort(int[] data);

        /// <summary>
        /// Method to register a comparison
        /// </summary>
        protected void RaiseComparisonEvent(int firstIndex, int secondIndex, int result, int[] currentState)
        {
            ComparisonPerformed?.Invoke(this, new ComparisonEventArgs(firstIndex, secondIndex, result, (int[])currentState.Clone()));
        }

        /// <summary>
        /// Method to register a swap
        /// </summary>
        protected void RaiseSwapEvent(int firstIndex, int secondIndex, int[] currentState)
        {
            SwapPerformed?.Invoke(this, new SwapEventArgs(firstIndex, secondIndex, (int[])currentState.Clone()));
        }

        /// <summary>
        /// Utility method to register a sorting step
        /// </summary>
        protected SortingStep CreateStep(int[] currentState, OperationType operationType, 
            int[] comparedIndices = null, int[] swappedIndices = null, string description = null)
        {
            return new SortingStep
            {
                CurrentState = (int[])currentState.Clone(),
                OperationType = operationType,
                ComparedIndices = comparedIndices,
                SwappedIndices = swappedIndices,
                Description = description,
                AlgorithmName = this.Name
            };
        }

        /// <summary>
        /// Method to swap two elements in an array
        /// </summary>
        protected void Swap(int[] array, int i, int j)
        {
            if (i == j) return;
            
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
