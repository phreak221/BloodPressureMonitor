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

namespace BPMonitor.Views
{
    /// <summary>
    /// Interaction logic for BPReportView.xaml
    /// </summary>
    public partial class BPReportView : UserControl
    {
        public BPReportView()
        {
            InitializeComponent();
        }

        private void OnReportLoaded (object sender, RoutedEventArgs e)
        {
            DpStartDate.Focus();
            DpStartDate.Focusable = true;
            Keyboard.Focus(DpStartDate);
        }
    }
}
