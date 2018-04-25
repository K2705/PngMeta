using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    public abstract class ParsedChunkData
    {
        public abstract byte[] GetBytes();
    }

    public class ParsedIHDR : ParsedChunkData
    {
        public int Width { get; }
        public int Height { get; }
        public byte BitDepth { get; }
        public byte ColourType { get; }
        public byte CompressionMethod { get; }
        public byte FilterMethod { get; }
        public byte InterlaceMethod { get; }


        public ParsedIHDR(byte[] data)
        {
            Width = ByteUtils.ToInt32(data, 0);
            Height = ByteUtils.ToInt32(data, 4);
            BitDepth = data[8];
            ColourType = data[9];
            CompressionMethod = data[10];
            FilterMethod = data[11];
            InterlaceMethod = data[12];
        }

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[13];
            ByteUtils.GetBytes(Width).CopyTo(bytes, 0);
            ByteUtils.GetBytes(Height).CopyTo(bytes, 4);
            bytes[8] = BitDepth;
            bytes[9] = ColourType;
            bytes[10] = CompressionMethod;
            bytes[11] = FilterMethod;
            bytes[12] = InterlaceMethod;

            return bytes;
        }
    }

    public class ParsedGAMA : ParsedChunkData
    {
        private UInt32 gamma;
        public int Gamma
        {
            get
            {
                return (int)(gamma / 100000u);
            }
            set
            {
                gamma = (uint)(value * 100000);
            }
        }

        public ParsedGAMA(byte[] data)
        {
            gamma = ByteUtils.ToUInt32(data);
        }

        public override byte[] GetBytes()
        {
            return ByteUtils.GetBytes(gamma);
        }
    }

    public class ParsedTEXT : ParsedChunkData
    {
        private Encoding latin1 = Encoding.GetEncoding("ISO-8859-1"); //Latin-1 charset
        public List<StringPair> TextData { get; set; }

        public ParsedTEXT(byte[] data)
        {
            TextData = new List<StringPair>();
            StringBuilder sbKey = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();

            int i = 0;
            while (i < data.Length)
            {
                //keyword is supposed to be 1-79 characters but better not assume
                for (; i < data.Length && data[i] != 0x00; i++) //until null character or end of array
                {
                    sbKey.Append(latin1.GetChars(data, i, 1));
                }
                //found null (or data ended prematurely)
                i++;
                //data can be any length, including zero
                for (; i < data.Length && data[i] != 0x00; i++)
                {
                    sbValue.Append(latin1.GetChars(data, i, 1));
                }
                i++;
                //another null or end of data
                TextData.Add(new StringPair(sbKey.ToString(), sbValue.ToString()));
                sbKey.Clear();
                sbValue.Clear();
            }
            
        }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach (StringPair k in TextData)
            {
                bytes.AddRange(latin1.GetBytes(k.Key));
                bytes.Add(0x00);
                bytes.AddRange(latin1.GetBytes(k.Value));
                bytes.Add(0x00);
            }
            bytes.RemoveAt(bytes.Count - 1); // loop adds one extra null byte at the end
            return bytes.ToArray();
        }
    }


    public class StringPair
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public StringPair(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}