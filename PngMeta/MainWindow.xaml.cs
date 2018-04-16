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
        byte[] fileBytes;
        List<PngDataChunk> fileChunks;

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
                uiPreviewImage.Source = new BitmapImage(new Uri(dialog.FileName));

                fileBytes = File.ReadAllBytes(dialog.FileName);
                //myTextBox.Text = BitConverter.ToString(fileBytes).Replace("-", " ");
                //myTextBox.Text = Encoding.UTF8.GetString(fileBytes);

                fileChunks = BLImageData.GetChunks(fileBytes);
                uiChunkList.ItemsSource = fileChunks;
                uiChunkListTitle.Text = fileChunks.Count + " chunks read";
            }

        }

        private void uiChunkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PngDataChunk currentChunk = uiChunkList.SelectedItem as PngDataChunk;
            if (currentChunk != null)
            {
                //tbRawViewHex.Text = BitConverter.ToString(currentChunk.GetBytes()).Replace("-", " ");
                //tbRawViewAscii.Text = ByteUtils.ParseAscii(currentChunk.GetBytes());
                spRawHex.Children.Clear();
                StringToChunks(spRawHex, BitConverter.ToString(currentChunk.GetBytes()).Replace("-", " "), 24);
                spRawAscii.Children.Clear();
                StringToChunks(spRawAscii, ByteUtils.ParseAscii(currentChunk.GetBytes()), 8);
                
            }
        }

        public static void StringToChunks(StackPanel panel, string str, int size)
        {
            for (int i = 0; i < str.Length; i += size)
            {
                TextBlock tb = new TextBlock();
                tb.Text = str.Substring(i, Math.Min(size, str.Length - i));
                tb.FontFamily = new FontFamily("Consolas");
                panel.Children.Add(tb);
            }
            //ret.Add(str.Substring(i, Math.Min(size, str.Length - i)));
        }

    }
}
