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

        private void btnOpen_Click(object sender, RoutedEventArgs e)
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

                //ColumnDefinition column = new ColumnDefinition();
                //column.MinWidth = 300;
                //grChunkContents.ColumnDefinitions.Add(column);
                //column = new ColumnDefinition();
                //column.MinWidth = 100;
                //grChunkContents.ColumnDefinitions.Add(column);
                //StackPanel spLeft = new StackPanel();
                //StackPanel spRight = new StackPanel();
                //grChunkContents.Children.Add(spLeft);
                //grChunkContents.Children.Add(spRight);

                //tbChunkName.Text = currentChunk.Type.ToString();
                //tbChunkAttribs.Text = currentChunk.Type.Ancillary + " " + currentChunk.Type.Private + " "
                //    + currentChunk.Type.Reserved + " " + currentChunk.Type.SafeToCopy;
                
                switch (currentChunk.Type.ToString())
                {
                    case "IHDR":
                        CtrlShowIHDR headerControl = new CtrlShowIHDR();
                        headerControl.HeaderData = currentChunk.ParsedData as ParsedIHDR;
                        headerControl.UpdateView();
                        tabChunkContents.Content = headerControl;
                        break;
                    case "gAMA":
                        CtrlShowGAMA gammaControl = new CtrlShowGAMA();
                        gammaControl.GammaData = currentChunk.ParsedData as ParsedGAMA;
                        gammaControl.UpdateView();
                        tabChunkContents.Content = gammaControl;
                        break;
                    case "tEXt":
                        CtrlShowTEXT textControl = new CtrlShowTEXT();
                        textControl.DataContext = currentChunk.ParsedData as ParsedTEXT;
                        tabChunkContents.Content = textControl;
                        break;
                    case "iTXt":
                        CtrlShowITXT itextControl = new CtrlShowITXT();
                        itextControl.DataContext = currentChunk.ParsedData as ParsedITXT;
                        tabChunkContents.Content = itextControl;
                        break;

                    default:
                        tabChunkContents.Content = new TextBlock { Text = "Data cannot be read." };
                        break;
                }

                tbRawHex.Text = SplitStringIntoLines(BitConverter.ToString(currentChunk.GetBytes()).Replace("-", " "), 24);
                tbRawAscii.Text = SplitStringIntoLines(ByteUtils.ParseAscii(currentChunk.GetBytes()), 8);

                //spRawHex.Children.Clear();
                //DrawHexView(spRawHex, BitConverter.ToString(currentChunk.GetBytes()).Replace("-", " "), 24);
                //spRawAscii.Children.Clear();
                //DrawHexView(spRawAscii, ByteUtils.ParseAscii(currentChunk.GetBytes()), 8);

            }
        }

        //public static void DrawHexView(StackPanel panel, string str, int size)
        //{
        //    for (int i = 0; i < str.Length; i += size)
        //    {
        //        TextBox tb = new TextBox();
        //        tb.IsReadOnly = true;
        //        tb.FontFamily = new FontFamily("Consolas");
        //        tb.Text = str.Substring(i, Math.Min(size, str.Length - i));
        //        panel.Children.Add(tb);
        //    }
        //    //ret.Add(str.Substring(i, Math.Min(size, str.Length - i)));
        //}

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            byte[] newBuffer = imageWrapper.GetImageBuffer();
            if (imageWrapper.FileBytes.SequenceEqual(newBuffer))
            {
                MessageBox.Show("buffer has not been modified");
            }
            else
            {
                MessageBox.Show("buffer has been modified");
            }
        }
    }
}
