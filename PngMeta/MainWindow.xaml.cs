using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PngMeta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //byte[] fileBytes;
        //List<PngDataChunk> fileChunks;
        ImageWrapper imageWrapper = new ImageWrapper();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "PNG images|*.png";

            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                imageWrapper.LoadImage(dialog.FileName);
                // TODO: deal w/ exception
                
                uiPreviewImage.Source = imageWrapper.Image;
                uiChunkList.ItemsSource = imageWrapper.FileChunks;
                uiChunkListTitle.Text = imageWrapper.FileChunks.Count + " chunks read";
            }

        }

        private void uiChunkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PngDataChunk currentChunk = uiChunkList.SelectedItem as PngDataChunk;
            if (currentChunk != null)
            {
                //tbRawViewHex.Text = BitConverter.ToString(currentChunk.GetBytes()).Replace("-", " ");
                //tbRawViewAscii.Text = ByteUtils.ParseAscii(currentChunk.GetBytes());
                //if (currentChunk.ParsedData != null)
                //dgChunkContents.ItemsSource = currentChunk.ParsedData.DataList;

                tbChunkName.Text = currentChunk.Type.ToString();
                tbChunkAttribs.Text = currentChunk.Type.Ancillary + " " + currentChunk.Type.Private + " "
                    + currentChunk.Type.Reserved + " " + currentChunk.Type.SafeToCopy;

                spRawHex.Children.Clear();
                WriteStringChunks(spRawHex, BitConverter.ToString(currentChunk.GetBytes()).Replace("-", " "), 24);
                spRawAscii.Children.Clear();
                WriteStringChunks(spRawAscii, ByteUtils.ParseAscii(currentChunk.GetBytes()), 8);

                Console.WriteLine(currentChunk.ToString());
            }
        }

        public static void WriteStringChunks(StackPanel panel, string str, int size)
        {
            for (int i = 0; i < str.Length; i += size)
            {
                TextBox tb = new TextBox();
                tb.IsReadOnly = true;
                tb.FontFamily = new FontFamily("Consolas");
                tb.Text = str.Substring(i, Math.Min(size, str.Length - i));
                panel.Children.Add(tb);
            }
            //ret.Add(str.Substring(i, Math.Min(size, str.Length - i)));
        }

        public static string SplitStringIntoLines(string str, int size)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i+= size)
            {
                sb.Append(str.Substring(i, Math.Min(size, str.Length - i)).Replace('\n', '⏎'));
                if (i < str.Length) sb.Append("\n");
            }
            return sb.ToString();
        }

    }
}
