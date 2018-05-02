using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PngMeta
{
    class ImageWrapper
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
            FileChunks = ImageData.GetChunks(FileBytes);
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
    }
}
