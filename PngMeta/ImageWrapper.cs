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
            Image = new BitmapImage(FilePath);
            FileBytes = File.ReadAllBytes(path);
            FileChunks = ImageData.GetChunks(FileBytes);
        }
        
        public byte[] GetImageBuffer()
        {
            List<byte> bytes = new List<byte>();
            foreach (PngDataChunk chunk in FileChunks)
            {
                bytes.AddRange(chunk.GetBytes());
            }
            return bytes.ToArray();
        }

    }
}
