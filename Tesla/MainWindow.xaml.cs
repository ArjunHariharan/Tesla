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
    public partial class MainWindow : Window
    {
        public static User UserData;
        public static LoginWindow loginwindow;
        
        public MainWindow()
        {
            UserData = new User();
            UserData.name = "arjun";
            UserData.role = 1;
            UserData.UserId = 1;
            UserData.eventname = "";
            InitializeComponent();
            
        }

        private void EventButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultLabel.Visibility = System.Windows.Visibility.Hidden;
            UserFrame.Navigate(new Uri("EventPage.xaml", UriKind.Relative));
        }

        private void ReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultLabel.Visibility = System.Windows.Visibility.Hidden;
            UserFrame.Navigate(new Uri("ReceiptPage.xaml", UriKind.Relative));
        }

        private void BillButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature will be added shortly. Sorry for the inconvinience");
            DefaultLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ParticipantButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultLabel.Visibility = System.Windows.Visibility.Hidden;
            UserFrame.Navigate(new Uri("ParticipantListPage.xaml", UriKind.Relative));
        }

        private void FinanceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature will be added shortly. Sorry for the inconvinience");
            DefaultLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ContactButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Logout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            this.Hide();
            loginwindow.Show();
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            //disable features based on role
            userlabel.Content = UserData.name;

            switch (UserData.role)
            {
                case 1:
                    EventButton.IsEnabled = true;
                    ReceiptButton.IsEnabled = true;
                    BillButton.IsEnabled = true;
                    FinanceButton.IsEnabled = true;
                    AccountButton.IsEnabled = true;
                    break;
                case 2:
                    ReceiptButton.IsEnabled = true;
                    break;
                case 3:
                    FinanceButton.IsEnabled = true;
                    break;
                case 4:
                    BillButton.IsEnabled = true;
                    FinanceButton.IsEnabled = true;
                    break;
            }
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultLabel.Visibility = System.Windows.Visibility.Hidden;
            UserFrame.Navigate(new Uri("UserAccountPage.xaml", UriKind.Relative));
        }

        private void ChangePassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangePasswordWindow obj = new ChangePasswordWindow();
            obj.Show();
        }
    }
}
