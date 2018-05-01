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
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int length = (int)stream.Length;
                FileBytes = new byte[length];
                stream.Read(FileBytes, 0, length);
            }
            FileChunks = ImageData.GetChunks(FileBytes);
        }
        
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

        internal void SaveImage(string path, byte[] buffer)
        {
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
