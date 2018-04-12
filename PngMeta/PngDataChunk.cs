using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    class PngDataChunk
    {
        public UInt32 Length { get; private set; }
        public byte[] Type { get; }
        public byte[] Data { get; set; }
        public UInt32 CRC { get; private set; }

        public PngDataChunk(byte[] chunkBytes)
        {
            Length = BitConverter.ToUInt32(chunkBytes, 0);
            Type = new byte[4];
            //for (int i = 0; i < 4; i++)
            //{
            //    Type[i] = chunkBytes[i + 4];
            //}
            Array.Copy(chunkBytes, 4, Type, 0, 4);
            Data = new byte[Length];
            //for (int i = 0; i < Length; i++)
            //{
            //    Data[i] = chunkBytes[i + 8];
            //}
            Array.Copy(chunkBytes, 8, Data, 0, Length);
            CRC = BitConverter.ToUInt32(chunkBytes, (int)(Length + 8));
            
        }

        public PngDataChunk(UInt32 length, byte[] type, byte[] data, UInt32 CRC)
        {
            if (type.Length != 4)
            {
                throw new ArgumentOutOfRangeException("type", "field length mismatch");
            }
            if (data.Length != Length)
            {
                throw new ArgumentOutOfRangeException("data", "field length mismatch");
            }

            this.Length = length;
            this.Type = type;
            this.Data = data;
            this.CRC = CRC;
        }

        public string TypeString()
        {
            char[] chars = new char[4];
            for (int i = 0; i < 4; i++)
            {
                chars[i] = (char)Type[i];
            }
            return chars.ToString();
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Length + 12];
            Array.Copy(BitConverter.GetBytes(Length), 0, bytes, 0, 4);
            Array.Copy(Type, 0, bytes, 4, 4);
            Array.Copy(Data, 0, bytes, 8, Length);
            Array.Copy(BitConverter.GetBytes(CRC), 0, bytes, Length + 8, 4);

            return bytes;
        }


        public void UpdateLength()
        {
            Length = (uint)Data.Length;
        }

        public void UpdateCrc()
        {
            //TODO
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(Length);
            ret.Append(' ');
            foreach (byte b in Type)
            {
                ret.Append((char)b);
            }
            ret.Append(' ');
            foreach (byte b in Data)
            {
                ret.Append(b);
            }
            ret.Append(' ');
            foreach (byte b in BitConverter.GetBytes(CRC))
            {
                ret.Append(b);
            }

            return ret.ToString();
        }
    }
}
