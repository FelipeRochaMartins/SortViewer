using System;
using System.Collections.Generic;
using SortViewer.Models;

namespace SortViewer.Algorithms.Base
{
    /// <summary>
    /// Interface for sorting algorithms
    /// </summary>
    public interface ISortAlgorithm
    {
        /// <summary>
        /// Name of the algorithm
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Description of the algorithm
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Executes the sorting algorithm and returns the steps for visualization
        /// </summary>
        /// <param name="data">Data to be sorted</param>
        /// <returns>List of sorting states for visualization</returns>
        List<SortingStep> Sort(int[] data);
        
        /// <summary>
        /// Event fired when a comparison is made
        /// </summary>
        event EventHandler<ComparisonEventArgs> ComparisonPerformed;
        
        /// <summary>
        /// Event fired when a swap is made
        /// </summary>
        event EventHandler<SwapEventArgs> SwapPerformed;
    }
}
