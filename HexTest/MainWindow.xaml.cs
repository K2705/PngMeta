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

namespace HexTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BytesWrapper bytes = new BytesWrapper();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                bytes.Bytes = File.ReadAllBytes(dialog.FileName);
                //TextBoxHex.Text = bytes.BytesAsText;
                TextBoxText.Text = bytes.Text;
                icHex.ItemsSource = bytes.Bytes;
            }
            
        }
    }
}
