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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PngMeta
{
    /// <summary>
    /// Interaction logic for CtrlShowIHDR.xaml
    /// </summary>
    public partial class CtrlShowIHDR : UserControl
    {
        public ParsedIHDR HeaderData { get; set; }

        public CtrlShowIHDR()
        {
            InitializeComponent();
        }

        public void UpdateView()
        {
            tbWidth.Text = HeaderData.Width.ToString();
            tbHeight.Text = HeaderData.Height.ToString();
            tbBitDepth.Text = HeaderData.BitDepth.ToString();
        }
    }
}
