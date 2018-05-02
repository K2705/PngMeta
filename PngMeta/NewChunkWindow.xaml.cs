using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PngMeta
{
    /// <summary>
    /// Interaction logic for NewChunkWindow.xaml
    /// </summary>
    public partial class NewChunkWindow : Window
    {
        public ImageWrapper Image { get; set; }
        public NewChunkWindow()
        {
            InitializeComponent();
        }

        private void tbKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbKeyword.Text.Length > 0)
                btnSave.IsEnabled = true;
            else
                btnSave.IsEnabled = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Image.FileChunks.Insert(Image.FileChunks.Count - 1, ImageData.NewTextChunk(tbKeyword.Text, tbValue.Text)); // must be before IEND
            this.Close();
        }
    }
}
