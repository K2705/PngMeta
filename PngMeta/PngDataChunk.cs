using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    /// <summary>
    /// A single PNG data chunk
    /// </summary>
    public class PngDataChunk
    {
        private byte[] data;

        public UInt32 Length { get; private set; }
        public ChunkType Type { get; }
        public byte[] Data
        {
            get
            {
                if (ParsedData != null)
                    return ParsedData.GetBytes();
                else
                    return data;
            }
        }
        public UInt32 CRC { get; private set; }
        public ParsedChunkData ParsedData { get; set; }
        public string Description
        {
            get
            {
                return Type.Description();
            }
        }

        public PngDataChunk(byte[] chunkBytes)
        {
            Length = ByteUtils.ToUInt32(chunkBytes, 0);
            byte[] type = new byte[4];
            Array.Copy(chunkBytes, 4, type, 0, 4);
            Type = new ChunkType(type);
            data = new byte[Length];
            Array.Copy(chunkBytes, 8, data, 0, Length);
            CRC = ByteUtils.ToUInt32(chunkBytes, (int)(Length + 8));

            ParseData();
        }

        private void ParseData()
        {
            switch (Type.ToString())
            {
                case "IHDR":
                    ParsedData = new ParsedIHDR(data);
                    break;
                case "gAMA":
                    ParsedData = new ParsedGAMA(data);
                    break;
                case "tEXt":
                    ParsedData = new ParsedTEXT(data);
                    break;
                case "iTXt":
                    ParsedData = new ParsedITXT(data);
                    break;
                case "tIME":
                    ParsedData = new ParsedTIME(data);
                    break;
                default:
                    break;
            }
        }

        public PngDataChunk(UInt32 length, ChunkType type, byte[] data, UInt32 CRC)
        {
            if (data.Length != length)
            {
                throw new ArgumentOutOfRangeException("data", "field length mismatch");
            }

            this.Length = length;
            this.Type = type;
            this.data = data;
            this.CRC = CRC;

            ParseData();
        }

        public string TypeString()
        {
            return Type.ToString();
        }

        public byte[] GetBytes()
        {
            UpdateData();
            UpdateLength();
            UpdateCrc();

            byte[] bytes = new byte[Length + 12];
            Array.Copy(ByteUtils.GetBytes(Length), 0, bytes, 0, 4);
            Array.Copy(Type.Type, 0, bytes, 4, 4);
            Array.Copy(data, 0, bytes, 8, Length);
            Array.Copy(ByteUtils.GetBytes(CRC), 0, bytes, Length + 8, 4);

            return bytes;
        }

        public void UpdateData()
        {
            if (ParsedData != null)
            {
                data = ParsedData.GetBytes();
            }
        }

        public void UpdateLength()
        {
            Length = (uint)data.Length;
        }

        public void UpdateCrc()
        {
            this.CRC = CalculateCrc();
        }

        public UInt32 CalculateCrc()
        {
            byte[] buffer = new byte[Length + 4];
            Array.Copy(Type.Type, 0, buffer, 0, 4);
            Array.Copy(data, 0, buffer, 4, Length);
            return Crc.GetCRC(buffer);
        }

        //public virtual List<KeyValuePair<string, string>> ChunkData()
        //{
        //    List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
        //    ret.Add(new KeyValuePair<string, string>(Type.ToString(), "Cannot display chunk contents."));
        //    return ret;
        //}
        

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
            foreach (byte b in data)
            {
                ret.Append(b);
            }
            ret.Append(' ');
            foreach (byte b in ByteUtils.GetBytes(CRC))
            {
                ret.Append(b.ToString("X2")); 
            }

            return ret.ToString();
        }
    }

}
