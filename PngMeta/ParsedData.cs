using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    public abstract class ParsedChunkData
    {
        public abstract bool ReadOnly { get; }
        public abstract bool DynamicFields { get; }
        public abstract List<KeyValuePair<string, string>> DataList { get; }
        public abstract byte[] GetBytes();
    }

    public class ParsedIHDR : ParsedChunkData
    {
        public override bool ReadOnly { get { return true; } }
        public override bool DynamicFields { get { return false; } }

        public override List<KeyValuePair<string, string>> DataList { get; }

        public ParsedIHDR(byte[] data)
        {
            DataList = new List<KeyValuePair<string, string>>();
            DataList.Add(new KeyValuePair<string, string>("Width", ByteUtils.ToInt32(data, 0).ToString()));
            DataList.Add(new KeyValuePair<string, string>("Height", ByteUtils.ToInt32(data, 4).ToString()));
            DataList.Add(new KeyValuePair<string, string>("Bit depth", data[8].ToString()));
            DataList.Add(new KeyValuePair<string, string>("Colour type", data[9].ToString()));
            DataList.Add(new KeyValuePair<string, string>("Compression method", data[10].ToString()));
            DataList.Add(new KeyValuePair<string, string>("Filter method", data[11].ToString()));
            DataList.Add(new KeyValuePair<string, string>("Interlace method", data[12].ToString()));
        }

        public override byte[] GetBytes()
        {
            byte[] bytes = new byte[13];
            ByteUtils.GetBytes(UInt32.Parse(DataList[0].Value)).CopyTo(bytes, 0);
            ByteUtils.GetBytes(UInt32.Parse(DataList[1].Value)).CopyTo(bytes, 4);
            bytes[8] = Byte.Parse(DataList[2].Value);
            bytes[9] = Byte.Parse(DataList[2].Value);
            bytes[10] = Byte.Parse(DataList[2].Value);
            bytes[11] = Byte.Parse(DataList[2].Value);
            bytes[12] = Byte.Parse(DataList[2].Value);

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

        public override bool ReadOnly { get { return true; } }

        public override bool DynamicFields { get { return false; } }

        public override List<KeyValuePair<string, string>> DataList
        {
            get
            {
                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                list.Add(new KeyValuePair<string, string>("Gamma", (gamma / 100000u).ToString()));
                return list;
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
        public override bool ReadOnly { get { return false; } }
        public override bool DynamicFields { get { return true; } }
        private Encoding latin1 = Encoding.GetEncoding("ISO-8859-1"); //Latin-1 charset
        public override List<KeyValuePair<string, string>> DataList { get; }

        public ParsedTEXT(byte[] data)
        {
            DataList = new List<KeyValuePair<string, string>>();
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
                //another null or end of data
                i++;
                DataList.Add(new KeyValuePair<string, string>(sbKey.ToString(), sbValue.ToString()));
                sbKey.Clear();
                sbValue.Clear();
            }
        }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach (KeyValuePair<string, string> k in DataList)
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
}
