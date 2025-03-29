using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using SortViewer.Algorithms;
using SortViewer.Models;
using SortViewer.Algorithms.Base;

namespace SortViewer.Services
{
    /// <summary>
    /// Service for visualizing sorting algorithms
    /// </summary>
    public class SortingVisualizerService
    {
        private readonly Canvas _canvas;
        private readonly DispatcherTimer _timer;
        private readonly PerformanceMonitorService _performanceMonitorService;
        private int _currentStepIndex;
        private List<SortingStep> _sortingSteps;
        private double _animationSpeed = 500; // ms between frames (default)
        private bool _isRunning;
        private Action<SortingStatistics>? _onStatisticsUpdated;
        private Action<SortingStep>? _onStepExecuted;
        private Action? _onVisualizationCompleted;
        private int _framesToSkip = 0; // Number of frames to skip for ultra-fast animation

        public SortingVisualizerService(Canvas canvas)
        {
            _canvas = canvas;
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _sortingSteps = new List<SortingStep>();
            _performanceMonitorService = new PerformanceMonitorService();
        }

        /// <summary>
        /// Sets the animation speed
        /// </summary>
        /// <param name="speedMs">Time between frames in ms (the smaller, the faster)</param>
        public void SetAnimationSpeed(double speedMs)
        {
            _animationSpeed = speedMs;
            
            // When speed is extremely low (ultra-fast), also enable frame skipping
            if (speedMs < 0.00001)
            {
                // Calculate how many frames to skip based on the speed
                // The lower the speed, the more frames we skip
                _framesToSkip = (int)Math.Max(1, Math.Floor(0.00001 / speedMs));
            }
            else
            {
                _framesToSkip = 0;
            }
            
            if (_isRunning)
            {
                // Ensure we never use less than 1 tick interval (system limitation)
                _timer.Interval = TimeSpan.FromMilliseconds(Math.Max(0.1, speedMs));
            }
        }

        /// <summary>
        /// Sets the callback for statistics updates
        /// </summary>
        public void SetStatisticsCallback(Action<SortingStatistics> callback)
        {
            _onStatisticsUpdated = callback;
        }

        /// <summary>
        /// Sets the callback for when a step is executed
        /// </summary>
        public void SetStepCallback(Action<SortingStep> callback)
        {
            _onStepExecuted = callback;
        }

        /// <summary>
        /// Sets the callback for when the visualization is completed
        /// </summary>
        public void SetCompletionCallback(Action callback)
        {
            _onVisualizationCompleted = callback;
        }

