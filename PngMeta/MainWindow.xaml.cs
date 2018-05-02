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
                try
                {
                    imageWrapper.LoadImage(dialog.FileName);
                }
                catch (Exception ex)    // Expected causes of exception: File doesn't exist, no permissions to read file, file otherwise inaccessible,
                {                       // something wonky happened while reading it, is not a png image, is a png image but corrupted and/or broken
                    MessageBox.Show("Could not load image:\n" + ex.Message);
                    return;
                }
                
                uiPreviewImage.Source = imageWrapper.Image;
                uiChunkList.ItemsSource = imageWrapper.FileChunks;
                btnSave.IsEnabled = true;
                uiChunkListTitle.Text = imageWrapper.FileChunks.Count + " chunks read";
                tabChunkContents.Content = null;
            }
            
        }

        private void uiChunkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PngDataChunk currentChunk = uiChunkList.SelectedItem as PngDataChunk;
            if (currentChunk != null)
            {
                
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
                    case "tIME":
                        CtrlShowTIME timeControl = new CtrlShowTIME();
                        timeControl.DataContext = currentChunk.ParsedData as ParsedTIME;
                        tabChunkContents.Content = timeControl;
                        break;

                    default:
                        tabChunkContents.Content = new TextBlock { Text = "Data cannot be displayed." };
                        break;
                }

                tbRawHex.Text = SplitStringIntoLines(BitConverter.ToString(currentChunk.GetBytes()).Replace("-", " "), 24);
                tbRawAscii.Text = SplitStringIntoLines(ByteUtils.ParseAscii(currentChunk.GetBytes()), 8);

            }
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            byte[] newBuffer = imageWrapper.GetImageBuffer();
            if (imageWrapper.FileBytes.SequenceEqual(newBuffer))
            {
                MessageBox.Show("buffer has not been modified");
            }
            else
            {
                Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                dialog.Filter = "PNG images|*.png";
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        imageWrapper.SaveImage(dialog.FileName, newBuffer);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving image:\n" + ex.Message);
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NewChunkWindow window = new NewChunkWindow();
            window.Image = imageWrapper;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //TODO not implemented
        }
    }
}
