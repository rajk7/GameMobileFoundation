﻿using System;
using System.Collections.Generic;

namespace Pancake.Linq
{
    public static partial class L
    {
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>        
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A sequence that contains the specified number of elements from the start of the input sequence.</returns>
        public static T[] Take<T>(this T[] source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (count < 0)
            {
                count = 0;
            }
            else if (count > source.Length)
            {
                count = source.Length;
            }

            var result = new T[count];
            Array.Copy(source,
                0,
                result,
                0,
                count);
            return result;
        }

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true.
        /// </summary>        
        /// <param name="source">A sequence to return elements from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A sequence that contains the elements from the input sequence that occur before the element at which the test no longer passes.</returns>
        public static T[] TakeWhile<T>(this T[] source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int count = 0;
            for (; count < source.Length; count++)
            {
                if (!predicate(source[count]))
                    break;
            }

            var result = new T[count];
            Array.Copy(source,
                0,
                result,
                0,
                count);
            return result;
        }

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.
        /// </summary>        
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence that contains elements from the input sequence that occur before the element at which the test no longer passes.</returns>
        public static T[] TakeWhile<T>(this T[] source, Func<T, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int count = 0;
            for (; count < source.Length; count++)
            {
                if (!predicate(source[count], count))
                    break;
            }

            var result = new T[count];
            Array.Copy(source,
                0,
                result,
                0,
                count);
            return result;
        }

        /*---- spans ---- */
#if UNITY_2021_3_OR_NEWER
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>        
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A sequence that contains the specified number of elements from the start of the input sequence.</returns>
        public static T[] Take<T>(this Span<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (count < 0)
            {
                count = 0;
            }
            else if (count > source.Length)
            {
                count = source.Length;
            }

            var result = new T[count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = source[i];
            }

            return result;
        }


        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true.
        /// </summary>        
        /// <param name="source">A sequence to return elements from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A sequence that contains the elements from the input sequence that occur before the element at which the test no longer passes.</returns>
        public static T[] TakeWhile<T>(this Span<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int count = 0;
            for (; count < source.Length; count++)
            {
                if (!predicate(source[count]))
                    break;
            }

            var result = new T[count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = source[i];
            }

            return result;
        }

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.
        /// </summary>        
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence that contains elements from the input sequence that occur before the element at which the test no longer passes.</returns>
        public static T[] TakeWhile<T>(this Span<T> source, Func<T, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int count = 0;
            for (; count < source.Length; count++)
            {
                if (!predicate(source[count], count))
                    break;
            }

            var result = new T[count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = source[i];
            }

            return result;
        }
#endif


        // ------------- Lists ----------------

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>        
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A sequence that contains the specified number of elements from the start of the input sequence.</returns>
        public static List<T> Take<T>(this List<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (count < 0)
            {
                count = 0;
            }
            else if (count > source.Count)
            {
                count = source.Count;
            }

            var result = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true.
        /// </summary>        
        /// <param name="source">A sequence to return elements from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A sequence that contains the elements from the input sequence that occur before the element at which the test no longer passes.</returns>
        public static List<T> TakeWhile<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            for (int i = 0; i < source.Count; i++)
            {
                if (predicate(source[i]))
                {
                    result.Add(source[i]);
                }
                else
                {
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.
        /// </summary>        
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence that contains elements from the input sequence that occur before the element at which the test no longer passes.</returns>
        public static List<T> TakeWhile<T>(this List<T> source, Func<T, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            for (int i = 0; i < source.Count; i++)
            {
                if (predicate(source[i], i))
                    result.Add(source[i]);
                else
                    return result;
            }

            return result;
        }
    }
}