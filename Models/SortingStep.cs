using System;

namespace SortViewer.Models
{
    /// <summary>
    /// Represents a step during the sorting process
    /// </summary>
    public class SortingStep
    {
        /// <summary>
        /// Current state of the array
        /// </summary>
        public int[] CurrentState { get; set; }
        
        /// <summary>
        /// Indices being compared (if any)
        /// </summary>
        public int[] ComparedIndices { get; set; }
        
        /// <summary>
        /// Indices involved in a swap (if any)
        /// </summary>
        public int[] SwappedIndices { get; set; }
        
        /// <summary>
        /// Type of operation being performed
        /// </summary>
        public OperationType OperationType { get; set; }
        
        /// <summary>
        /// Timestamp when the step was created
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Optional description of the step
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Name of the algorithm performing this step
        /// </summary>
        public string AlgorithmName { get; set; }
        
        /// <summary>
        /// Creates a copy of the current state array
        /// </summary>
        public int[] GetStateCopy()
        {
            return (int[])CurrentState.Clone();
        }
    }

    /// <summary>
    /// Type of operation being performed during sorting
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// Initial state
        /// </summary>
        Initial,
        
        /// <summary>
        /// Comparing elements
        /// </summary>
        Comparison,
        
        /// <summary>
        /// Swapping elements
        /// </summary>
        Swap,
        
        /// <summary>
        /// Reading elements
        /// </summary>
        Read,
        
        /// <summary>
        /// Writing elements
        /// </summary>
        Write,
        
        /// <summary>
        /// Final sorted state
        /// </summary>
        Final
    }
}
