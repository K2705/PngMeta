namespace CRCTest
{
    class CRC
    {
        private uint[] CRCTable = new uint[256];
        private bool CRCTableComputed = false;

        private void MakeCRCTable()
        {
            uint c;
            for (int i = 0; i < CRCTable.Length; i++)
            {
                c = (uint)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((c & 1) != 0)
                        c = 0xedb88320 ^ (c >> 1);
                    else c = c >> 1;
                }
                CRCTable[i] = c;
            }
            CRCTableComputed = true;
        }

        uint UpdateCRC(uint crc, byte[] bytes, int length)
        {
            uint c = crc;
            if (!CRCTableComputed)
                MakeCRCTable();
            for (int i = 0; i < length; i++)
            {
                c = CRCTable[(c ^ bytes[i]) & 0xff] ^ (c >> 8);
            }
            return c;
        }

        uint GetCRC(byte[] bytes, int length)
        {
            return UpdateCRC(0xffffffff, bytes, length) ^ 0xffffffff;
        }
    }
}
