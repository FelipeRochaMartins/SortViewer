using System;
using System.Windows;
using System.Windows.Controls;
using SortViewer.Models;

namespace SortViewer
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
            
            // Enable alternating item background
            LogListBox.AlternationCount = 2;
        }

        public void AppendLog(string message)
        {
            // Ensure UI updates happen on the UI thread
            Dispatcher.Invoke(() =>
            {
                LogListBox.Items.Add(message);
                
                // Scroll to the last item
                if (LogListBox.Items.Count > 0)
                {
                    LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
                }
            });
        }

        public void LogSortingStep(SortingStep step)
        {
            // Format the timestamp
            string timestamp = $"[{DateTime.Now.ToString("HH:mm:ss.fff")}]";
            
            // Format the operation type
            string opType = string.Empty;
            switch (step.OperationType)
            {
                case OperationType.Initial:
                    opType = "[INITIAL]";
                    break;
                case OperationType.Comparison:
                    opType = "[COMPARE]";
                    break;
                case OperationType.Swap:
                    opType = "[SWAP]";
                    break;
                case OperationType.Read:
                    opType = "[READ]";
                    break;
                case OperationType.Write:
                    opType = "[WRITE]";
                    break;
                case OperationType.Final:
                    opType = "[FINAL]";
                    break;
            }
            
            // Combine the elements with the timestamp first, then operation type, then description
            string message = $"{timestamp} {opType} {step.Description}";
            
            AppendLog(message);
        }

        public void ClearLog()
        {
            LogListBox.Items.Clear();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            
            // Notify the main window that this window is now hidden
            if (Owner is MainWindow mainWindow)
            {
                mainWindow.OnLogWindowClosed();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Hide instead of close, so the window can be reused
            e.Cancel = true;
            Hide();
            
            // Notify the main window that this window is now hidden
            if (Owner is MainWindow mainWindow)
            {
                mainWindow.OnLogWindowClosed();
            }
        }
    }
}
