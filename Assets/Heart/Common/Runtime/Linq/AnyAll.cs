using System;
using System.Collections.Generic;

namespace Pancake.Linq
{
    public static partial class L
    {
        // --------------------------  ARRAYS --------------------------------------------

        /// <summary>
        /// Determines whether an array contains any elements
        /// </summary>        
        /// <param name="source">The array to check for emptiness</param>
        /// <returns>true if the source array contains any elements, otherwise, false/</returns>
        public static bool Any<T>(this T[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Length > 0;
        }


        /// <summary>
        /// Determines whether any element of an array satisfies a condition.
        /// </summary>        
        /// <param name="source">An array whose elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if any elements in the source array pass the test in the specified predicate; otherwise, false.</returns>
        public static bool Any<TSource>(this TSource[] source, Predicate<TSource> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return Array.Exists(source, predicate);
        }


        /// <summary>
        /// Determines whether all elements of an array satisfy a condition.
        /// </summary>        
        /// <param name="source">An array that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if every element of the source array passes the test in the specified
        /// predicate, or if the array is empty; otherwise, false</returns>
        public static bool AllF<TSource>(this TSource[] source, Predicate<TSource> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return Array.TrueForAll(source, predicate);
        }

        // --------------------------  this SpanS --------------------------------------------

#if UNITY_2021_3_OR_NEWER
        /// <summary>
        /// Determines whether an array contains any elements
        /// </summary>        
        /// <param name="source">The array to check for emptiness</param>
        /// <returns>true if the source array contains any elements, otherwise, false/</returns>
        public static bool Any<T>(this Span<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Length > 0;
        }


        /// <summary>
        /// Determines whether any element of an array satisfies a condition.
        /// </summary>        
        /// <param name="source">An array whose elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if any elements in the source array pass the test in the specified predicate; otherwise, false.</returns>
        public static bool Any<TSource>(this Span<TSource> source, Predicate<TSource> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (int i = 0; i < source.Length; i++)
            {
                if (predicate(source[i])) return true;
            }

            return false;
        }


        /// <summary>
        /// Determines whether all elements of an array satisfy a condition.
        /// </summary>        
        /// <param name="source">An array that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if every element of the source array passes the test in the specified
        /// predicate, or if the array is empty; otherwise, false</returns>
        public static bool AllF<TSource>(this Span<TSource> source, Predicate<TSource> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (int i = 0; i < source.Length; i++)
            {
                if (!predicate(source[i])) return false;
            }

            return true;
        }
#endif


        // --------------------------  Lists --------------------------------------------
        /// <summary>
        /// Determines whether a list contains any elements
        /// </summary>        
        /// <param name="source">The list to check for emptiness</param>
        /// <returns>true if the source list contains any elements, otherwise, false/</returns>
        public static bool Any<T>(this List<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Count > 0;
        }

        /// <summary>
        /// Determines whether any element of an array satisfies a condition.
        /// </summary>        
        /// <param name="source">An array whose elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if any elements in the source array pass the test in the specified predicate; otherwise, false.</returns>
        public static bool Any<TSource>(this List<TSource> source, Predicate<TSource> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return source.Exists(predicate);
        }

        /// <summary>
        /// Determines whether all elements of a list satisfy a condition.
        /// </summary>        
        /// <param name="source">A list that contains the elements to apply the predicate to.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if every element of the source array passes the test in the specified
        /// predicate, or if the list is empty; otherwise, false</returns>
        public static bool AllF<TSource>(this List<TSource> source, Predicate<TSource> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return source.TrueForAll(predicate);
        }
    }
}