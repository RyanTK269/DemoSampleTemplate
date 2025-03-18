using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.DataType
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Sets all values.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array that will be modified.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array</param>
        /// <param name="value">The value.</param>
        /// <returns>A reference to the changed array.</returns>
        public static T[] SetAllValues<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }

            return array;
        }

        /// <summary>
        /// Get the array slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array that will be sliced from.</param>
        /// <param name="index">The start index.</param>
        /// <param name="end">The end index.  If end is negative, it is treated like length.</param>
        /// <returns>The resulting array.</returns>
        public static T[] Slice<T>(this T[] array, int index, int end)
        {
            // Handles negative ends
            int len;
            if (end == 0)
            {
                throw new ArgumentOutOfRangeException("end", end, "must be a positive index or a length (indicated by negative), not 0");
            }
            else if (end > 0)
            {
                len = end - index;
            }
            else
            {
                len = -end;
                end = index + len;
            }

            // Return new array
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = array[i + index];
            }

            return res;
        }

        /// <summary>
        /// Checks if the Arrays are equal.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="a">The <see cref="Array"/> that contains data to compare with.</param>
        /// <param name="b">The <see cref="Array"/> that contains data to compare to.</param>
        /// <param name="index">A 32-bit integer that represents the index in the arrays at which comparing begins.</param>
        /// <param name="length">A 32-bit integer that represents the number of elements to compare.</param>
        /// <returns>
        /// Returns <c>true</c> if all element match and <c>false</c> otherwise.
        /// </returns>
        public static bool ArrayEqual<T>(this T[] a, T[] b, int index, int length)
            where T : IEquatable<T>
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            if (a.Length < index + length || b.Length < index + length)
            {
                return false;
            }

            for (int i = index; i < length; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Chack that the arrays are equal.
        /// </summary>
        /// <typeparam name="T">Type implementing IEquatable.  Value types work.</typeparam>
        /// <param name="a">First array</param>
        /// <param name="b">Second array</param>
        /// <returns>
        /// <c>true</c> if contents are equal, otherwise <c>false</c>.
        /// </returns>
        public static bool ArrayEqual<T>(this T[] a, T[] b)
            where T : IEquatable<T>
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            if (a.LongLength != b.LongLength)
            {
                return false;
            }

            for (long i = 0; i < a.LongLength; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Extracts specified number of the right-most elements from a source array.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="value">The source buffer.</param>
        /// <param name="countFromRight">The number of bytes to copy.</param>
        /// <returns>The resulting buffer.</returns>
        /// <exception cref="ArgumentNullException">value is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">countFromRight is greater than the length of the value array.</exception>
        /// <example>
        /// <code lang="CS">
        /// <![CDATA[
        /// byte[] flashAddressByte = BitHelper.Right(BitHelper.BytesToLittleEndian(BitConverter.GetBytes(BitHelper.Shift(flashAddress >> 9, 15, 0))), 2);]]>
        /// </code>
        /// </example>
        public static T[] Right<T>(this T[] value, int countFromRight)
            where T : struct
        {
            T[] retVal = new T[countFromRight];
            Buffer.BlockCopy(value, value.Length - countFromRight, retVal, 0, countFromRight);
            return retVal;
        }
    }
}
