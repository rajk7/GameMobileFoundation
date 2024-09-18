﻿using System;
using System.Collections.Generic;

namespace Pancake.Linq
{
    //int, long, float,double, decimal
    public static partial class L
    {
        // --------------------------  ARRAYS  --------------------------------------------

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static T Max<T>(this T[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            Comparer<T> comparer = Comparer<T>.Default;
            T r = default(T);
            if (r == null)
            {
                r = source[0];
                for (int i = 1; i < source.Length; i++)
                {
                    if (source[i] != null && comparer.Compare(source[i], r) > 0) r = source[i];
                }
            }
            else
            {
                r = source[0];
                for (int i = 1; i < source.Length; i++)
                {
                    if (comparer.Compare(source[i], r) > 0) r = source[i];
                }
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static TResult Max<T, TResult>(this T[] source, Func<T, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            Comparer<TResult> comparer = Comparer<TResult>.Default;
            TResult r = default(TResult);
            if (r == null)
            {
                r = selector(source[0]);
                for (int i = 1; i < source.Length; i++)
                {
                    var v = selector(source[i]);
                    if (v != null && comparer.Compare(v, r) > 0) r = v;
                }
            }
            else
            {
                r = selector(source[0]);
                for (int i = 1; i < source.Length; i++)
                {
                    var v = selector(source[i]);
                    if (comparer.Compare(v, r) > 0) r = v;
                }
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static int Max(this int[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            int r = int.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static int Max<T>(this T[] source, Func<T, int> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            int r = int.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static long Max(this long[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            long r = long.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }


        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static long Max<T>(this T[] source, Func<T, long> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            long r = long.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static float Max(this float[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            float r = source[0];
            int startIndex = 0;
            for (; startIndex < source.Length; startIndex++)
            {
                if (!float.IsNaN(source[startIndex]))
                {
                    r = source[startIndex];
                    break;
                }
            }

            for (int i = startIndex; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static float Max<T>(this T[] source, Func<T, float> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            float r = selector(source[0]);
            int startIndex = 0;
            for (; startIndex < source.Length; startIndex++)
            {
                var v = selector(source[startIndex]);
                if (!float.IsNaN(v))
                {
                    r = v;
                    break;
                }
            }

            for (int i = startIndex; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static double Max(this double[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            double r = source[0];
            int startIndex = 0;
            for (; startIndex < source.Length; startIndex++)
            {
                if (!double.IsNaN(source[startIndex]))
                {
                    r = source[startIndex];
                    break;
                }
            }

            for (int i = startIndex; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static double Max<T>(this T[] source, Func<T, double> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            double r = selector(source[0]);
            int startIndex = 0;
            for (; startIndex < source.Length; startIndex++)
            {
                var v = selector(source[startIndex]);
                if (!double.IsNaN(v))
                {
                    r = v;
                    break;
                }
            }

            for (int i = startIndex; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }


        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static decimal Max(this decimal[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            decimal r = decimal.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static decimal Max<T>(this T[] source, Func<T, decimal> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            decimal r = decimal.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        // --------------------------  this Spans  --------------------------------------------

#if UNITY_2021_3_OR_NEWER
        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static T Max<T>(this Span<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            Comparer<T> comparer = Comparer<T>.Default;
            T r = default(T);
            if (r == null)
            {
                r = source[0];
                for (int i = 1; i < source.Length; i++)
                {
                    if (source[i] != null && comparer.Compare(source[i], r) > 0) r = source[i];
                }
            }
            else
            {
                r = source[0];
                for (int i = 1; i < source.Length; i++)
                {
                    if (comparer.Compare(source[i], r) > 0) r = source[i];
                }
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static TResult Max<T, TResult>(this Span<T> source, Func<T, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            Comparer<TResult> comparer = Comparer<TResult>.Default;
            TResult r = default(TResult);
            if (r == null)
            {
                r = selector(source[0]);
                for (int i = 1; i < source.Length; i++)
                {
                    var v = selector(source[i]);
                    if (v != null && comparer.Compare(v, r) > 0) r = v;
                }
            }
            else
            {
                r = selector(source[0]);
                for (int i = 1; i < source.Length; i++)
                {
                    var v = selector(source[i]);
                    if (comparer.Compare(v, r) > 0) r = v;
                }
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static int Max(this Span<int> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            int r = int.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static int Max<T>(this Span<T> source, Func<T, int> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            int r = int.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static long Max(this Span<long> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            long r = long.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }


        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static long Max<T>(this Span<T> source, Func<T, long> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            long r = long.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static float Max(this Span<float> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            float r = source[0];
            int startIndex = 0;
            for (; startIndex < source.Length; startIndex++)
            {
                if (!float.IsNaN(source[startIndex]))
                {
                    r = source[startIndex];
                    break;
                }
            }

            for (int i = startIndex; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static float Max<T>(this Span<T> source, Func<T, float> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            float r = selector(source[0]);
            int startIndex = 0;
            for (; startIndex < source.Length; startIndex++)
            {
                var v = selector(source[startIndex]);
                if (!float.IsNaN(v))
                {
                    r = v;
                    break;
                }
            }

            for (int i = startIndex; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static double Max(this Span<double> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            double r = source[0];
            int startIndex = 0;
            for (; startIndex < source.Length; startIndex++)
            {
                if (!double.IsNaN(source[startIndex]))
                {
                    r = source[startIndex];
                    break;
                }
            }

            for (int i = startIndex; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static double Max<T>(this Span<T> source, Func<T, double> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            double r = selector(source[0]);
            int startIndex = 0;
            for (; startIndex < source.Length; startIndex++)
            {
                var v = selector(source[startIndex]);
                if (!double.IsNaN(v))
                {
                    r = v;
                    break;
                }
            }

            for (int i = startIndex; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }


        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static decimal Max(this Span<decimal> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            decimal r = decimal.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static decimal Max<T>(this Span<T> source, Func<T, decimal> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Length == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            decimal r = decimal.MinValue;
            for (int i = 0; i < source.Length; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }
#endif

        // --------------------------  LISTS  --------------------------------------------

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static T Max<T>(this List<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            Comparer<T> comparer = Comparer<T>.Default;
            T r = default(T);
            if (r == null)
            {
                r = source[0];
                for (int i = 1; i < source.Count; i++)
                {
                    if (source[i] != null && comparer.Compare(source[i], r) > 0) r = source[i];
                }
            }
            else
            {
                r = source[0];
                for (int i = 1; i < source.Count; i++)
                {
                    if (comparer.Compare(source[i], r) > 0) r = source[i];
                }
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static TResult Max<T, TResult>(this List<T> source, Func<T, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            Comparer<TResult> comparer = Comparer<TResult>.Default;
            TResult r = default(TResult);
            if (r == null)
            {
                r = selector(source[0]);
                for (int i = 1; i < source.Count; i++)
                {
                    var v = selector(source[i]);
                    if (v != null && comparer.Compare(v, r) > 0) r = v;
                }
            }
            else
            {
                r = selector(source[0]);
                for (int i = 1; i < source.Count; i++)
                {
                    var v = selector(source[i]);
                    if (comparer.Compare(v, r) > 0) r = v;
                }
            }

            return r;
        }


        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static int Max(this List<int> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            int r = int.MinValue;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static int Max<T>(this List<T> source, Func<T, int> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            int r = int.MinValue;
            for (int i = 0; i < source.Count; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static long Max(this List<long> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            long r = long.MinValue;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static long Max<T>(this List<T> source, Func<T, long> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            long r = long.MinValue;
            for (int i = 0; i < source.Count; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static float Max(this List<float> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            float r = source[0];
            int startIndex = 0;
            for (; startIndex < source.Count; startIndex++)
            {
                if (!float.IsNaN(source[startIndex]))
                {
                    r = source[startIndex];
                    break;
                }
            }

            for (int i = startIndex; i < source.Count; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static float Max<T>(this List<T> source, Func<T, float> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            float r = selector(source[0]);
            int startIndex = 0;
            for (; startIndex < source.Count; startIndex++)
            {
                var v = selector(source[startIndex]);
                if (!float.IsNaN(v))
                {
                    r = v;
                    break;
                }
            }

            for (int i = startIndex; i < source.Count; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static double Max(this List<double> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            double r = source[0];
            int startIndex = 0;
            for (; startIndex < source.Count; startIndex++)
            {
                if (!double.IsNaN(source[startIndex]))
                {
                    r = source[startIndex];
                    break;
                }
            }

            for (int i = startIndex; i < source.Count; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static double Max<T>(this List<T> source, Func<T, double> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            double r = selector(source[0]);
            int startIndex = 0;
            for (; startIndex < source.Count; startIndex++)
            {
                var v = selector(source[startIndex]);
                if (!double.IsNaN(v))
                {
                    r = v;
                    break;
                }
            }

            for (int i = startIndex; i < source.Count; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <returns>The maximum value in the sequence</returns>
        public static decimal Max(this List<decimal> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            decimal r = decimal.MinValue;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] > r) r = source[i];
            }

            return r;
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and returns the maximum value.
        /// </summary>        
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the transform of the sequence.</returns>
        public static decimal Max<T>(this List<T> source, Func<T, decimal> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Count == 0) throw new InvalidOperationException("Source sequence doesn't contain any elements.");

            decimal r = decimal.MinValue;
            for (int i = 0; i < source.Count; i++)
            {
                var v = selector(source[i]);
                if (v > r) r = v;
            }

            return r;
        }
    }
}