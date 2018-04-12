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
        public ChunkType Type { get; }
        public byte[] Data { get; set; }
        public UInt32 CRC { get; private set; }

        public PngDataChunk(byte[] chunkBytes)
        {
            Length = ByteUtils.ToUInt32(chunkBytes, 0);
            byte[] type = new byte[4];
            //for (int i = 0; i < 4; i++)
            //{
            //    Type[i] = chunkBytes[i + 4];
            //}
            Array.Copy(chunkBytes, 4, type, 0, 4);
            Type = new ChunkType(type);
            Data = new byte[Length];
            //for (int i = 0; i < Length; i++)
            //{
            //    Data[i] = chunkBytes[i + 8];
            //}
            Array.Copy(chunkBytes, 8, Data, 0, Length);
            CRC = ByteUtils.ToUInt32(chunkBytes, (int)(Length + 8));
            
        }

        public PngDataChunk(UInt32 length, ChunkType type, byte[] data, UInt32 CRC)
        {
            if (data.Length != length)
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
            return Type.ToString();
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Length + 12];
            Array.Copy(ByteUtils.GetBytes(Length), 0, bytes, 0, 4);
            Array.Copy(Type.Type, 0, bytes, 4, 4);
            Array.Copy(Data, 0, bytes, 8, Length);
            Array.Copy(ByteUtils.GetBytes(CRC), 0, bytes, Length + 8, 4);

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
            foreach (byte b in Type.Type)
            {
                ret.Append((char)b);
            }
            ret.Append(' ');
            foreach (byte b in Data)
            {
                ret.Append(b);
            }
            ret.Append(' ');
            foreach (byte b in ByteUtils.GetBytes(CRC))
            {
                ret.Append(b); // TODO: Does not work
            }

            return ret.ToString();
        }
    }
}
