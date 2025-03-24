using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SortViewer.Models;

namespace SortViewer.Services
{
    /// <summary>
    /// Service for monitoring sort algorithm performance
    /// </summary>
    public class PerformanceMonitorService
    {
        private long _initialMemory;
        private long _peakMemoryUsage;
        
        public PerformanceMonitorService()
        {
            // Force garbage collection to get a clean baseline
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            // Get initial memory
            _initialMemory = GC.GetTotalMemory(true);
            _peakMemoryUsage = 0;
        }
        
        /// <summary>
        /// Records current memory usage and updates peak if necessary
        /// </summary>
        /// <returns>Current memory usage in bytes</returns>
        public long MeasureCurrentMemory()
        {
            // Get current memory without forcing collection
            long currentMemory = GC.GetTotalMemory(false);
            
            // Calculate relative memory usage compared to baseline
            long currentUsage = Math.Max(0, currentMemory - _initialMemory);
            
            // Update peak if necessary
            if (currentUsage > _peakMemoryUsage)
            {
                _peakMemoryUsage = currentUsage;
            }
            
            return currentUsage;
        }
        
        /// <summary>
        /// Resets the memory monitoring
        /// </summary>
        public void ResetMemoryMonitoring()
        {
            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            // Reset initial memory
            _initialMemory = GC.GetTotalMemory(true);
            _peakMemoryUsage = 0;
        }
        
        /// <summary>
        /// Calculates comprehensive statistics for a sorting process
        /// </summary>
        /// <param name="steps">List of sorting steps to analyze</param>
        /// <param name="algorithmName">Name of the sorting algorithm</param>
        /// <param name="dataSize">Size of the input data</param>
        /// <returns>Sorting statistics</returns>
        public SortingStatistics CalculateStatistics(List<SortingStep> steps, string algorithmName, int dataSize)
        {
            if (steps == null || steps.Count == 0)
                return new SortingStatistics();
                
            int comparisons = 0;
            int swaps = 0;
            
            foreach (var step in steps)
            {
                if (step.OperationType == OperationType.Comparison)
                {
                    comparisons++;
                }
                else if (step.OperationType == OperationType.Swap)
                {
                    swaps++;
                }
            }
            
            // Calculate execution time
            TimeSpan executionTime = TimeSpan.Zero;
            if (steps.Count > 0)
            {
                executionTime = steps.Last().Timestamp - steps.First().Timestamp;
            }
            
            // Use peak memory usage rather than current
            var statistics = new SortingStatistics
            {
                TotalSteps = steps.Count,
                Comparisons = comparisons,
                Swaps = swaps,
                ExecutionTimeMs = executionTime.TotalMilliseconds,
                MemoryUsageBytes = _peakMemoryUsage
            };
            
            return statistics;
        }
    }

    /// <summary>
    /// Performance statistics of a sorting algorithm
    /// </summary>
    public class SortingStatistics
    {
        /// <summary>
        /// Total number of steps
        /// </summary>
        public int TotalSteps { get; set; }
        
        /// <summary>
        /// Total number of comparisons
        /// </summary>
        public int Comparisons { get; set; }
        
        /// <summary>
        /// Total number of swaps
        /// </summary>
        public int Swaps { get; set; }
        
        /// <summary>
        /// Execution time in milliseconds
        /// </summary>
        public double ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// Approximate memory usage in bytes
        /// </summary>
        public long MemoryUsageBytes { get; set; }
    }
}
