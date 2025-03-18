using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.DataType
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Checks if the target enumeration includes the specified flag.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// <b>true</b> if match, otherwise <b>false</b>.
        /// </returns>
        /// <remarks>
        /// From http://www.codeproject.com/KB/cs/fun-with-cs-extensions.aspx?msg=2838918#xx2838918xx
        /// </remarks>
        public static bool Includes<TEnum>(this TEnum target, TEnum flags)
            where TEnum : IComparable, IConvertible, IFormattable
        {
            if (target.GetType() != flags.GetType())
            {
                throw new ArgumentException("Enum type mismatch", "flags");
            }

            long a = Convert.ToInt64(target);
            long b = Convert.ToInt64(flags);
            return (a & b) == b;
        }

        /// <summary>
        /// Checks if the target enumeration includes any of the specified flags
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// <b>true</b> if match, otherwise <b>false</b>.
        /// </returns>
        /// <remarks>
        /// From http://www.codeproject.com/KB/cs/fun-with-cs-extensions.aspx?msg=2838918#xx2838918xx
        /// </remarks>
        public static bool IncludesAny<TEnum>(this TEnum target, TEnum flags)
            where TEnum : IComparable, IConvertible, IFormattable
        {
            if (target.GetType() != flags.GetType())
            {
                throw new ArgumentException("Enum type mismatch", "flags");
            }

            long a = Convert.ToInt64(target);
            long b = Convert.ToInt64(flags);
            return (a & b) != 0L;
        }

        /// <summary>
        /// Returns the description string.
        /// </summary>
        /// <param name="val">The enumeration.</param>
        /// <returns>The value of the <see cref="DescriptionAttribute"/> associated with the <c>enum</c> or the ToString of the <c>enum</c>.</returns>
        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : val.ToString();
        }
    }
}
