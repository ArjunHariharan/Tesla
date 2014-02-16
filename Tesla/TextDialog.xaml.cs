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

namespace Tesla
{
    /// <summary>
    /// Interaction logic for TextDialog.xaml
    /// </summary>
    public partial class TextDialog : Window
    {
        public TextDialog()
        {
            InitializeComponent();
        }

        private void Notes_GotFocus(object sender, RoutedEventArgs e)
        {
            Notes.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Notes.Text.CompareTo("Enter Desription (Max 300 letters)") == 0 || Notes.Text.Length == 0)
            {
                MessageBox.Show("Write Description");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
