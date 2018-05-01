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
    /// Interaction logic for CtrlShowTIME.xaml
    /// </summary>
    public partial class CtrlShowTIME : UserControl
    {
        public CtrlShowTIME()
        {
            InitializeComponent();
        }

        private void btnSetDateNow_Click(object sender, RoutedEventArgs e)
        {
            ParsedTIME timeChunk = this.DataContext as ParsedTIME;
            if (timeChunk != null) // It really shouldn't be, but might as well check
            {
                timeChunk.Time = DateTime.Now;
                this.DataContext = null;        // This is to update the view
                this.DataContext = timeChunk;   // There's probably a more elegant way but w/e this works
            }
        }
    }
}
