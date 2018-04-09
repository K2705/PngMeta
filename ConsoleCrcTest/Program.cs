using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCrcTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Crc crc = new Crc();

            byte[] tEXt;
            tEXt = Encoding.GetEncoding("UTF-8").GetBytes("tEXtSoftware\0www.inkscape.org".ToCharArray());
            //tEXt[12] = 0x00;
            PrintUint(crc.GetCRC(tEXt, tEXt.Length));
            PrintUint(CrcReddit.update_crc(tEXt));
            //should be:
            //9B EE 3C 1A
            //2616081434

            Console.WriteLine();
            byte[] gAMA = { (byte)'g', (byte)'A', (byte)'M', (byte)'A', 0x00, 0x00, 0xB1, 0x8F };
            PrintUint(crc.GetCRC(gAMA, gAMA.Length));
            PrintUint(CrcReddit.update_crc(gAMA));
            //should be:
            //0B FC 61 05
            //201089285

            Console.WriteLine();
            byte[] sRGB = { 0x73, 0x52, 0x47, 0x42, 0x00 };
            PrintUint(crc.GetCRC(sRGB, sRGB.Length));
            PrintUint(CrcReddit.update_crc(sRGB));
            //should be:
            //AE CE 1C E9
            //2932743401

            Console.WriteLine();
            byte[] IEND = { (byte)'I', (byte)'E', (byte)'N', (byte)'D' };
            PrintUint(crc.GetCRC(IEND, IEND.Length));
            PrintUint(CrcReddit.update_crc(IEND));
            //should be:
            //AE 42 60 82
            //2923585666

            Console.WriteLine();
        }

        private static void PrintUint(uint number)
        {
            Console.WriteLine(BitConverter.ToString(BitConverter.GetBytes(number)));
            Console.WriteLine(number);
        }
    }
}
