﻿using System;
using System.Collections.Generic;


namespace Pancake.Linq
{
    public static partial class L
    {
        // --------------------------  ARRAYS  --------------------------------------------

        /// <summary>
        /// Projects each element of a sequence into a new form in place.
        /// The result will change itself <paramref name = "source" />
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on (map).</param>
        /// <param name="selector">A transform function to apply (map) to each element.</param>        
        public static void MapOrigin<T>(this T[] source, Func<T, T> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            for (int i = 0; i < source.Length; i++)
            {
                source[i] = selector(source[i]);
            }
        }

        /// <summary>
        /// Projects each element of a sequence into a new form, in place, by incorporating the element's index.
        /// The result will change itself <paramref name = "source" />
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>        
        public static void MapOrigin<T>(this T[] source, Func<T, int, T> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            for (int i = 0; i < source.Length; i++)
            {
                source[i] = selector(source[i], i);
            }
        }


        /// <summary>
        /// Projects each element of a sequence into a new form. (map in every other language)
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on (map).</param>
        /// <param name="selector">A transform function to apply (map) to each element.</param>
        /// <returns>A sequence whose elements are the result of invoking the transform function on each element (mapping) of source.</returns>
        public static TResult[] Map<T, TResult>(this T[] source, Func<T, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var r = new TResult[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                r[i] = selector(source[i]);
            }

            return r;
        }


        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence whose elements are the result of invoking the transform function on each element of source.</returns>
        public static TResult[] Map<T, TResult>(this T[] source, Func<T, int, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var r = new TResult[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                r[i] = selector(source[i], i);
            }

            return r;
        }

        // --------------------------  this SpanS  --------------------------------------------

#if UNITY_2021_3_OR_NEWER
        /// <summary>
        /// Projects each element of a sequence into a new form in place.
        /// The result will change itself <paramref name = "source" />
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on (map).</param>
        /// <param name="selector">A transform function to apply (map) to each element.</param>        
        public static void MapOrigin<T>(this Span<T> source, Func<T, T> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            for (int i = 0; i < source.Length; i++)
            {
                source[i] = selector(source[i]);
            }
        }

        /// <summary>
        /// Projects each element of a sequence into a new form, in place, by incorporating the element's index.
        /// The result will change itself <paramref name = "source" />
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>        
        public static void MapOrigin<T>(this Span<T> source, Func<T, int, T> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            for (int i = 0; i < source.Length; i++)
            {
                source[i] = selector(source[i], i);
            }
        }


        /// <summary>
        ///  Projects each element of a sequence into a new form. (map in every other language)
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on (map).</param>
        /// <param name="selector">A transform function to apply (map) to each element.</param>
        /// <returns>A sequence whose elements are the result of invoking the transform function on each element (mapping) of source.</returns>
        public static TResult[] Map<T, TResult>(this Span<T> source, Func<T, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var r = new TResult[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                r[i] = selector(source[i]);
            }

            return r;
        }


        /// <summary>
        ///  Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence whose elements are the result of invoking the transform function on each element of source.</returns>
        public static TResult[] Map<T, TResult>(this Span<T> source, Func<T, int, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var r = new TResult[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                r[i] = selector(source[i], i);
            }

            return r;
        }
#endif

        // --------------------------  LISTS --------------------------------------------

        /// <summary>
        /// Projects each element of a sequence into a new form in place.
        /// The result will change itself <paramref name = "source" />
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on (map).</param>
        /// <param name="selector">A transform function to apply (map) to each element.</param>        
        public static void MapOrigin<T>(this List<T> source, Func<T, T> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            for (int i = 0; i < source.Count; i++)
            {
                source[i] = selector(source[i]);
            }
        }

        /// <summary>
        /// Projects each element of a sequence into a new form, in place, by incorporating the element's index.
        /// The result will change itself <paramref name = "source" />
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>        
        public static void MapOrigin<T>(this List<T> source, Func<T, int, T> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            for (int i = 0; i < source.Count; i++)
            {
                source[i] = selector(source[i], i);
            }
        }


        /// <summary>
        ///  Projects each element of a sequence into a new form. (map in every other language)
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on (map).</param>
        /// <param name="selector">A transform function to apply (map) to each element.</param>
        /// <returns>A sequence whose elements are the result of invoking the transform function on each element (mapping) of source.</returns>
        public static List<TResult> Map<T, TResult>(this List<T> source, Func<T, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var r = new List<TResult>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                r.Add(selector(source[i]));
            }

            return r;
        }


        /// <summary>
        ///  Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>        
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A sequence whose elements are the result of invoking the transform function on each element of source.</returns>
        public static List<TResult> Map<T, TResult>(this List<T> source, Func<T, int, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var r = new List<TResult>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                r.Add(selector(source[i], i));
            }

            return r;
        }
    }
}