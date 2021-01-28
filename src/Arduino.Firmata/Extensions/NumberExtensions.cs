using System;
using System.Linq;

namespace Arduino.Firmata
{
    /// <summary>
    /// Provides extension methods for <see cref="String"/> objects.
    /// </summary>
    public static class NumberExtensions
    {
        public static byte decode8BitSignedByte(byte arg1, byte arg2)
        {
            var result = arg1 | (arg2 << 7);
            //var result = decode32BitSignedInteger(arg1, arg2, 0, 0, 0);
            return (byte)result;
        }
        public static byte[] encode8BitSignedByte(this byte value)
        {
            var pdata = new[] {
                (byte) (value & 0x7f),
                (byte) ((value >> 7) & 0x7f)
            };
            return pdata;
        }
        public static long decode32BitSignedInteger(byte arg1, byte arg2, byte arg3, byte arg4, byte arg5)
        {
            long result = (long)arg1 | (long)arg2 << 7 | (long)arg3 << 14 | (long)arg4 << 21 | (((long)arg5 << 28) & 0x07);

            if ((long)arg5 >> 3 == 0x01)
            {
                result = result * -1;
            }

            return result;
        }
        public static byte[] encode32BitSignedInteger(this int value)
        {
            bool inv = false;

            if (value < 0)
            {
                inv = true;
                value = value * -1;
            }
            var pdata = new[] {
                (byte) (value & 0x7f),
                (byte) ((value >> 7) & 0x7f),
                (byte) ((value >> 14) & 0x7f),
                (byte) ((value >> 21) & 0x7f),
                (byte) ((value >> 28) & 0x7f)
            };
            if (inv == true)
            {
                pdata[4] = (byte)(pdata[4] | 0x08);
            }
            return pdata;
        }
        public static float decodeCustomFloat(byte arg1, byte arg2, byte arg3, byte arg4)
        {
            int l4 = (int)arg4;
            int significand = (int)arg1 | (int)arg2 << 7 | (int)arg3 << 14 | (l4 & 0x03) << 21;
            double exponent = ((l4 >> 2) & 0x0f) - 11;
            bool sign = Convert.ToBoolean((l4 >> 6) & 0x01);
            double result = significand;

            if (sign)
            {
                result *= -1;
            }

            result = result * Math.Pow(10.0, exponent);

            return (float)result;
        }
        public static byte[] encodeCustomFloat(this float value)
        {
            bool inv = false;

            if (value < 0)
            {
                inv = true;
                value = value * -1;
            }

            var decimalNum = getDecimalNum(value);
            var intValue = decimalNum.Item1;
            var 小数位 = decimalNum.Item2;

            var pdata = new byte[] {
                (byte) (intValue & 0x7f),
                (byte) ((intValue >> 7) & 0x7f),
                (byte) ((intValue >> 14) & 0x7f),
                (byte) ((intValue >> 21) & 0x03)
            };

            //小数位
            pdata[3] = (byte)(pdata[3] | (((11 - 小数位) & 0b1111) << 2));

            //正负符号
            if (inv == true)
                pdata[3] = (byte)(pdata[3] | 0b1000000);

            return pdata;
        }
        private static System.Tuple<int, int> getDecimalNum(float num)
        {
            //最多不超过11位小数
            var maxNum = 8388607;
            var result = -5;
            double newNum;
            do
            {
                result++;
                newNum = num * Math.Pow(10, result);

                if ((int)newNum == newNum)
                {
                    break;
                }

            } while (result < 11 - 1 && newNum * 10 <= maxNum);
            if (newNum > maxNum) newNum = maxNum;
            return System.Tuple.Create((int)newNum, result);
        }
    }
}