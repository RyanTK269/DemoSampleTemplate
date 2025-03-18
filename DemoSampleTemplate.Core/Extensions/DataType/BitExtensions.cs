using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.DataType
{
    public static class BitExtensions
    {
        /// <summary>
        /// Reverses the specified value up to the number of bits.
        /// </summary>
        /// <param name="i">The bits to reverse.</param>
        /// <param name="bits">The number of bits to reverse from the left (lsb).  Other bits are discarded.</param>
        /// <returns>The reversed bits value.</returns>
        public static ulong Reverse(this ulong i, int bits)
        {
            i = ((i >> 1) & 0x5555555555555555) | ((i & 0x5555555555555555) << 1);
            i = ((i >> 2) & 0x3333333333333333) | ((i & 0x3333333333333333) << 2);
            i = ((i >> 4) & 0x0F0F0F0F0F0F0F0F) | ((i & 0x0F0F0F0F0F0F0F0F) << 4);
            i = ((i >> 8) & 0x00FF00FF00FF00FF) | ((i & 0x00FF00FF00FF00FF) << 8);
            i = ((i >> 16) & 0x0000FFFF0000FFFF) | ((i & 0x0000FFFF0000FFFF) << 16);
            i = (i >> 32) | (i << 32);
            return i >> (64 - bits);
        }

        /// <summary>
        /// Returns a <see cref="String"/> formatted as hex with spaces that represents the current ushort[] array.
        /// </summary>
        /// <param name="data">The byte[] array.</param>
        /// <returns>A <see cref="String"/> formatted as hex with spaces that represents the current ushort[] array.</returns>
        public static string ToStringHex(this ushort[] data)
        {
            StringBuilder datastring = new StringBuilder(data.Length * 5);

            for (var i = 0; i < data.Length; i++)
            {
                datastring.AppendFormat("{0,4:x4} ", data[i]);
            }

            // remove the last space
            datastring.Remove(datastring.Length - 1, 1);

            return datastring.ToString();
        }

        public static byte[] ToBytes(this ushort[] data)
        {
            var bytes = new byte[data.Length * 2];

            for (int i = 0; i < data.Length; i++)
            {
                var shortWord = data[i];

                bytes[i * 2] = (byte)((shortWord >> 8) & 0xFF);
                bytes[i * 2 + 1] = (byte)(shortWord & 0xFF);
            }

            return bytes;
        }

        /// <summary>
        /// Returns a <see cref="String"/> formatted as hex with spaces that represents the current byte[] array.
        /// </summary>
        /// <param name="data">The byte[] array.</param>
        /// <returns>A <see cref="String"/> formatted as hex with spaces that represents the current byte[] array.</returns>
        public static string ToStringHex(this byte[] data)
        {
            StringBuilder datastring = new StringBuilder(data.Length * 3);

            for (var i = 0; i < data.Length; i++)
            {
                datastring.AppendFormat("{0,2:x2} ", data[i]);
            }

            // remove the last space
            datastring.Remove(datastring.Length - 1, 1);

            return datastring.ToString();
        }

        /// <summary>
        /// Returns a <see cref="String"/> formatted in ASCII characters that represents the current byte[] array.
        /// </summary>
        /// <param name="data">The byte[] array.</param>
        /// <returns>A <see cref="String"/> formatted in ASCII characters that represents the current byte[] array.</returns>
        public static string ToStringAscii(this byte[] data)
        {
            StringBuilder datastring = new StringBuilder(data.Length);

            for (var i = 0; i < data.Length; i++)
            {
                if (20 > data[i])
                {
                    datastring.Append((char)0xB7);
                }
                else
                {
                    datastring.Append((char)data[i]);
                }
            }

            return datastring.ToString();
        }

        /// <summary>
        /// Converts the numeric value of input value to its equivalent binary string
        /// representation. Prepends '0b' to the string.
        /// </summary>
        /// <returns>
        /// A String of binary sets separated by spaces, where each pair represents the
        /// corresponding element in value; for example, "0b1101 1100".
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Length not divisable by 8.</exception>
        /// <exception cref="ArgumentNullException">value is a null reference</exception>
        /// <example>
        /// <code lang="CS">
        /// string dataBinary = BitHelper.FormatBinary(data, 16);
        /// </code>
        /// </example>
        /// <param name="value">Contains the bits to output right aligned in the value.</param>
        /// <param name="length">The length of the bits in the value. Must be divisible by 8.</param>
        public static string ToStringBinary(this int value, int length)
        {
            if ((length % 8) != 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "length not divisable by 8");
            }

            byte[] valueByteLength = Right(BytesToLittleEndian(BitConverter.GetBytes(value)), length / 8);

            BitArray valueBits = new BitArray(length);

            // Build the BitArray the way we want it.
            for (int i = 0; i < length; i++)
            {
                valueBits.Set(i, ((valueByteLength[i / 8] >> (7 - (i % 8))) & 0x01) != 0);
            }

            // Format BitArray to string with space every 4 bits (starting from right).
            string valueString = string.Empty;
            {
                int posNeg = valueBits.Length;
                foreach (bool bit in valueBits)
                {
                    if (posNeg % 4 == 0 && posNeg > 0 && posNeg != valueBits.Length)
                    {
                        valueString += " ";
                    }

                    posNeg--;
                    valueString += bit ? "1" : "0";
                }
            }

            return "0b" + valueString;
        }

        /// <summary>
        /// Shift a single bit to the left within a byte
        /// </summary>
        /// <returns>byte with value shifted</returns>
        /// <exception cref="ArgumentOutOfRangeException">'shift' needs to be at least 0"</exception>
        /// <example>
        /// <code lang="CS">
        /// <![CDATA[
        /// byte writeBuffer = (byte)((BitHelper.Shift((int)txRxMode, 1, 0)
        ///     | BitHelper.Shift((int)powerMode, 1, 2)
        ///     | BitHelper.Shift(on12SecondFrameBoundary, 3)
        ///     | BitHelper.Shift(on24HourFrameBoundary, 4)) & 0x00FF);]]>
        /// </code>
        /// </example>
        /// <param name="value">Bit to shift into output word.</param>
        /// <param name="shift">Amount to shift to the left</param>
        public static byte ShiftToByte(this bool value, int shift)
        {
            if (shift < 0)
            {
                throw new ArgumentOutOfRangeException("shift", shift, "'shift' needs to be at least 0");
            }

            return (byte)(((value ? 1 : 0) << shift) & (0x1 << shift));
        }

        /// <summary>
        /// Shift a value to the left within a byte
        /// </summary>
        /// <returns>byte with value shifted</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 'shift' needs to be at least 0 and 'width' needs to be at least 1
        /// </exception>
        /// <example>
        /// <code lang="CS">
        /// <![CDATA[
        /// byte writeBuffer = (byte)((BitHelper.Shift((byte)txRxMode, 1, 0)
        ///     | BitHelper.Shift((byte)powerMode, 1, 2)
        ///     | BitHelper.Shift(on12SecondFrameBoundary, 3)
        ///     | BitHelper.Shift(on24HourFrameBoundary, 4)) & 0x00FF);]]>
        /// </code>
        /// </example>
        /// <param name="value">Contains the bits to output right aligned in the value.</param>
        /// <param name="width">The length of the bits in the value.</param>
        /// <param name="shift">Amount to shift to the left</param>
        public static byte Shift(this byte value, int width, int shift)
        {
            if (shift < 0)
            {
                throw new ArgumentOutOfRangeException("shift", shift, "'shift' needs to be at least 0");
            }

            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("width", width, "'width' needs to be at least 1");
            }

            return (byte)((value << shift) & (((0x1 << width) - 1) << shift));
        }

        /// <summary>
        /// Converts a byte array between native ordered and big endian.
        /// </summary>
        /// <remarks>
        /// On a little endian machine (Windows), the bytes are flipped, otherwise
        /// they remain the same.
        /// </remarks>
        /// <returns>An array of bytes.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Too many bytes passed in.  Limit to 4.
        /// </exception>
        /// <example>
        /// <code lang="CS">
        /// <![CDATA[
        /// byte[] flashAddressByte = BitHelper.Right(BitHelper.BytesToLittleEndian(BitConverter.GetBytes(BitHelper.Shift(flashAddress >> 9, 15, 0))), 2);]]>
        /// </code>
        /// </example>
        /// <param name="value">An array of bytes.</param>
        public static byte[] BytesToLittleEndian(this byte[] value)
        {
            byte[] newValue = (byte[])value.Clone();

            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(newValue);
            }

            return newValue;
        }

        /// <summary>
        /// Shifts the bits within the byte array to the left.
        /// </summary>
        /// <returns>An array of bytes.</returns>
        /// <example>
        /// <code lang="CS">
        /// byte[] result = BitHelper.ShiftBytesLeft(requestWord, 0);
        /// </code>
        /// </example>
        /// <param name="value">An array of bytes.</param>
        /// <param name="shift">Number of bits to shift to the left.</param>
        public static byte[] ShiftBytesLeft(this byte[] value, int shift)
        {
            byte[] newValue = new byte[value.Length];
            byte overflow = 0x00;

            // Count down to 0
            for (int i = value.Length - 1; i >= 0; i--)
            {
                int byteEndPosition = (i * 8) - shift + 7;
                int resultBytePosition = byteEndPosition / 8;

                if (byteEndPosition >= 0)
                {
                    newValue[resultBytePosition] = (byte)(value[i] << (shift % 8));
                    newValue[resultBytePosition] |= overflow;
                    overflow = (byte)(((value[i] << (shift % 8)) & 0xFF00) >> 8);
                }
            }

            return newValue;
        }

        /// <summary>
        /// Shifts the bits within the byte array to the right.
        /// </summary>
        /// <returns>An array of bytes.</returns>
        /// <example>
        /// <code lang="CS">
        /// byte[] result = BitHelper.ShiftBytesRight(requestWord, 0);
        /// </code>
        /// </example>
        /// <param name="value">An array of bytes.</param>
        /// <param name="shift">Number of bits to shift to the right.</param>
        public static byte[] ShiftBytesRight(this byte[] value, int shift)
        {
            byte[] newValue = new byte[value.Length];
            byte overflow = 0x00;

            // Count up to length
            for (int i = 0; i < value.Length; i++)
            {
                int byteStartPosition = (i * 8) + shift;
                int resultBytePosition = byteStartPosition / 8;

                if (resultBytePosition < value.Length)
                {
                    newValue[resultBytePosition] = (byte)(value[i] >> (shift % 8));
                    newValue[resultBytePosition] |= overflow;
                    overflow = (byte)((value[i] << (8 - (shift % 8))) & 0xFF);
                }
            }

            return newValue;
        }

        /// <summary>
        /// Extracts a value from the array of bytes.
        /// </summary>
        /// <returns>Value extracted, up to 32 bits.</returns>
        /// <example>
        /// <code lang="CS">
        /// int firmwareDateDay = BitHelper.GetValueInBytes(thisReport, 23 - 12, 5);
        /// </code>
        /// </example>
        /// <param name="value">An array of bytes in big endian order.</param>
        /// <param name="startBit">0 index of bits starting from the left.</param>
        /// <param name="length">Length of bits to extract.</param>
        public static int GetValueInBytes(this byte[] value, int startBit, int length)
        {
            int byteSize = (length / 8) + (length % 8 > 0 ? 1 : 0);

            if (byteSize > 4)
            {
                throw new ArgumentOutOfRangeException("length", length, "length must be 32 bits or less");
            }

            // what size value are we putting this into?  Up to 8 bytes.
            int byteSizeUp = (byteSize <= 1) ? 1 : ((byteSize <= 2) ? 2 : ((byteSize <= 4) ? 4 : 8));

            // holds the value we want, right aligned, with garbage to the left
            byte[] shiftedValue = new byte[byteSizeUp];
            Array.Copy(ShiftBytesRight(value, (value.Length * 8) - startBit - length), value.Length - byteSizeUp, shiftedValue, 0, byteSizeUp);

            // Convert to little endian for the BitConverter
            shiftedValue = BytesToLittleEndian(shiftedValue);

            // TODO: Extend for signed numbers?

            switch (byteSizeUp)
            {
                case 1:
                    byte mask8 = (byte)((1 << length) - 1);
                    return shiftedValue[0] & mask8;
                case 2:
                    short mask16 = (short)((1 << length) - 1);
                    return BitConverter.ToInt16(shiftedValue, 0) & mask16;
                case 4:
                    int mask32 = (1 << length) - 1;
                    return BitConverter.ToInt32(shiftedValue, 0) & mask32;
                case 8:
                    long mask64 = ((long)1 << length) - 1;
                    return (int)(BitConverter.ToInt64(shiftedValue, 0) & mask64);
                default:
                    throw new InvalidOperationException("Something is wrong with the conversion");
            }
        }

        public static T[] Right<T>(this T[] value, int countFromRight)
            where T : struct
        {
            T[] retVal = new T[countFromRight];
            Buffer.BlockCopy(value, value.Length - countFromRight, retVal, 0, countFromRight);
            return retVal;
        }
    }
}
