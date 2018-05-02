using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PngMeta
{
    public class ImageWrapper
    {
        public List<PngDataChunk> FileChunks { get; private set; }
        public Uri FilePath { get; private set; }
        public BitmapImage Image { get; private set; }

        public byte[] FileBytes { get; private set; }

        /// <summary>
        /// Load a new image 
        /// </summary>
        /// <param name="path"></param>
        public void LoadImage(string path)
        {
            FilePath = new Uri(path);
            Image = new BitmapImage();
            {
                Image.BeginInit();
                Image.CacheOption = BitmapCacheOption.OnLoad;
                Image.UriSource = FilePath;
                Image.EndInit();
            }
            FileBytes = File.ReadAllBytes(path);
            FileChunks = GetChunks(FileBytes);
        }
        
        /// <summary>
        /// Compile all data chunks back into a single byte array for saving
        /// </summary>
        /// <returns>byte[] containing whole image</returns>
        public byte[] GetImageBuffer()
        {
            List<byte> bytes = new List<byte>();
            byte[] pngHeader = { 137, 80, 78, 71, 13, 10, 26, 10, };
            bytes.AddRange(pngHeader);
            foreach (PngDataChunk chunk in FileChunks)
            {
                bytes.AddRange(chunk.GetBytes());
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Save image
        /// </summary>
        /// <param name="path">path to save to, as string</param>
        /// <param name="buffer">image as byte array</param>
        internal void SaveImage(string path, byte[] buffer)
        {
            File.WriteAllBytes(path, buffer);
        }

        private static List<PngDataChunk> GetChunks(byte[] imageBytes)
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
