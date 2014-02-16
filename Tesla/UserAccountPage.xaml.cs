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
using System.Text.RegularExpressions;
using System.Data;

namespace Tesla
{
    /// <summary>
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class NewUser : Page
    {
        DatabaseQuery Dbobj;
        private DataSet Users,Event;

        public NewUser()
        {
            InitializeComponent();
            Dbobj = new DatabaseQuery();
            Users = new DataSet();
            Event = new DataSet();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            Regex alpha = new Regex("^[a-zA-Z]*$");
            Regex numeric = new Regex("[0-9]*$");
            int role = -1;
            string rl = "";
            object[] result;

            if(FirstNameBox.Text == "")
            {
                MessageBox.Show("Enter First Name");
                return;
            }

            if (!alpha.IsMatch(FirstNameBox.Text))
            {
                MessageBox.Show("First Name Invalid");
                return;
            }

            if (!alpha.IsMatch(LastNameBox.Text))
            {
                MessageBox.Show("Last Name Invalid");
                return;
            }
            if (LastNameBox.Text == "")
            {
                MessageBox.Show("Enter Last Name");
                return;
            }

            if (MobileBox.Text == "")
            {
                MessageBox.Show("Enter Mobile no");
                return;
            }


            if (!numeric.IsMatch(MobileBox.Text))
            {
                MessageBox.Show("Invalid Mobile number");
                return;
            }
            else
            {
                if (MobileBox.Text.Length != 10)
                {
                    MessageBox.Show("Mobile Number incorrect.");
                    return;
                }

            }
            if (EmailBox.Text == "")
            {
                MessageBox.Show("Enter Email id");
                return;
            }
            try
            {
                var mail = new System.Net.Mail.MailAddress(EmailBox.Text);
            }
            catch
            {
                MessageBox.Show("Invalid Email. Enter Again");
                return;
            }
            foreach (RadioButton r in RolePanel.Children)
            {
                if (r.IsChecked== true)
                {
                    role =Convert.ToInt32(r.Tag);
                    rl = r.Content.ToString();
                    break;
                }
            }

            if (role == -1)
            {
                MessageBox.Show("Select a role");
                return;
            }

            result = Dbobj.CreateUser(FirstNameBox.Text, LastNameBox.Text, MobileBox.Text, EmailBox.Text, role);

            if (result[0]. ToString().CompareTo("1") == 0)
            {
                UserGrid.Items.Add(new UserGridData
                {
                    Fname = FirstNameBox.Text,
                    Lname = LastNameBox.Text,
                    Mobile = MobileBox.Text,
                    Email = EmailBox.Text,
                    Role = rl,
                    UserName = result[1].ToString(),
                    Password = result[2].ToString()

                });
            }
            else
            {
                MessageBox.Show("An Account Already Exists for this user.");
            }

            ClearButton_Click(sender, e);
        }

        private struct UserGridData
        {
            public string Fname { set; get; }
            public string Lname { set; get; }
            public string Mobile { set; get; }
            public string Email { set; get; }
            public string Role { set; get; }
            public string UserName { set; get; }
            public string Password { set; get; }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FirstNameBox.Text = "";
            LastNameBox.Text = "";
            EmailBox.Text = "";
            MobileBox.Text = "";

            foreach (RadioButton r in RolePanel.Children)
            {
                r.IsChecked = false;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Regex alpha = new Regex("^[a-zA-Z ]*$");
            if (!alpha.IsMatch(NameBox.Text))
            {
                MessageBox.Show("Invalid Name");
                return;
            }

            String Query = "SELECT u.userid,concat_ws(' ',u.firstname,u.lastname) as name, l.role "
                           + "FROM tesla.users u "
                           + "join tesla.login l on l.userid = u.UserId "
                           + "where concat_ws(' ',u.firstname,u.lastname) like '%" + NameBox.Text + "%' "
                           + "and ( u.role = 3 or u.role = 5)"
                           + "order by concat_ws(' ',u.firstname,u.lastname) asc";

            Dbobj.dbfetch(Query, Users);

            NameListBox.ItemsSource = Users.Tables[0].DefaultView;
            NameListBox.DisplayMemberPath = "name";
        }

        private void NameListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = NameListBox.SelectedIndex;
            String Query = "";

            if (Convert.ToInt32( Users.Tables[0].Rows[index][2]) == 3)
            {
                Query = "Select * ";
            }

        }
    }
}
