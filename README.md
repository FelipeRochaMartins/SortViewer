# SortViewer: Sorting Algorithm Visualization Tool

SortViewer is an interactive WPF application designed to visualize sorting algorithms and their step-by-step execution. The application provides a clean and intuitive interface that allows users to observe how different sorting algorithms arrange data, compare their performance metrics, and understand their mechanics.

## Features

- **Multiple Sorting Algorithms**: Visualize different sorting algorithms (currently BubbleSort and QuickSort)
- **Data Types**: Test algorithms on various data distributions (Random, Nearly Sorted, Reversed, Many Duplicates)
- **Custom Array Size**: Manually input array sizes up to 10,000 elements
- **Adjustable Animation Speed**: Control visualization speed from very slow to ultra-fast
- **Step Controls**: Forward, backward, pause, and stop controls for detailed analysis
- **Performance Statistics**: Real-time statistics on comparisons, swaps, execution time, and memory usage

## Application Architecture

SortViewer follows the MVVM (Model-View-ViewModel) architecture pattern, with a clear separation of concerns:

### Model Layer

The model layer represents the core data structures and business logic:

#### Core Models:

- **SortingStep.cs**: Represents a single step during the sorting process
  - Contains the current state of the array
  - Tracks compared and swapped indices
  - Stores the operation type (comparison, swap, read, or final)
  - Records timestamp for performance metrics

- **OperationType.cs**: Enum defining possible operations during sorting
  - `Comparison`: Comparing two elements
  - `Swap`: Swapping two elements
  - `Read`: Reading an element
  - `Final`: Final sorted state

- **SortingStatistics.cs**: Contains performance metrics
  - Total steps
  - Number of comparisons
  - Number of swaps
  - Execution time
  - Memory usage in bytes

### Algorithms

The algorithms are implemented as separate classes, all inheriting from a common base class:

- **BaseSortAlgorithm.cs**: Abstract base class for all sorting algorithms
  - Provides common utility methods like CreateStep and Swap
  - Defines the interface for concrete algorithm implementations

- **BubbleSort.cs**: Implementation of the Bubble Sort algorithm
  - Creates steps for visualization during the sorting process
  - Tracks all comparisons and swaps

- **QuickSort.cs**: Implementation of the Quick Sort algorithm
  - Uses median-of-three pivot selection for better performance
  - Creates detailed steps showing partitioning and recursion
  - Enhanced to work well with all data types including nearly sorted and reversed arrays

### Services

Services handle specific functionality domains:

- **SortingVisualizerService.cs**: Core visualization service
  - Controls the animation timer
  - Draws each step of the sorting process
  - Manages playback controls (forward, backward, pause, stop)
  - Processes large arrays in background threads to avoid UI freezing
  - Uses intelligent frame-skipping for ultra-fast speeds

- **DataGeneratorService.cs**: Generates data arrays for sorting
  - Random arrays
  - Nearly sorted arrays (sorted with some random swaps)
  - Reversed arrays (descending order)
  - Arrays with many duplicates

- **PerformanceMonitorService.cs**: Tracks and calculates performance metrics
  - Monitors execution time
  - Counts operations (comparisons and swaps)
  - Tracks memory usage during algorithm execution

### View and ViewModel

- **MainWindow.xaml/xaml.cs**: The main UI
  - Controls for algorithm selection, array size input, and data type selection
  - Visualization canvas that displays the current state
  - Playback controls for stepping through the visualization
  - Statistics display showing performance metrics

## Component Interactions

### 1. Initialization Flow

1. The application starts in `App.xaml.cs` which loads `MainWindow.xaml`
2. `MainWindow` constructor initializes services and populates the list of algorithms
3. `Window_Loaded` event connects the visualizer service to the canvas and sets up callbacks

### 2. Data Generation Flow

1. User selects data type and inputs array size
2. `GenerateData()` validates input and calls appropriate method on `DataGeneratorService`
3. The generated array is stored in `_currentData` and displayed on the canvas

### 3. Sorting Visualization Flow

1. User clicks "Start" button which calls `StartSorting()`
2. Selected algorithm's `Sort()` method is called to generate steps
3. `SortingVisualizerService.StartVisualization()` begins the animation:
   - For large arrays (>1000 elements), sorting happens in a background thread
   - Smaller arrays are processed directly on the UI thread
4. Timer ticks advance through sorting steps at specified intervals
5. Each step is visualized by `DrawStep()` which:
   - Clears the canvas
   - Draws bars representing array values
   - Colors bars based on the current operation
   - Updates statistics

### 4. Control Mechanisms

- **Speed Control**: Slider value is mapped logarithmically to animation delay
- **Step Controls**: 
  - Next/Previous: Move one step forward/backward and pause
  - Pause/Resume: Toggle animation playback
  - Stop: Completely halt and reset visualization

