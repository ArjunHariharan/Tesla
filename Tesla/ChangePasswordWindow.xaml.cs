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
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            String Query, pass;
            DatabaseQuery dbobj = new DatabaseQuery();
            if (OldPasswordBox.Password == "")
            {
                MessageBox.Show("Enter your old password");
                return;
            }
            if (NewPasswordBox.Password == "")
            {
                MessageBox.Show("Enter a new password");
                return;
            }
            if (ConfirmPasswordBox.Password == "Enter the new password to confirm")
            {
                MessageBox.Show("");
                return;
            }
            if (NewPasswordBox.Password.CompareTo(ConfirmPasswordBox.Password) != 0)
            {
                MessageBox.Show("The New password and Confirm Password doesn't match. Enter again");
                return;
            }

            Query = "Select password from tesla.login where userid = '" + MainWindow.UserData.UserId + "'";
            pass = dbobj.GetValue(Query);
            
            if (OldPasswordBox.Password.CompareTo(pass) == 0)
            {
                Query = "update tesla.login set password = '" + NewPasswordBox.Password +"' where userid = '" + MainWindow.UserData.UserId + "'";
                dbobj.UpdateValue(Query);
                MessageBox.Show("Password Changed");
                this.Close();
            }
            else
            {
                MessageBox.Show("Old Password is incorrect. Please try again");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
