using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    /// <summary>
    /// four-character identifier for a PNG datachunk
    /// </summary>
    public class ChunkType
    {
        private string strType;
        public byte[] Type { get; private set; }
        // Spooky bitshifting magic to get bit five of each byte
        // Refer to https://www.w3.org/TR/2003/REC-PNG-20031110/#5Chunk-naming-conventions as to why
        public bool Ancillary { get { return (Type[0] & (1 << 5)) != 0; } }
        public bool Private { get { return (Type[1] & (1 << 5)) != 0; } }
        public bool Reserved { get { return (Type[2] & (1 << 5)) != 0; } }
        public bool SafeToCopy { get { return (Type[3] & (1 << 5)) != 0; } }
        // End bitshifting magic

        public ChunkType(byte[] type)
        {
            if (type.Length != 4) throw new ArgumentOutOfRangeException("type", "length incorrect");

            this.Type = type;
        }

        public ChunkType(string type) : this(Array.ConvertAll(type.ToCharArray(), item => (byte)item)) //convert to byte array
        {
        }

        public ChunkType(byte[] stream, int index)
        {
            Type = new byte[4];
            for ( int i = 0; i < 4; i++)
            {
                Type[i] = stream[i + index];
            }
        }

        /// <summary>
        /// short description of this chunk type (if known)
        /// </summary>
        /// <returns>human-readable description as string</returns>
        public string Description()
        {
            return Description(this);
        }

        public override string ToString()
        {
            if (strType == null)
            {
                char[] chars = new char[4];
                for (int i = 0; i < 4; i++)
                {
                    chars[i] = (char)Type[i];
                }
                strType = new string(chars);
            }
            return strType;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
            // I think the string hashcode was okay?
            // It's not like anyone's ever going to call this.
        }

        public override bool Equals(object obj)
        {
            ChunkType t = obj as ChunkType;
            if (t == null) return false;
            return Type.SequenceEqual(t.Type);
        }

        /// <summary>
        /// short description of given chunk type (if known)
        /// </summary>
        /// <param name="chunkType">chunk type to describe</param>
        /// <returns>human-readable description, as string</returns>
        public static string Description(ChunkType chunkType)
        {
            switch (chunkType.ToString())
            {
                // critical chunks
                case "IHDR":
                    return "image header, the first chunk in a PNG datastream";
                case "PLTE":
                    return "palette table";
                case "IDAT":
                    return "image data chunk";
                case "IEND":
                    return "image trailer, the last chunk in a PNG datastream";

                // ancillary chunks
                case "tRNS":
                    return "transparency information";

                case "cHRM":
                    return "primary chromaticities and white point";
                case "gAMA":
                    return "image gamma";
                case "iCCP":
                    return "embedded ICC profile";
                case "sBIT":
                    return "significant bits";
                case "sRGB":
                    return "standard RGB color space";
                    
                case "tEXt":
                    return "textual data";
                case "zTXt":
                    return "compressed textual data";
                case "iTXt":
                    return "international textual data";

                case "bKGD":
                    return "background color";
                case "hIST":
                    return "image histogram";
                case "pHYs":
                    return "physical pixel dimensions";
                case "sPLT":
                    return "suggested palette";

                case "tIME":
                    return "image last-modification time";

                // ???
                default:
                    return "unknown chunk";
            }
        }
    }
}