## Implementation Details

### Key Design Patterns

1. **Strategy Pattern**: Different sort algorithms implement the same interface
2. **Observer Pattern**: Callbacks for statistics updates and visualization completion
3. **Command Pattern**: Each sorting step represents a command in the visualization sequence

### Performance Optimizations

1. **Background Threading**: Large arrays are processed in background threads
2. **Frame Skipping**: Ultra-fast speeds skip frames instead of trying to render every step
3. **Efficient Drawing**: Canvas drawing is optimized to handle large arrays
4. **QuickSort Improvements**: Median-of-three pivot selection to avoid worst-case scenarios

### UI Performance

- Non-blocking UI through asynchronous operations
- Smart handling of extreme speeds
- Efficient canvas rendering

## How to Use

1. **Generate Data**:
   - Enter desired array size (1-10,000)
   - Select data type (Random, Nearly Sorted, etc.)
   - Click "Generate Data"

2. **Start Visualization**:
   - Select algorithm from dropdown
   - Click "Start"
   - Use speed slider to adjust visualization speed

3. **Control Playback**:
   - Use Previous/Next buttons for step-by-step analysis
   - Pause/Resume to control continuous playback
   - Stop to reset the visualization

4. **Analyze Performance**:
   - View statistics panel for metrics
   - Compare different algorithms' performance

## Technical Requirements

- .NET Framework 4.7.2 or higher
- Windows 7 or higher
- WPF support

## Future Improvements

- Additional sorting algorithms (MergeSort, HeapSort, etc.)
- Customizable colors and visualization styles
- Sound effects for operations
- Export performance data for analysis
- Comparison mode to visualize two algorithms side by side

---

## Technical Documentation

### Model Classes

#### SortingStep.cs
```csharp
public class SortingStep
{
    public int[] CurrentState { get; set; }      // Current array state
    public int[] ComparedIndices { get; set; }   // Indices being compared
    public int[] SwappedIndices { get; set; }    // Indices being swapped
    public OperationType OperationType { get; set; }  // Type of operation
    public DateTime Timestamp { get; set; }      // When step occurred
    public string Description { get; set; }      // Human-readable description
}
```

#### SortingStatistics.cs
```csharp
public class SortingStatistics
{
    public int TotalSteps { get; set; }          // Total steps in sorting
    public int Comparisons { get; set; }         // Number of comparisons
    public int Swaps { get; set; }               // Number of swaps
    public double ExecutionTimeMs { get; set; }  // Time taken in milliseconds
    public long MemoryUsageBytes { get; set; }   // Memory usage in bytes
}
```

### Algorithm Implementation

Each sorting algorithm must implement:

```csharp
public abstract class BaseSortAlgorithm : ISortAlgorithm
{
    // Common properties
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    // Main sorting method - generates visualization steps
    public abstract List<SortingStep> Sort(int[] data);
    
    // Utility methods
    protected SortingStep CreateStep(int[] array, OperationType operationType, 
        int[] activeIndices, int[] rangeIndices, string description);
    protected void Swap(int[] array, int i, int j);
}
```

### Optimization Techniques

1. **QuickSort Pivot Selection**:
   - Uses median-of-three method to select pivot
   - Reduces worst-case behavior for nearly-sorted or reversed arrays

```csharp
// Median-of-three pivot selection
int mid = low + (high - low) / 2;
if (array[mid] < array[low])
    Swap(array, low, mid);
if (array[high] < array[low])
    Swap(array, low, high);
if (array[mid] < array[high])
    Swap(array, mid, high);
```

2. **Background Threading**:
   - Large arrays processed in background threads
   - UI updates happen on the dispatcher thread

```csharp
if (data.Length > 100)
{
    Task.Run(() => 
    {
        _sortingSteps = algorithm.Sort(data);
        
        // Return to UI thread
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            // Start animation on UI thread
        }));
    });
}
```

3. **Frame Skipping**:
   - At extreme speeds, frames are intelligently skipped
   - Ensures smooth animation without trying to render every step

```csharp
if (_framesToSkip > 0)
{
    int stepsRemaining = _sortingSteps.Count - 1 - _currentStepIndex;
    int stepsToAdvance = Math.Min(stepsRemaining, _framesToSkip + 1);
    _currentStepIndex += stepsToAdvance;
}
```

4. **Logarithmic Speed Scaling**:
   - Speed slider maps exponentially to delay values
   - Provides fine control at both slow and fast speeds

```csharp
double sliderPercentage = e.NewValue / 100000000;
double speed = 1000 * Math.Pow(0.0000001/1000, sliderPercentage);
```