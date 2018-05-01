using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    public static class ByteUtils
    {
        public static UInt32 ToUInt32(byte[] buffer)
        {
            return ToUInt32(buffer, 0);
        }

        public static UInt32 ToUInt32(byte[] buffer, int index)
        {
            int value = buffer[index++] << 24 | buffer[index++] << 16 | buffer[index++] << 8 | buffer[index++];
            return (UInt32)value;
        }

        public static Int32 ToInt32(byte[] buffer)
        {
            return ToInt32(buffer, 0);
        }

        public static Int32 ToInt32(byte[] buffer, int index)
        {
            int value = buffer[index++] << 24 | buffer[index++] << 16 | buffer[index++] << 8 | buffer[index++];
            return value;
        }

        public static byte[] GetBytes(UInt32 number)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(number >> 24);
            bytes[1] = (byte)(number >> 16);
            bytes[2] = (byte)(number >> 8);
            bytes[3] = (byte)number;
            return bytes;
        }

        public static byte[] GetBytes(Int32 number)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(number >> 24);
            bytes[1] = (byte)(number >> 16);
            bytes[2] = (byte)(number >> 8);
            bytes[3] = (byte)number;
            return bytes;
        }

        public static string ParseAscii(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in buffer)
            {
                if (b < 32 || b > 126) // non-printing character
                {
                    sb.Append('.');
                }
                else
                {
                    sb.Append((char)b);
                }
            }
            return sb.ToString();
        }

        public static int IntFromTwoBytes(byte[] buffer, int index)
        {
            int value = buffer[index++] << 8 | buffer[index++];
            return value;
        }

        public static byte[] TwoBytesFromInt(int number)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(number >> 8);
            bytes[1] = (byte)number;
            return bytes;
        }

    }
}
