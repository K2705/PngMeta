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
                myImage.Source = new BitmapImage(new Uri(dialog.FileName));

                fileBytes = File.ReadAllBytes(dialog.FileName);
                //myTextBox.Text = BitConverter.ToString(fileBytes).Replace("-", " ");
                //myTextBox.Text = Encoding.UTF8.GetString(fileBytes);

                fileChunks = BLImageData.GetChunks(fileBytes);
                myTextBox.Text = string.Join(" ", fileChunks.Select(x => x.Type.ToString()));
            }

        }
    }
}
