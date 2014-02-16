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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>

    public partial class LoginWindow : Window
    {
        private static MainWindow mainwindow;
        
        public LoginWindow()
        {
            InitializeComponent();
            mainwindow = new MainWindow();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseQuery dbobj = new DatabaseQuery();
            User u;
            if (UsernameTextbox.Text == "")
            {
                MessageBox.Show("Enter Username");
                return;
            }

            if (PasswordBox.Password == "")
            {
                MessageBox.Show("Enter Password");
                return;
            }

            u = dbobj.Password_verify(UsernameTextbox.Text, PasswordBox.Password);

            if (u.name == "")
            {
                MessageBox.Show("Invalid Username. Try Again");
            }
            else
            {
                CallMainWindow(u);
            }

        }

        private void CallMainWindow(User u)
        {
            this.Hide();
            MainWindow.loginwindow = this;
            MainWindow.UserData = u;
            mainwindow.Show();   
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameTextbox.Focus();
        }
    }
}
