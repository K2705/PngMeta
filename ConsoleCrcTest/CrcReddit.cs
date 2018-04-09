using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCrcTest
{
    static class CrcReddit
    {
        /* Table of CRCs of all 8-bit messages. */
        public static uint[] crc_table = null;

        /* Make the table for a fast CRC. */
        public static void make_crc_table()
        {
            crc_table = new uint[256];
            uint c;

            for (int n = 0; n < 256; n++)
            {
                c = (uint)n;
                for (int k = 0; k < 8; k++)
                {
                    if ((c & 1) == 1)
                        c = 0xedb88320 ^ (c >> 1);
                    else
                        c = c >> 1;
                }
                crc_table[n] = c;
            }
        }

        public static uint update_crc(byte[] buf)
        {
            uint c = 0xffffffff;

            if (crc_table == null)
                make_crc_table();
            for (int n = 0; n < buf.Length; n++)
            {
                c = crc_table[(c ^ buf[n]) & 0xff] ^ (c >> 8);
            }
            return c ^ 0xffffffff;
        }
    }
}
