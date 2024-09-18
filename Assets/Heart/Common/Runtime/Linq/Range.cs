﻿using System;
using System.Collections.Generic;

namespace Pancake.Linq
{
    public static partial class L
    {
        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>A sequence that contains a range of sequential integral numbers.</returns>
        public static int[] RangeArray(int start, int count)
        {
            long max = ((long) start) + count - 1;
            if (count < 0 || max > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(count));

            int[] result = new int[count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = i + start;
            }

            return result;
        }


        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>A sequence that contains a range of sequential integral numbers.</returns>
        public static List<int> RangeList(int start, int count)
        {
            long max = ((long) start) + count - 1;
            if (count < 0 || max > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(count));

            var result = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(i + start);
            }

            return result;
        }
    }
}