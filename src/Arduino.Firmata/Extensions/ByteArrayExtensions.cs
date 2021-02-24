using System;

namespace Arduino.Firmata
{
    /// <summary>
    /// Provides extension methods for <see cref="byte"/> arrays.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Converts a <see cref="byte"/> array holding binary coded digits to a readable string.
        /// </summary>
        /// <param name="data">The binary coded digit bytes</param>
        /// <param name="isLittleEndian">Value indicating if the first nibble contains the least significant part</param>
        /// <returns>A string containing numeric data</returns>
        /// <exception cref="ArgumentException">The array contains one or more non-BCD bytes.</exception>
        public static string ConvertBinaryCodedDecimalToString(this byte[] data, bool isLittleEndian = false)
        {
            if (data == null)
                throw new ArgumentNullException();

            if (data.Length == 0)
                return string.Empty;

            char[] chars = new char[data.Length * 2];
            int charIndex = 0;

            if (isLittleEndian)
            {
                for (int x = data.Length - 1; x >= 0; x--)
                {
                    chars[charIndex++] = ConvertToChar(data[x] & 0x0F);
                    chars[charIndex++] = ConvertToChar(data[x] >> 4);
                }
            }
            else
            {
                for (int x = 0; x < data.Length; x++)
                {
                    chars[charIndex++] = ConvertToChar(data[x] >> 4);
                    chars[charIndex++] = ConvertToChar(data[x] & 0x0F);
                }
            }

            return new string(chars);
        }

        private static char ConvertToChar(int code)
        {
            if (code > 9)
                throw new ArgumentException(Messages.ArgumentEx_CannotConvertBcd);

            return Convert.ToChar(code | 0x30);
        }
        public static System.Collections.Generic.IEnumerable<byte> Encoder7Bit(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();

            int previous = 0;
            int shift = 0;

            foreach (var data in bytes)
            {
                if (shift == 0)
                {
                    //Firmata.write(data & 0x7f);
                    yield return (byte)(data & 0x7f);
                    shift++;
                    previous = data >> 7;
                }
                else
                {
                    //Firmata.write(((data << shift) & 0x7f) | previous);
                    yield return (byte)(((data << shift) & 0x7f) | previous);
                    if (shift == 6)
                    {
                        //Firmata.write(data >> 1);
                        yield return (byte)(data >> 1);
                        shift = 0;
                    }
                    else
                    {
                        shift++;
                        previous = data >> (8 - shift);
                    }
                }
            }
            if (shift > 0)
            {
                //Firmata.write(previous);
                yield return (byte)previous;
            }
        }
        public static byte[] Decode7Bit(byte[] inData)
        {
            if (inData == null)
                throw new ArgumentNullException();

            var outBytes = num7BitOutbytes(inData.Length);
            byte[] outData = new byte[outBytes];
            for (int i = 0; i < outBytes; i++)
            {
                int j = i << 3;
                int pos = j / 7;
                byte shift = (byte)(j % 7);
                outData[i] = (byte)((inData[pos] >> shift) | ((inData[pos + 1] << (7 - shift)) & 0xFF));
            }

            return outData;
        }
        public static int num7BitOutbytes(int a)
        {
            return (a * 7) >> 3;
        }
    }
}
