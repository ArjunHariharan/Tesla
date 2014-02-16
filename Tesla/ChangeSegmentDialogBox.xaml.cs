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
using System.Data;
using System.Web.UI.WebControls;

namespace Tesla
{
    /// <summary>
    /// Interaction logic for ChangeSegmentDialogBox.xaml
    /// </summary>
    public partial class ChangeSegmentDialogBox : Window
    {
        public static DataSet Segment;
        private int NewSegment;
        
        public ChangeSegmentDialogBox()
        {
            InitializeComponent();
            NewSegment = -1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SegmentList.ItemsSource = Segment.Tables[0].DefaultView;
            SegmentList.DisplayMemberPath = "name";
            SegmentList.SelectedValuePath = "id";
            SegmentList.SelectedIndex = 0;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
           // EventPage.SegmentChange = NewSegment;
            this.Close();
        }

        private void SegmentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewSegment = (int) SegmentList.SelectedValue;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
