using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Text.RegularExpressions;
using SortViewer.Algorithms;
using SortViewer.Services;
using SortViewer.Algorithms.Base;

namespace SortViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<ISortAlgorithm> _algorithms;
        private readonly DataGeneratorService _dataGeneratorService;
        private readonly PerformanceMonitorService _performanceMonitorService;
        private SortingVisualizerService _visualizerService = null!;
        
        private int[] _currentData = Array.Empty<int>();
        private bool _isPaused;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize services
            _dataGeneratorService = new DataGeneratorService();
            _performanceMonitorService = new PerformanceMonitorService();
            
            // Initialize the algorithm list
            _algorithms = new List<ISortAlgorithm>
            {
                new BubbleSort(),
                new QuickSort()
                // Add other algorithms here
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Configure the visualizer
            _visualizerService = new SortingVisualizerService(VisualizationCanvas);
            _visualizerService.SetStatisticsCallback(UpdateStatistics);
            _visualizerService.SetCompletionCallback(() => 
            {
                StatusTextBlock.Text = "Completed";
                EnableControls(true);
            });
            
            // Fill the algorithm ComboBox
            foreach (var algorithm in _algorithms)
            {
                AlgorithmComboBox.Items.Add(algorithm.Name);
            }
            
            if (AlgorithmComboBox.Items.Count > 0)
            {
                AlgorithmComboBox.SelectedIndex = 0;
            }
            
            // Generate initial data
            GenerateData();
            
            // Configure navigation controls
            EnableNavigationControls(false);
        }

        private void GenerateData()
        {
            if (AlgorithmComboBox.SelectedIndex < 0 || ArraySizeTextBox.Text == string.Empty)
                return;

            // Get array size from TextBox
            if (!int.TryParse(ArraySizeTextBox.Text, out int arraySize) || arraySize <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for array size.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Limit array size to a reasonable maximum to prevent performance issues
            if (arraySize > 10000)
            {
                MessageBox.Show("Array size capped at 10000 to maintain performance.", "Size Adjusted", MessageBoxButton.OK, MessageBoxImage.Information);
                arraySize = 10000;
                ArraySizeTextBox.Text = "10000";
            }
            
            // Make sure a data type is selected
            if (DataTypeComboBox.SelectedItem == null)
            {
                DataTypeComboBox.SelectedIndex = 0; // Default to first item
            }
            
            // Generate data based on selected type
            string dataType = "Random"; // Default value
            if (DataTypeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content is string content)
            {
                dataType = content;
            }
            
            try
            {
                switch (dataType)
                {
                    case "Random":
                        _currentData = _dataGeneratorService.GenerateRandomArray(arraySize);
                        break;
                    case "Nearly Sorted":
                        _currentData = _dataGeneratorService.GenerateNearlySortedArray(arraySize, arraySize / 10);
                        break;
                    case "Reversed":
                        _currentData = _dataGeneratorService.GenerateReversedArray(arraySize);
                        break;
                    case "Many Duplicates":
                        _currentData = _dataGeneratorService.GenerateArrayWithDuplicates(arraySize, arraySize / 5);
                        break;
                    default:
                        _currentData = _dataGeneratorService.GenerateRandomArray(arraySize);
                        break;
                }
                
                // Log data generation
                Console.WriteLine($"Generated {dataType} data with {arraySize} elements");
                
                // Visualize initial state
                DrawInitialState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Data generation error: {ex}");
            }
        }

        private void DrawInitialState()
        {
            if (_currentData == null || _currentData.Length == 0)
                return;
                
            VisualizationCanvas.Children.Clear();
            
            // Determine maximum value for scaling
            int maxValue = _currentData.Max();
            
            // Debugging information
            Console.WriteLine($"Canvas dimensions: {VisualizationCanvas.ActualWidth} x {VisualizationCanvas.ActualHeight}");
            Console.WriteLine($"Data points: {_currentData.Length}, Max value: {maxValue}");
            
            // Make sure canvas has dimensions
            if (VisualizationCanvas.ActualWidth <= 0 || VisualizationCanvas.ActualHeight <= 0)
            {
                // Force layout update to get actual dimensions
                VisualizationCanvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                VisualizationCanvas.Arrange(new Rect(VisualizationCanvas.DesiredSize));
                
                // If still no dimensions, use default values
                if (VisualizationCanvas.ActualWidth <= 0) VisualizationCanvas.Width = 800;
                if (VisualizationCanvas.ActualHeight <= 0) VisualizationCanvas.Height = 400;
            }
            
            // Calculate bar width
            double barWidth = VisualizationCanvas.ActualWidth / _currentData.Length;
            
            // Scale factor for height (ensure it's not zero)
            double heightScale = maxValue > 0 ? 
                (VisualizationCanvas.ActualHeight * 0.9) / maxValue : 
                VisualizationCanvas.ActualHeight / 100;
            
            // Draw each bar
            for (int i = 0; i < _currentData.Length; i++)
            {
                Rectangle bar = new Rectangle();
                
                // Set bar size
                double height = _currentData[i] * heightScale;
                bar.Width = Math.Max(1, barWidth - 1);
                bar.Height = Math.Max(1, height); // Ensure minimum height of 1
                
                // Set color
                bar.Fill = Brushes.CornflowerBlue;
                
                // Position the bar
                Canvas.SetLeft(bar, i * barWidth);
                Canvas.SetTop(bar, VisualizationCanvas.ActualHeight - bar.Height);
                
                // Add to Canvas
                VisualizationCanvas.Children.Add(bar);
            }
            
            // Reset statistics
            ResetStatistics();
            
            // Update status
            StatusTextBlock.Text = $"Generated {_currentData.Length} data points";
        }

        private void StartSorting()
        {
            if (_currentData == null || AlgorithmComboBox.SelectedIndex < 0)
                return;
                
            // Get selected algorithm
            ISortAlgorithm algorithm = _algorithms[AlgorithmComboBox.SelectedIndex];
            
            // Update status
            StatusTextBlock.Text = $"Running {algorithm.Name}...";
            
            // Start visualization
            _visualizerService.StartVisualization(algorithm, _currentData);
            
            // Configure controls
            EnableControls(false);
            EnableNavigationControls(true);
            _isPaused = false;
            PauseResumeButton.Content = "⏸ Pause";
        }

        private void UpdateStatistics(SortingStatistics statistics)
        {
            ComparisonsTextBlock.Text = statistics.Comparisons.ToString();
            SwapsTextBlock.Text = statistics.Swaps.ToString();
            TimeTextBlock.Text = $"{statistics.ExecutionTimeMs:F2} ms";
            
            // Format memory usage (convert from bytes to appropriate unit)
            string memoryText;
            if (statistics.MemoryUsageBytes < 1024)
            {
                memoryText = $"{statistics.MemoryUsageBytes} B";
            }
            else if (statistics.MemoryUsageBytes < 1024 * 1024)
            {
                memoryText = $"{statistics.MemoryUsageBytes / 1024.0:F2} KB";
            }
            else
            {
                memoryText = $"{statistics.MemoryUsageBytes / (1024.0 * 1024.0):F2} MB";
            }
            
            MemoryTextBlock.Text = memoryText;
        }

        private void ResetStatistics()
        {
            ComparisonsTextBlock.Text = "0";
            SwapsTextBlock.Text = "0";
            TimeTextBlock.Text = "0 ms";
            MemoryTextBlock.Text = "0 KB";
            StatusTextBlock.Text = "Ready";
        }

        private void EnableControls(bool enable)
        {
            AlgorithmComboBox.IsEnabled = enable;
            ArraySizeTextBox.IsEnabled = enable;
            DataTypeComboBox.IsEnabled = enable;
            GenerateButton.IsEnabled = enable;
            StartButton.IsEnabled = enable;
        }

        private void EnableNavigationControls(bool enable)
        {
            PreviousStepButton.IsEnabled = enable;
            PauseResumeButton.IsEnabled = enable;
            NextStepButton.IsEnabled = enable;
        }

        #region Event Handlers
        
        private void AlgorithmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Can implement algorithm-specific logic if needed
        }

        private void ArraySizeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            NumberValidationTextBox(sender, e);
        }

        private void DataTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Only generate data if the Window is fully loaded and control is enabled
            if (IsLoaded && DataTypeComboBox.IsEnabled)
            {
                GenerateData();
            }
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_visualizerService != null)
            {
                // Map slider value to speed - higher slider values = faster speed (smaller interval)
                // Use an exponential scale for more extreme speeds
                double sliderPercentage = e.NewValue / 100000000;
                
                // Ultra-fast exponential scale: from 1000ms down to 0.0000001ms (100 nanoseconds)
                // When slider is at minimum (0%), use 1000ms
                // When slider is at maximum (100%), use 0.0000001ms
                double speed = 1000 * Math.Pow(0.0000001/1000, sliderPercentage);
                
                _visualizerService.SetAnimationSpeed(speed);
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateData();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartSorting();
        }

        private void PreviousStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (_visualizerService != null)
            {
                _visualizerService.PauseVisualization();
                _visualizerService.PreviousStep();
                _isPaused = true;
                PauseResumeButton.Content = "▶ Resume";
            }
        }

        private void PauseResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_visualizerService != null)
            {
                if (_isPaused)
                {
                    _visualizerService.ResumeVisualization();
                    PauseResumeButton.Content = "⏸ Pause";
                }
                else
                {
                    _visualizerService.PauseVisualization();
                    PauseResumeButton.Content = "▶ Resume";
                }
                
                _isPaused = !_isPaused;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_visualizerService != null)
            {
                _visualizerService.StopVisualization();
                EnableControls(true);
                StatusTextBlock.Text = "Stopped";
                _isPaused = false;
                PauseResumeButton.Content = "⏸ Pause";
                DrawInitialState();
            }
        }

        private void NextStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (_visualizerService != null)
            {
                _visualizerService.PauseVisualization();
                _visualizerService.NextStep();
                _isPaused = true;
                PauseResumeButton.Content = "▶ Resume";
            }
        }
        
        #endregion

        /// <summary>
        /// Validates that only numbers can be entered in the TextBox
        /// </summary>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}