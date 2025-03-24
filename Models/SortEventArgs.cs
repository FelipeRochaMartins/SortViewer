using System;

namespace SortViewer.Models
{
    /// <summary>
    /// Arguments for comparison events in sorting algorithms
    /// </summary>
    public class ComparisonEventArgs : EventArgs
    {
        /// <summary>
        /// Index of the first compared element
        /// </summary>
        public int FirstIndex { get; }
        
        /// <summary>
        /// Index of the second compared element
        /// </summary>
        public int SecondIndex { get; }
        
        /// <summary>
        /// Result of the comparison
        /// </summary>
        public int Result { get; }
        
        /// <summary>
        /// Current state of the array
        /// </summary>
        public int[] CurrentState { get; }

        public ComparisonEventArgs(int firstIndex, int secondIndex, int result, int[] currentState)
        {
            FirstIndex = firstIndex;
            SecondIndex = secondIndex;
            Result = result;
            CurrentState = currentState;
        }
    }

    /// <summary>
    /// Arguments for swap events in sorting algorithms
    /// </summary>
    public class SwapEventArgs : EventArgs
    {
        /// <summary>
        /// Index of the first swapped element
        /// </summary>
        public int FirstIndex { get; }
        
        /// <summary>
        /// Index of the second swapped element
        /// </summary>
        public int SecondIndex { get; }
        
        /// <summary>
        /// Current state of the array
        /// </summary>
        public int[] CurrentState { get; }

        public SwapEventArgs(int firstIndex, int secondIndex, int[] currentState)
        {
            FirstIndex = firstIndex;
            SecondIndex = secondIndex;
            CurrentState = currentState;
        }
    }
}
