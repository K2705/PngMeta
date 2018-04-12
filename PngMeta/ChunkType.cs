using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    class ChunkType
    {
        public byte[] Type { get; private set; }
        // Spooky bitshifting magic to get the fifth bit of each byte
        // Refer to https://www.w3.org/TR/2003/REC-PNG-20031110/#5Chunk-naming-conventions as to why
        // TODO: Does not work
        public bool Ancillary { get { return (Type[0] & (1 << 4)) != 0; } }
        public bool Private { get { return (Type[1] & (1 << 4)) != 0; } }
        public bool Reserved { get { return (Type[2] & (1 << 4)) != 0; } }
        public bool SafeToCopy { get { return (Type[3] & (1 << 4)) != 0; } }
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

        public override string ToString()
        {
            char[] chars = new char[4];
            for (int i = 0; i < 4; i++)
            {
                chars[i] = (char)Type[i];
            }
            return new string(chars);
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
    }
}
