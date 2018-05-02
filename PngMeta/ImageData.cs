using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngMeta
{
    static class ImageData
    {
        
        public static List<PngDataChunk> GetChunks(byte[] imageBytes)
        {
            // Check for the eight-byte PNG signature
            byte[] pngHeader = { 137, 80, 78, 71, 13, 10, 26, 10, };
            for (int i = 0; i < 8; i++)
            {
                if (pngHeader[i] != imageBytes[i]) throw new ArgumentException("No PNG signature found");
            }

            List<PngDataChunk> chunks = new List<PngDataChunk>();
            int k = 8;

            while (true)
            {
                // get the data
                UInt32 chunkLength = ByteUtils.ToUInt32(imageBytes, k);
                k += 4;
                ChunkType chunkType = new ChunkType(imageBytes, k);
                k += 4;
                byte[] chunkData = new byte[chunkLength];
                for (int i = 0; i < chunkLength; i++, k++)
                {
                    chunkData[i] = imageBytes[k];
                }
                UInt32 chunkCRC = ByteUtils.ToUInt32(imageBytes, k);
                k += 4;

                // create chunk from data and add it to list
                PngDataChunk dataChunk = new PngDataChunk(chunkLength, chunkType, chunkData, chunkCRC);
                chunks.Add(dataChunk);

                // stop once we hit the IEND
                if (chunkType.ToString() == "IEND") break;
                // TODO: proper error handling if IEND is never found
            }

            return chunks;
        }

    }
}
