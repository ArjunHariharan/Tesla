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

namespace Tesla
{
    /// <summary>
    /// Interaction logic for AddressBookPage.xaml
    /// </summary>
    public partial class AddressBookPage : Page
    {
        public AddressBookPage()
        {
            InitializeComponent();

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string Query = "";
            if (CoreTeam.IsChecked == true)
            {
                Query = "select u.firstname,u.lastname,u.email,u.mobile " +
                        "from tesla.users u " +
                        "join tesla.incharge i on u.UserId = i.userid " +
                        "join tesla.events e on e.EventId = i.eventid";
            }
            else
            {
                Query = "select u.firstname,u.lastname,u.email,u.mobile " +
                            "from tesla.users u " +
                            "join tesla.incharge i on u.UserId = i.userid " +
                            "join tesla.segments s on s.id = i.segmentid";
            }
            
        }
    }
}
