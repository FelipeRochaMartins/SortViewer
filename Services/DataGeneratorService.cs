using System;
using System.Linq;

namespace SortViewer.Services
{
    /// <summary>
    /// Service for generating data for sorting algorithm tests
    /// </summary>
    public class DataGeneratorService
    {
        private readonly Random _random;

        public DataGeneratorService()
        {
            _random = new Random();
        }

        /// <summary>
        /// Generates an array of random integers
        /// </summary>
        /// <param name="size">Size of the array</param>
        /// <param name="minValue">Minimum value (inclusive)</param>
        /// <param name="maxValue">Maximum value (exclusive)</param>
        /// <returns>Array of random integers</returns>
        public int[] GenerateRandomArray(int size, int minValue = 1, int maxValue = 100)
        {
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = _random.Next(minValue, maxValue);
            }
            return array;
        }

        /// <summary>
        /// Generates a nearly sorted array (with some swaps)
        /// </summary>
        /// <param name="size">Size of the array</param>
        /// <param name="swapCount">Number of swaps to be made</param>
        /// <returns>Nearly sorted array</returns>
        public int[] GenerateNearlySortedArray(int size, int swapCount)
        {
            // Creates a sorted array
            int[] array = Enumerable.Range(1, size).ToArray();
            
            // Makes some random swaps
            for (int i = 0; i < swapCount; i++)
            {
                int index1 = _random.Next(0, size);
                int index2 = _random.Next(0, size);
                
                // Swap elements
                int temp = array[index1];
                array[index1] = array[index2];
                array[index2] = temp;
            }
            
            return array;
        }

        /// <summary>
        /// Generates a reversed array (from largest to smallest)
        /// </summary>
        /// <param name="size">Size of the array</param>
        /// <returns>Array in reverse order</returns>
        public int[] GenerateReversedArray(int size)
        {
            return Enumerable.Range(1, size).Reverse().ToArray();
        }

        /// <summary>
        /// Generates an array with many duplicate values
        /// </summary>
        /// <param name="size">Size of the array</param>
        /// <param name="uniqueValuesCount">Number of unique values</param>
        /// <returns>Array with duplicate values</returns>
        public int[] GenerateArrayWithDuplicates(int size, int uniqueValuesCount)
        {
            int[] array = new int[size];
            
            for (int i = 0; i < size; i++)
            {
                array[i] = _random.Next(1, uniqueValuesCount + 1);
            }
            
            return array;
        }
    }
}
