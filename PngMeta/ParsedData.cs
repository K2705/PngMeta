using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    /// <summary>
    /// image metadata in a human-readable format
    /// </summary>
    public abstract class ParsedChunkData
    {
        public abstract byte[] GetBytes();
    }

    /// <summary>
    /// Image header
    /// </summary>
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
    
    /// <summary>
    /// Image gamma
    /// </summary>
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

    /// <summary>
    /// Text data (English-only)
    /// </summary>
    public class ParsedTEXT : ParsedChunkData
    {
        private Encoding latin1 = Encoding.GetEncoding("ISO-8859-1"); //Latin-1 charset
        public string Key { get; set; }
        public string Value { get; set; }

        public ParsedTEXT(byte[] data)
        {
            StringBuilder sbKey = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();

            int i = 0;

            //keyword is supposed to be 1-79 characters but better not assume
            for (; i < data.Length && data[i] != 0x00; i++) //until null character or end of array
            {
                sbKey.Append(latin1.GetChars(data, i, 1));
            }
            //found null (or data ended prematurely)
            i++;
            //data can be any length, including zero, not null-terminated
            for (; i < data.Length; i++)
            {
                sbValue.Append(latin1.GetChars(data, i, 1));
            }

            Key = sbKey.ToString();
            Value = sbValue.ToString();


        }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(latin1.GetBytes(Key));
            bytes.Add(0x00);
            bytes.AddRange(latin1.GetBytes(Value));
            return bytes.ToArray();
        }
    }

    /// <summary>
    /// Text data (any language)
    /// </summary>
    public class ParsedITXT : ParsedChunkData
    {
        private Encoding textEncoding = Encoding.UTF8;
        public string Keyword { get; set; }
        public bool Compressed { get; private set; }
        public string Language { get; set; }
        public string KeywordTranslated { get; set; }
        public string Text { get; set; }

        public ParsedITXT(byte[] data)
        {
            StringBuilder sbKey = new StringBuilder();
            int i = 0;
            for (; i < data.Length && data[i] != 0x00; i++) //until null character or end of array
            {
                sbKey.Append(textEncoding.GetChars(data, i, 1));
            }
            Keyword = sbKey.ToString();
            i++;
            Compressed = (data[i] == 1);
            i++;
            i++; //compression method byte, not used
            StringBuilder sbLanguage = new StringBuilder();
            for (; i < data.Length && data[i] != 0x00; i++)
            {
                sbLanguage.Append(textEncoding.GetChars(data, i, 1));
            }
            Language = sbLanguage.ToString();
            i++;
            StringBuilder sbTranslated = new StringBuilder();
            for (; i < data.Length && data[i] != 0x00; i++)
            {
                sbTranslated.Append(textEncoding.GetChars(data, i, 1));
            }
            KeywordTranslated = sbTranslated.ToString();
            i++;
            StringBuilder sbText = new StringBuilder();
            for (; i < data.Length; i++) //not null-terminated
            {
                sbText.Append(textEncoding.GetChars(data, i, 1));
            }
            Text = sbText.ToString();

        }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(textEncoding.GetBytes(Keyword));
            bytes.Add(0x00);
            bytes.Add(Compressed ? (byte)1 : (byte)0);
            bytes.Add(0x00); //This is the compression method, but always zero in the spec we're supporting
            bytes.AddRange(textEncoding.GetBytes(Language));
            bytes.Add(0x00);
            bytes.AddRange(textEncoding.GetBytes(KeywordTranslated));
            bytes.Add(0x00);
            bytes.AddRange(textEncoding.GetBytes(Text));
            return bytes.ToArray();
        }
    }

    /// <summary>
    /// Image last-modified time
    /// </summary>
    public class ParsedTIME : ParsedChunkData
    {
        private int year;
        private int month;
        private int day;
        private int hour;
        private int minute;
        private int second;

        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        public int Month
        {
            get { return month; }
            set
            {
                if (value >= 1 && value <= 12)
                    month = value;
            }
        }
        public int Day
        {
            get { return day; }
            set
            {
                if (value >= 1 && value <= 31) // spec doesn't care about the number of days in each month
                    day = value;
            }
        }
        public int Hour
        {
            get { return hour; }
            set
            {
                if (value >= 0 && value <= 23)
                    hour = value;
            }
        }
        public int Minute
        {
            get { return minute; }
            set
            {
                if (value >= 0 && value <= 59)
                    minute = value;
            }
        }
        public int Second
        {
            get { return second; }
            set
            {
                if (value >= 0 && value <= 60)
                    second = value;
            }
        }

        public DateTime Time
        {
            get { return new DateTime(Year, Month, Day, Hour, Minute, Second); }
            set
            {
                Year = value.Year;
                Month = value.Month;
                Day = value.Day;
                Hour = value.Hour;
                Minute = value.Minute;
                Second = value.Second;
            }
        }

        public ParsedTIME(byte[] buffer)
        {
            year = ByteUtils.IntFromTwoBytes(buffer, 0);
            month = (int)buffer[2];
            day = (int)buffer[3];
            hour = (int)buffer[4];
            minute = (int)buffer[5];
            second = (int)buffer[6];

        }

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(ByteUtils.TwoBytesFromInt(year));
            bytes.Add((byte)month);
            bytes.Add((byte)day);
            bytes.Add((byte)hour);
            bytes.Add((byte)minute);
            bytes.Add((byte)second);
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