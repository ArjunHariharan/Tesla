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
    /// Interaction logic for BalancePaidDateWindow.xaml
    /// </summary>
    public partial class BalancePaidDateWindow : Window
    {
        public BalancePaidDateWindow()
        {
            InitializeComponent();
            DatePicker.DisplayDate = DateTime.Now;
            DatePicker.DisplayDateEnd = DateTime.Now.Date;
        }

        public DateTime PaidDate
        {
            get { return DatePicker.DisplayDate; }
            //set { ResponseTextBox.Text = value; }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate.ToString() == "")
            {
                MessageBox.Show("Select a date");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