        /// <summary>
        /// Starts visualization with given algorithm and data
        /// </summary>
        public void StartVisualization(ISortAlgorithm algorithm, int[] data)
        {
            // Reset previous state
            _timer.Stop();
            _canvas.Children.Clear();
            _currentStepIndex = 0;
            
            // Reset memory monitoring
            _performanceMonitorService.ResetMemoryMonitoring();
            
            // Generate sorting steps
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    // Use a background thread to sort large datasets
                    if (data.Length > 100)
                    {
                        Task.Run(() => 
                        {
                            _sortingSteps = algorithm.Sort(data);
                            
                            // Return to UI thread to start visualization
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (_sortingSteps != null && _sortingSteps.Count > 0)
                                {
                                    // Draw initial state
                                    DrawStep(_sortingSteps[0]);
                                    
                                    // Start animation
                                    _timer.Interval = TimeSpan.FromMilliseconds(Math.Max(0.1, _animationSpeed));
                                    _timer.Start();
                                    _isRunning = true;
                                }
                            }));
                        });
                    }
                    else
                    {
                        // Small datasets can be processed directly
                        _sortingSteps = algorithm.Sort(data);
                        
                        if (_sortingSteps != null && _sortingSteps.Count > 0)
                        {
                            // Draw initial state
                            DrawStep(_sortingSteps[0]);
                            
                            // Start animation
                            _timer.Interval = TimeSpan.FromMilliseconds(Math.Max(0.1, _animationSpeed));
                            _timer.Start();
                            _isRunning = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during sorting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }));
        }

        /// <summary>
        /// Stops the visualization completely
        /// </summary>
        public void StopVisualization()
        {
            _timer.Stop();
            _isRunning = false;
            _currentStepIndex = 0;
            _canvas.Children.Clear();
        }

        /// <summary>
        /// Pauses the visualization
        /// </summary>
        public void PauseVisualization()
        {
            _timer.Stop();
            _isRunning = false;
        }

        /// <summary>
        /// Resumes the visualization
        /// </summary>
        public void ResumeVisualization()
        {
            if (!_isRunning && _currentStepIndex < _sortingSteps.Count)
            {
                _timer.Start();
                _isRunning = true;
            }
        }

        /// <summary>
        /// Manually advances to the next step
        /// </summary>
        public void NextStep()
        {
            if (_currentStepIndex < _sortingSteps.Count - 1)
            {
                _currentStepIndex++;
                DrawStep(_sortingSteps[_currentStepIndex]);
            }
        }

        /// <summary>
        /// Manually returns to the previous step
        /// </summary>
        public void PreviousStep()
        {
            if (_currentStepIndex > 0)
            {
                _currentStepIndex--;
                DrawStep(_sortingSteps[_currentStepIndex]);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_currentStepIndex < _sortingSteps.Count - 1)
            {
                // Measure memory at each step
                _performanceMonitorService.MeasureCurrentMemory();
                
                // Ultra-fast mode: skip multiple frames at once
                if (_framesToSkip > 0)
                {
                    // Calculate how many steps to move forward
                    int stepsRemaining = _sortingSteps.Count - 1 - _currentStepIndex;
                    int stepsToAdvance = Math.Min(stepsRemaining, _framesToSkip + 1);
                    
                    // Move forward multiple steps
                    _currentStepIndex += stepsToAdvance;
                }
                else
                {
                    // Normal mode: advance one frame at a time
                    _currentStepIndex++;
                }
                
                // Draw the current step
                DrawStep(_sortingSteps[_currentStepIndex]);
            }
            else
            {
                // Visualization completed
                _timer.Stop();
                _isRunning = false;
                _onVisualizationCompleted?.Invoke();
                
                // Calculate final statistics
                var statistics = CalculateStatistics();
                _onStatisticsUpdated?.Invoke(statistics);
            }
        }

        /// <summary>
        /// Draws a specific sorting step
        /// </summary>
        private void DrawStep(SortingStep step)
        {
            _canvas.Children.Clear();
            
            // Calculate metrics for canvas drawing
            double canvasWidth = _canvas.ActualWidth;
            double canvasHeight = _canvas.ActualHeight;
            
            double elementWidth = canvasWidth / step.CurrentState.Length;
            double maxValue = step.CurrentState.Max();
            double scaleFactor = canvasHeight / maxValue;
            
            // Create rectangles for each element
            for (int i = 0; i < step.CurrentState.Length; i++)
            {
                // Calculate height proportional to value
                double height = step.CurrentState[i] * scaleFactor;
                
                // Create rectangle
                var rect = new Rectangle
                {
                    Width = Math.Max(1, elementWidth - 1), // Ensure minimum width of 1px
                    Height = height,
                    Fill = GetBarColor(i, step)
                };
                
                // Add tooltip to show value and index
                rect.ToolTip = new ToolTip { Content = $"Index: {i}, Value: {step.CurrentState[i]}" };
                
                // Position rectangle
                Canvas.SetLeft(rect, i * elementWidth);
                Canvas.SetBottom(rect, 0);
                
                // Add to canvas
                _canvas.Children.Add(rect);
            }
            
            // Update statistics
            _onStatisticsUpdated?.Invoke(CalculateStatistics());
            
            // Notify step execution
            _onStepExecuted?.Invoke(step);
        }

        /// <summary>
        /// Gets the brush color for a bar based on the current operation
        /// </summary>
        private Brush GetBarColor(int index, SortingStep step)
        {
            // Default color
            Brush defaultColor = new SolidColorBrush(Colors.CornflowerBlue);
            
            // Check if the index is being compared
            if (step.OperationType == OperationType.Comparison && 
                step.ComparedIndices != null && Array.IndexOf(step.ComparedIndices, index) >= 0)
            {
                return Brushes.Gold;
            }
            
            // Check if the index is being swapped
            if (step.OperationType == OperationType.Swap && 
                step.SwappedIndices != null && Array.IndexOf(step.SwappedIndices, index) >= 0)
            {
                return Brushes.Red;
            }
            
            // Check for Read operation (specific colors for read elements)
            if (step.OperationType == OperationType.Read && 
                step.ComparedIndices != null && Array.IndexOf(step.ComparedIndices, index) >= 0)
            {
                return Brushes.Purple;
            }
            
            // Check for Write operation (specific colors for write elements)
            if (step.OperationType == OperationType.Write && 
                step.ComparedIndices != null && Array.IndexOf(step.ComparedIndices, index) >= 0)
            {
                return Brushes.Orange;
            }
            
            // If it's the final state, all bars are green
            if (step.OperationType == OperationType.Final)
            {
                return Brushes.Green;
            }
            
            return defaultColor;
        }

        /// <summary>
        /// Calculates statistics for the current visualization
        /// </summary>
        private SortingStatistics CalculateStatistics()
        {
            if (_sortingSteps == null || _sortingSteps.Count == 0)
            {
                return new SortingStatistics();
            }

            // Create a subset of steps up to the current point
            var currentSteps = _sortingSteps.Take(_currentStepIndex + 1).ToList();
            
            // Get the algorithm name from the first step (if available)
            string algorithmName = "Unknown";
            if (_sortingSteps.Count > 0 && _sortingSteps[0].AlgorithmName != null)
            {
                algorithmName = _sortingSteps[0].AlgorithmName;
            }
            
            // Get data size from the first step
            int dataSize = _sortingSteps.Count > 0 ? _sortingSteps[0].CurrentState.Length : 0;
            
            // Final memory measurement
            _performanceMonitorService.MeasureCurrentMemory();
            
            // Use performance monitor service to calculate statistics
            return _performanceMonitorService.CalculateStatistics(currentSteps, algorithmName, dataSize);
        }
    }
}
