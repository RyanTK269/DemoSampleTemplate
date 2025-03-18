using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.DataType
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}" />
    /// https://github.com/benfoster/Fabrik.Common/blob/master/src/Fabrik.Common/EnumerableExtensions.cs
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs an action on each value of the enumerable
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="enumerable">Sequence on which to perform action</param>
        /// <param name="action">Action to perform on every item</param>
        /// <exception cref="ArgumentNullException">Thrown when given null <paramref name="enumerable" /> or <paramref name="action" /></exception>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            Ensure.Argument.ArgumentNotNull(enumerable, "enumerable");
            Ensure.Argument.ArgumentNotNull(action, "action");

            foreach (T value in enumerable)
            {
                action(value);
            }
        }

        /// <summary>
        /// Convenience method for retrieving a specific page of items within a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">The index of the page to get.</param>
        /// <param name="pageSize">The size of the pages.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            Ensure.Argument.ArgumentNotNull(source, "source");
            Ensure.Argument.Is(pageIndex >= 0, "The page index cannot be negative.");
            Ensure.Argument.Is(pageSize > 0, "The page size must be greater than zero.");

            return source.Skip(pageIndex * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Split list data to list of pages base on PageSize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageSize">The size of the pages.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetListPages<T>(this IEnumerable<T> source, int pageSize)
        {
            Ensure.Argument.ArgumentNotNull(source, "source");
            Ensure.Argument.Is(pageSize > 0, "The page size must be greater than zero.");

            List<IEnumerable<T>> returnList = new List<IEnumerable<T>>();
            int pageIndex = 0;
            bool flag = true;
            while (flag)
            {
                flag = false;
                IEnumerable<T> listOfPage = source.GetPage(pageIndex, pageSize);
                if (listOfPage != null && listOfPage.Any())
                {
                    returnList.Add(listOfPage);
                    if (listOfPage.Count() == pageSize)
                    {
                        pageIndex++;
                        flag = true;
                    }
                }
            }
            return returnList;
        }

        /// <summary>
        /// Converts an enumerable into a readonly collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns></returns>
        public static IEnumerable<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        /// <summary>
        /// Validates that the <paramref name="enumerable" /> is not null and contains items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified enumerable]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.IsNotNullOrEmpty();
        }

        /// <summary>
        /// Validates that the <paramref name="enumerable" /> is not null and contains items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        ///   <c>true</c> if [is not null or empty] [the specified enumerable]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.Any();
        }

        /// <summary>
        /// Concatenates the members of a collection, using the specified separator between each member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">The values.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>
        /// A string that consists of the members of <paramref name="values" /> delimited by the <paramref name="separator" /> string. If values has no members, the method returns null.
        /// </returns>
        public static string JoinOrDefault<T>(this IEnumerable<T> values, string separator)
        {
            Ensure.Argument.ArgumentNotNullOrEmpty(separator, "separator");

            if (values == null)
                return default;

            return string.Join(separator, values);
        }

        /// <summary>
        /// Distincts the by.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count).ToArray();
        }
    }

    public static class Ensure
    {
        /// <summary>
        /// Ensures that the given expression is true
        /// </summary>
        /// <param name="condition">Condition to test/ensure</param>
        /// <param name="message">Message for the exception</param>
        /// <exception cref="Exception">Exception thrown if false condition</exception>
        /// <exception cref="Exception">Exception thrown if false condition</exception>
        public static void That(bool condition, string message = "")
        {
            That<Exception>(condition, message);
        }

        /// <summary>
        /// Ensures that the given expression is true
        /// </summary>
        /// <typeparam name="TException">Type of exception to throw</typeparam>
        /// <param name="condition">Condition to test/ensure</param>
        /// <param name="message">Message for the exception</param>
        /// <exception cref="Exception">Thrown when <paramref name="condition" /> is false</exception>
        /// <remarks>
        ///   <see cref="Exception" /> must have a constructor that takes a single string
        /// </remarks>
        public static void That<TException>(bool condition, string message = "") where TException : Exception
        {
            if (!condition)
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Ensures given condition is false
        /// </summary>
        /// <typeparam name="TException">Type of exception to throw</typeparam>
        /// <param name="condition">Condition to test</param>
        /// <param name="message">Message for the exception</param>
        /// <exception cref="Exception">Thrown when <paramref name="condition" /> is true</exception>
        /// <remarks>
        ///   <see cref="Exception" /> must have a constructor that takes a single string
        /// </remarks>
        public static void Not<TException>(bool condition, string message = "") where TException : Exception
        {
            That<TException>(!condition, message);
        }

        /// <summary>
        /// Ensures given condition is false
        /// </summary>
        /// <param name="condition">Condition to test</param>
        /// <param name="message">Message for the exception</param>
        /// <exception cref="Exception">Thrown when <paramref name="condition" /> is true</exception>
        public static void Not(bool condition, string message = "")
        {
            Not<Exception>(condition, message);
        }

        /// <summary>
        /// Ensures given object is not null
        /// </summary>
        /// <param name="value">Value of the object to test for null reference</param>
        /// <param name="message">Message for the Null Reference Exception</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="value" /> is null</exception>
        public static void NotNull(object value, string message = "")
        {
            That<NullReferenceException>(value != null, message);
        }

        /// <summary>
        /// Ensures given objects are equal
        /// </summary>
        /// <typeparam name="T">Type of objects to compare for equality</typeparam>
        /// <param name="left">First Value to Compare</param>
        /// <param name="right">Second Value to Compare</param>
        /// <param name="message">Message of the exception when values equal</param>
        /// <exception cref="Exception">Exception is thrown when left not equal toright /></exception>
        /// <remarks>
        /// Null values will cause an exception to be thrown
        /// </remarks>
        public static void Equal<T>(T left, T right, string message = "Values must be equal")
        {
            That(left != null && right != null && left.Equals(right), message);
        }

        /// <summary>
        /// Ensures given objects are not equal
        /// </summary>
        /// <typeparam name="T">Type of objects to compare for equality</typeparam>
        /// <param name="left">First Value to Compare</param>
        /// <param name="right">Second Value to Compare</param>
        /// <param name="message">Message of the exception when values equal</param>
        /// <exception cref="Exception">Exception is thrown when left not equal toright /></exception>
        /// <remarks>
        /// Null values will cause an exception to be thrown
        /// </remarks>
        public static void NotEqual<T>(T left, T right, string message = "Values must not be equal")
        {
            That(left != null && right != null && !left.Equals(right), message);
        }

        /// <summary>
        /// Ensures given collection contains a value that satisfied a predicate
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection to test</param>
        /// <param name="predicate">Predicate where one value in the collection must satisfy</param>
        /// <param name="message">Message of the exception if value not found</param>
        /// <exception cref="Exception">Thrown if collection is null, empty or doesn't contain a value that satisfies predicate/></exception>
        public static void Contains<T>(IEnumerable<T> collection, Func<T, bool> predicate, string message = "")
        {
            That(collection != null && collection.Any(predicate), message);
        }

        /// <summary>
        /// Ensures ALL items in the given collection satisfy a predicate
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection to test</param>
        /// <param name="predicate">Predicate that ALL values in the collection must satisfy</param>
        /// <param name="message">Message of the exception if not all values are valid</param>
        /// <exception cref="Exception">Thrown if collection is null, empty or not all values satisfies predicate /></exception>
        public static void Items<T>(IEnumerable<T> collection, Func<T, bool> predicate, string message = "")
        {
            That(collection != null && !collection.Any(x => !predicate(x)), message);
        }

        /// <summary>
        /// Ensures given string is not null or empty
        /// </summary>
        /// <param name="value">String value to compare</param>
        /// <param name="message">Message of the exception if value is null or empty</param>
        /// <exception cref="Exception">string value is null or empty</exception>
        public static void NotNullOrEmpty(string value, string message = "String cannot be null or empty")
        {
            That(value.IsNotNullOrEmpty(), message);
        }

        /// <summary>
        /// Argument-specific ensure methods
        /// </summary>
        public static class Argument
        {
            /// <summary>
            /// Ensures given condition is true
            /// </summary>
            /// <param name="condition">Condition to test</param>
            /// <param name="message">Message of the exception if condition fails</param>
            /// <exception cref="ArgumentException">Thrown if condition is false</exception>
            public static void Is(bool condition, string message = "")
            {
                That<ArgumentException>(condition, message);
            }

            /// <summary>
            /// Ensures given condition is false
            /// </summary>
            /// <param name="condition">Condition to test</param>
            /// <param name="message">Message of the exception if condition is true</param>
            /// <exception cref="ArgumentException">Thrown if condition is true</exception>
            public static void IsNot(bool condition, string message = "")
            {
                Is(!condition, message);
            }

            /// <summary>
            /// Ensures given value is not null
            /// </summary>
            /// <param name="value">Value to test for null</param>
            /// <param name="paramName">Name of the parameter in the method</param>
            /// <exception cref="ArgumentNullException">Thrown if valueis null</exception>
            public static void ArgumentNotNull(object value, string paramName = "")
            {
                That<ArgumentNullException>(value != null, paramName);
            }

            /// <summary>
            /// Ensures the given string value is not null or empty
            /// </summary>
            /// <param name="value">Value to test for null or empty</param>
            /// <param name="paramName">Name of the parameter in the method</param>
            /// <exception cref="ArgumentException">Thrown if value is null or empty string</exception>
            public static void ArgumentNotNullOrEmpty(string value, string paramName = "")
            {
                if (value.IsNullOrEmpty())
                {
                    if (paramName.IsNullOrEmpty())
                        throw new ArgumentException("String value cannot be empty");

                    throw new ArgumentException("String parameter " + paramName + " cannot be null or empty", paramName);
                }
            }
        }
    }
}
