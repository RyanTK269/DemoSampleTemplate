using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DemoSampleTemplate.Core.Extensions.DataType
{
    /// <summary>
    /// Extensions for <see cref="System.String"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// A nicer way of calling <see cref="System.String.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// A nicer way of calling the inverse of <see cref="System.String.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is not null or an empty string (""); otherwise, false.</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !value.IsNullOrEmpty();
        }

        /// <summary>
        /// A nicer way of calling <see cref="System.String.Format(string, object[])"/>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// Allows for using strings in null coalescing operations
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>Null if <paramref name="value"/> is empty or the original value of <paramref name="value"/>.</returns>
        public static string NullIfEmpty(this string value)
        {
            if (value == string.Empty)
                return null;

            return value;
        }

        /// <summary>
        /// Separates a PascalCase string
        /// </summary>
        /// <example>
        /// "ThisIsPascalCase".SeparatePascalCase(); // returns "This Is Pascal Case"
        /// </example>
        /// <param name="value">The value to split</param>
        /// <returns>The original string separated on each uppercase character.</returns>
        public static string SeparatePascalCase(this string value)
        {
            Ensure.Argument.ArgumentNotNullOrEmpty(value, "value");
            return Regex.Replace(value, "([A-Z])", " $1").Trim();
        }

        /// <summary>
        /// Returns a string array containing the trimmed substrings in this <paramref name="value"/>
        /// that are delimited by the provided <paramref name="separators"/>.
        /// </summary>
        public static IEnumerable<string> SplitAndTrim(this string value, params char[] separators)
        {
            Ensure.Argument.ArgumentNotNull(value, "source");
            return value.Trim().Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
        }

        /// <summary>
        /// Checks if the <paramref name="source"/> contains the <paramref name="input"/> based on the provided <paramref name="comparison"/> rules.
        /// </summary>
        public static bool Contains(this string source, string input, StringComparison comparison)
        {
            return source.IndexOf(input, comparison) >= 0;
        }

        /// <summary>
        /// Limits the length of the <paramref name="source"/> to the specified <paramref name="maxLength"/>.
        /// </summary>
        public static string Limit(this string source, int maxLength, string suffix = null)
        {
            if (suffix.IsNotNullOrEmpty())
            {
                maxLength = maxLength - suffix.Length;
            }

            if (source.Length <= maxLength)
            {
                return source;
            }

            return string.Concat(source.Substring(0, maxLength).Trim(), suffix ?? string.Empty);
        }

        /// <summary>
        /// Determines whether [is not in] [the specified array].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="array">The array.</param>
        /// <returns>
        ///   <c>true</c> if [is not in] [the specified array]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotIn(this string value, string[] array)
        {
            return !array.Contains(value);
        }

        /// <summary>
        /// Determines whether the specified array is in.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="array">The array.</param>
        /// <returns>
        ///   <c>true</c> if the specified array is in; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIn(this string value, string[] array)
        {
            return array.Contains(value);
        }

        /// <summary>
        /// Removes the accent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string RemoveAccent(string value)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double? ConvertToDouble(this string value)
        {
            var resultConvert = double.TryParse(value, out double result);
            return resultConvert ? (double?)result : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ConvertToLong(this string value)
        {
            long.TryParse(value, out long result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? ConvertToNullableInt(this string value)
        {
            if (int.TryParse(value, out int result)) return result;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ConvertToFromBase64String(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : Convert.FromBase64String(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToBase64String(this byte[] value)
        {
            return value == null ? null : Convert.ToBase64String(value);
        }

        /// <summary>
        /// Convert from CamelCase to PascalCase
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertFromCamelCaseToPascalCase(this string value)
        {
            Ensure.Argument.ArgumentNotNullOrEmpty(value, "value");
            return Regex.Replace(value, @"^[a-z]", m => m.ToString().ToUpper());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (url != null && !url.EndsWith("/"))
            {
                return url + "/";
            }
            return url;
        }
    }
}
