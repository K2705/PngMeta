namespace PngMeta
{
    static class Crc
    {
        private static uint[] CRCTable = new uint[256];
        private static bool CRCTableComputed = false;

        private static void MakeCRCTable()
        {
            uint c;
            for (uint i = 0; i < CRCTable.Length; i++)
            {
                c = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((c & 1) != 0)
                        c = 0xEDB88320 ^ (c >> 1);
                    else c = (c >> 1);
                }
                CRCTable[i] = c;
            }
            CRCTableComputed = true;
        }

        private static uint UpdateCRC(uint crc, byte[] bytes)
        {
            uint c = crc;
            if (!CRCTableComputed)
                MakeCRCTable();
            for (int i = 0; i < bytes.Length; i++)
            {
                c = CRCTable[ (c ^ bytes[i]) & 0xff ] ^ (c >> 8);
            }
            return c;
        }

        public static uint GetCRC(byte[] bytes)
        {
            return UpdateCRC(0xffffffff, bytes) ^ 0xffffffff;
        }
    }
}