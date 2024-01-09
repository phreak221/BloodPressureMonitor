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
    /// Interaction logic for EditReading.xaml
    /// </summary>
    public partial class EditReading : UserControl
    {
        public EditReading()
        {
           InitializeComponent();
        }

        private void OnEditLoaded(object sender, RoutedEventArgs e)
        {
            txtSystolic.Focus();
            txtSystolic.Focusable = true;
            Keyboard.Focus(txtSystolic);
            var element = Keyboard.FocusedElement;
        }

        private void KeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;
            if (source != null)
            {
                source.Background = Brushes.LightBlue;
                source.SelectAll();
            }
        }

        private void KeyboardLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;
            if (source != null)
            {
                source.Background = Brushes.White;
            }
        }
    }
}
