using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexTest
{
    class BytesWrapper
    {
        private byte[] bytes;

        public byte[] Bytes
        {
            get
            {
                return bytes;
            }
            set
            {
                bytes = value;
            }
        }
        public string Text
        {
            get
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }
        public string BytesAsText
        {
            get
            {
                return BitConverter.ToString(bytes).Replace("-", " ");
            }
        }
    }
}
