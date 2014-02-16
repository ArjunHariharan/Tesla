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
using System.Data;

namespace Tesla
{
    /// <summary>
    /// Interaction logic for ParticipantListPage.xaml
    /// </summary>
    public partial class ParticipantListPage : Page
    {
        DataSet Events,Participant;
        DatabaseQuery Dbobj;
        public ParticipantListPage()
        {
            InitializeComponent();
            Events = new DataSet();
            Participant = new DataSet();
            Dbobj = new DatabaseQuery();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserData.role == 5)
            {
                ParticipantBalance.IsEnabled = false;
                EventListBox.Items.Add(MainWindow.UserData.eventname);
                FetchDataButton_Click(sender, e);
            }
            else
            {
                String Query = "SELECT distinct(eventname) FROM tesla.events";
                Dbobj.dbfetch(Query, Events);
                EventListBox.ItemsSource = Events.Tables[0].DefaultView;
                EventListBox.SelectedIndex = 0;
                EventListBox.IsReadOnly = false;

                if (MainWindow.UserData.role == 4)
                {
                    EventListBox.SelectedItem = MainWindow.UserData.eventname;
                    EventListBox.IsReadOnly = true;
                }
            }
        }

        private void FetchDataButton_Click(object sender, RoutedEventArgs e)
        {
            string Query="";
            if (ParticipantBalance.IsChecked == true)
            {
                Query = "select r.ParticipantName,r.mobile_no,r.email,r.Advance_Payment as Paid_Amount, " +
                "r.Balance_Payment as Balance from tesla.receipt r " +
                "join tesla.events e on e.EventId = r.EventId " +
                "where e.EventName = '"+ EventListBox.SelectedItem.ToString()+"'";
            }
            else
            {
                Query = "select r.ParticipantName,r.mobile_no,r.email " +
                "from tesla.receipt r " +
                "join tesla.events e on e.EventId = r.EventId " +
                "where e.EventName = '" + EventListBox.SelectedItem.ToString() + "'";
            }
            Dbobj.dbfetch(Query, Participant);
            ParticipantCountBox.Text = Participant.Tables[0].Rows.Count.ToString();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This facility will be availble very soon. Sorry for the inconvinience");
        }
    }
}
