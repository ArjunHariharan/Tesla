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
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Tesla
{
    public partial class ReceiptPage : Page
    {
        private DataSet Receipts,Events,Colleges;
        private DatabaseQuery Dbobj;
        private int SizeOfReceiptBook,ReceiptFetchCount,BookNo,SelectRowIndex;
        private bool NewReceipt,updateReceipt;
        private List<viewclass> ReceiptDetails;

        public ReceiptPage()
        {
            InitializeComponent();
            Receipts = new DataSet();
            Events = new DataSet();
            Colleges = new DataSet();
            Dbobj = new DatabaseQuery();
            SizeOfReceiptBook = 50;
            ReceiptFetchCount = 0;
            BookNo = 0;
            NewReceipt = false;
            PaymentDate.DisplayDate = DateTime.Now;
            PaymentDate.DisplayDateEnd = DateTime.Now;
            ReceiptDetails = new List<viewclass>();
            updateReceipt = false;
            EditButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
        }

        private void ReceiptBookNoBox_LostFocus(object sender, RoutedEventArgs e)
        {
            int ReceiptNoBeginning;
            String Query;
            
            if (ReceiptBookNoBox.Text != "")
            {
                if (!int.TryParse(ReceiptBookNoBox.Text, out BookNo))
                {
                    MessageBox.Show("Invalid Receipt Book No.");
                    ReceiptBookNoBox.Text = "";
                    return;
                }
                else
                {
                    if (Receipts.HasChanges())
                    {
                        if (MessageBox.Show("Do you want to save Changes for previous Segment?",
                        "Save Events", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            Dbobj.UpdateData(Receipts);
                        }
                    }
                    ReceiptNoBeginning = ((BookNo - 1) * SizeOfReceiptBook) + 1;
                    Query = "Select * from tesla.receipt where receipt_no between "
                            + ReceiptNoBeginning + " and " + (ReceiptNoBeginning + SizeOfReceiptBook - 1);
                    Dbobj.dbfetch(Query, Receipts);

                    ReceiptFetchCount = Receipts.Tables[0].Rows.Count;

                    ReceiptViewGrid.ItemsSource = null;
                    ReceiptViewGrid.Items.Clear();
                    for (int i = 0; i < Receipts.Tables[0].Rows.Count;i++ )
                        AddRowsToList(i);

                    ReceiptViewGrid.ItemsSource = ReceiptDetails;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int ReceiptNoBeginning, recno;
            Boolean flag = true;

            Regex numeric = new Regex("[0-9]*$");
            Regex alpha = new Regex("^[a-zA-Z ]*$");

            if (!updateReceipt)
            {
                if (ReceiptBookNoBox.Text == "")
                {
                    MessageBox.Show("Enter receipt book no");
                    return;
                }
                if (ReceiptNoBox.Text == "")
                {
                    MessageBox.Show("Enter receipt no");
                    return;
                }
                if (!int.TryParse(ReceiptNoBox.Text, out recno))
                {
                    MessageBox.Show("Invalid receipt no. Enter again");
                    return;
                }
                ReceiptNoBeginning = ((BookNo - 1) * SizeOfReceiptBook) + 1;
                if (recno < ReceiptNoBeginning || recno > ReceiptNoBeginning + SizeOfReceiptBook - 1)
                {
                    MessageBox.Show("Receipt no doesn't belong to this Receipt Book No");
                    return;
                }
                for (int i = 0; i < Receipts.Tables[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(Receipts.Tables[0].Rows[i][0]) == recno)
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag)
                {
                    MessageBox.Show("Entry for this receipt no exists.");
                    return;
                }
            }
            if (NameBox.Text == "")
            {
                MessageBox.Show("Enter the participant's name");
                return;
            }
           
            if (!alpha.IsMatch(NameBox.Text))
            {
                MessageBox.Show("Participant Name invalid");
                return;
            }
            if (MobileBox.Text == "")
            {
                MessageBox.Show("Enter the participant's mobile no");
                return;
            }

            if (!numeric.IsMatch(MobileBox.Text) || MobileBox.Text.Length != 10)
            {
                MessageBox.Show("Invalid Mobile No. Enter again");
                return;
            }
            if (EmailBox.Text == "")
            {
                MessageBox.Show("Enter the participant's email id");
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

            if (IssueBox.Text == "")
            {
                MessageBox.Show("Issued By field cannot be empty.");
                return;
            }
            if (!alpha.IsMatch(IssueBox.Text))
            {
                MessageBox.Show("Issued by field invalid");
                return;
            }

            if (PaymentBox.Text == "")
            {
                MessageBox.Show("Enter the amount paid");
                return;
            }
            if (!numeric.IsMatch(PaymentBox.Text))
            {
                MessageBox.Show("Payment Invalid!");
                return;
            }
            if ((int)Events.Tables[0].Rows[EventList.SelectedIndex][2] - Convert.ToInt32(PaymentBox.Text) < 0)
            {
                MessageBox.Show("Event Fees = " + Events.Tables[0].Rows[EventList.SelectedIndex][2].ToString() + ". Entry invalid");
                return;
            }
            if (PaymentDate.SelectedDate.ToString() == "")
            {
                MessageBox.Show("Select a date");
                return;
            }
            if (updateReceipt)
            {
                var notes = new TextDialog();
                string bal_pay_date = "";
                if (Receipts.Tables[0].Rows[SelectRowIndex][10].ToString()!="")
                {
                    bal_pay_date = Convert.ToDateTime(Receipts.Tables[0].Rows[SelectRowIndex][10]).Date.ToString();
                }
                if (notes.ShowDialog() == true)
                {
                    string query = string.Format("INSERT INTO `tesla`.`receipt_backup` (`receiptno`, " +
                        "`participant name`, `mobile`, `email`, `collegeid`, `eventid`, `issuedby`, " +
                        "`advance_payment`, `balance_payment`, `advance_payment_date`, " +
                        "`balance_payment_date`, `entry_modified_date`, `cancel` ,`notes`, `userid`) " +
                        "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', " +
                        "'{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}' , '{14}');",
                        Receipts.Tables[0].Rows[SelectRowIndex][0], Receipts.Tables[0].Rows[SelectRowIndex][1],
                        Receipts.Tables[0].Rows[SelectRowIndex][2], Receipts.Tables[0].Rows[SelectRowIndex][3],
                        Receipts.Tables[0].Rows[SelectRowIndex][4], Receipts.Tables[0].Rows[SelectRowIndex][5],
                        Receipts.Tables[0].Rows[SelectRowIndex][6], Receipts.Tables[0].Rows[SelectRowIndex][7],
                        Receipts.Tables[0].Rows[SelectRowIndex][8],
                        Convert.ToDateTime(Receipts.Tables[0].Rows[SelectRowIndex][9]).ToShortDateString(),
                        bal_pay_date, Receipts.Tables[0].Rows[SelectRowIndex][11],
                        Receipts.Tables[0].Rows[SelectRowIndex][12], Receipts.Tables[0].Rows[SelectRowIndex][13],
                        Receipts.Tables[0].Rows[SelectRowIndex][14]);

                    Dbobj.UpdateValue(query);
                    
                    Receipts.Tables[0].Rows[SelectRowIndex][1] = NameBox.Text.ToUpper();
                    Receipts.Tables[0].Rows[SelectRowIndex][2] = MobileBox.Text;
                    Receipts.Tables[0].Rows[SelectRowIndex][3] = EmailBox.Text;
                    Receipts.Tables[0].Rows[SelectRowIndex][4] = (int)CollegeList.SelectedValue;
                    Receipts.Tables[0].Rows[SelectRowIndex][5] = (int)EventList.SelectedValue;
                    Receipts.Tables[0].Rows[SelectRowIndex][6] = IssueBox.Text.ToUpper(); ;
                    Receipts.Tables[0].Rows[SelectRowIndex][7] = Convert.ToInt32(PaymentBox.Text);
                    Receipts.Tables[0].Rows[SelectRowIndex][8] = (int)Events.Tables[0].Rows[EventList.SelectedIndex][2] - Convert.ToInt32(PaymentBox.Text);
                    Receipts.Tables[0].Rows[SelectRowIndex][9] = PaymentDate.SelectedDate;
                    Receipts.Tables[0].Rows[SelectRowIndex][11] = DateTime.Now;
                    Receipts.Tables[0].Rows[SelectRowIndex][13] = notes.Notes.Text;
                    Receipts.Tables[0].Rows[SelectRowIndex][14] = MainWindow.UserData.UserId;

                    ReceiptDetails[SelectRowIndex].Name = NameBox.Text.ToUpper();
                    ReceiptDetails[SelectRowIndex].Mobile = MobileBox.Text;
                    ReceiptDetails[SelectRowIndex].Email= EmailBox.Text;
                    ReceiptDetails[SelectRowIndex].College = GetCollegeName((int)CollegeList.SelectedValue);
                    ReceiptDetails[SelectRowIndex].Event = GetEventName((int)EventList.SelectedValue);
                    ReceiptDetails[SelectRowIndex].Issued = IssueBox.Text.ToUpper();
                    ReceiptDetails[SelectRowIndex].Advance = Convert.ToInt32(PaymentBox.Text);
                    ReceiptDetails[SelectRowIndex].AdvanceDate = PaymentDate.SelectedDate;
                    ReceiptDetails[SelectRowIndex].Balance = (int)Events.Tables[0].Rows[EventList.SelectedIndex][2] - Convert.ToInt32(PaymentBox.Text);

                    Dbobj.UpdateData(Receipts);
                    
                }
            }
            else
            {
                DataRow row = Receipts.Tables[0].NewRow();
                row["Receipt_no"] = Convert.ToInt32(ReceiptNoBox.Text);
                row["ParticipantName"] = NameBox.Text.ToUpper();
                row["Mobile_No"] = MobileBox.Text;
                row["Email"] = EmailBox.Text;
                row["CollegeId"] = (int)CollegeList.SelectedValue;
                row["EventId"] = (int)EventList.SelectedValue;
                row["Issued By"] = IssueBox.Text.ToUpper();
                row["Advance_Payment"] = Convert.ToInt32(PaymentBox.Text);
                row["Balance_Payment"] = (int)Events.Tables[0].Rows[EventList.SelectedIndex][2] - Convert.ToInt32(PaymentBox.Text);
                row["Advance_payment_date"] = PaymentDate.SelectedDate;
                row["EntryModifiedDate"] = DateTime.Now;
                row["Cancel"] = 'N';
                row["Notes"] = "";

                Receipts.Tables[0].Rows.Add(row);
                AddRowsToList(Receipts.Tables[0].Rows.Count - 1);

                ReceiptViewGrid.Items.Refresh();
            }

            AddButton.Content = "Add";
            ClearFieldButton.Content = "Clear Fields";

            ClearFieldButton_Click(sender, e);
        }

        private void ClearFieldButton_Click(object sender, RoutedEventArgs e)
        {
            if (updateReceipt)
            {
                updateReceipt = false;
                AddButton.Content = "Add";
                ClearFieldButton.Content = "Clear Fields";

                ReceiptNoBox.IsReadOnly = false;
                ReceiptBookNoBox.IsReadOnly = false;
            }
            ReceiptViewGrid.UnselectAll();
            CancelButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            ReceiptNoBox.Text = "";
            NameBox.Text = "";
            MobileBox.Text = "";
            EmailBox.Text = "";
            IssueBox.Text = "";
            PaymentBox.Text = "";
            CollegeList.SelectedIndex = 0;
            EventList.SelectedIndex = 0;
            PaymentDate.DisplayDate = DateTime.Now;
            PaymentDate.SelectedDate = null;
        }

        private void ReceiptViewGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!updateReceipt)
            { 
                SelectRowIndex = ReceiptViewGrid.SelectedIndex;
                if (SelectRowIndex > -1)
                {
                    if (Receipts.Tables[0].Rows[SelectRowIndex][12].ToString().CompareTo("N") == 0)
                    {
                        CancelButton.IsEnabled = true;
                        if (Receipts.Tables[0].Rows[SelectRowIndex][10].ToString() == "")
                            EditButton.IsEnabled = true;
                    }
                    else
                    {
                        CancelButton.IsEnabled = false;
                        EditButton.IsEnabled = false;
                    }
                }
                else
                {
                    CancelButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                }

                if (SelectRowIndex >= ReceiptFetchCount)
                {
                    CancelButton.Content = "Delete Receipt";
                    NewReceipt = true;
                }
                else
                {
                    CancelButton.Content = "Cancel Registration";
                    NewReceipt = false;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiptViewGrid.SelectedItem != null)
            {
                if (NewReceipt)
                {
                    Receipts.Tables[0].Rows.RemoveAt(SelectRowIndex);
                    ReceiptDetails.RemoveAt(SelectRowIndex);
                }
                else
                {
                    var notes = new TextDialog();
                    if (notes.ShowDialog() == true)
                    {
                        Receipts.Tables[0].Rows[SelectRowIndex][7] = 0;
                        Receipts.Tables[0].Rows[SelectRowIndex][8] = 0;
                        Receipts.Tables[0].Rows[SelectRowIndex][12] = 'Y';
                        Receipts.Tables[0].Rows[SelectRowIndex][13] = notes.Notes.Text;
                        Receipts.Tables[0].Rows[SelectRowIndex][14] = MainWindow.UserData.UserId;

                        ReceiptDetails[SelectRowIndex].Advance = Receipts.Tables[0].Rows[SelectRowIndex][7];
                        ReceiptDetails[SelectRowIndex].Balance = Receipts.Tables[0].Rows[SelectRowIndex][8];
                        ReceiptDetails[SelectRowIndex].Cancel = Receipts.Tables[0].Rows[SelectRowIndex][12];

                        MessageBox.Show("Receipt Cancelled");
                    }
                }
            }
            else
            {
                MessageBox.Show("No entry selected");
            }
        }

        private void ReceiptPage1_Loaded(object sender, RoutedEventArgs e)
        {
            String Query;

            Query = "Select * from Tesla.College order by collegeid asc";
            Dbobj.dbfetch(Query, Colleges);

            CollegeList.ItemsSource = Colleges.Tables[0].DefaultView;
            CollegeList.DisplayMemberPath = "Name";
            CollegeList.SelectedValuePath = "CollegeId";
            CollegeList.SelectedIndex = 0;

            Query = "SELECT Eventid, concat_ws('',eventname,'(',teamsize,',concession-' ,mitcoeconcession,')') "
                + "as EventName, Fees FROM tesla.events e join tesla.segments s on e.SegmentId = s.id "
                + "and s.HasEvents like 'Y' and e. Cancel like 'N' ";

            Dbobj.dbfetch(Query, Events);

            EventList.ItemsSource = Events.Tables[0].DefaultView;
            EventList.DisplayMemberPath = "EventName";
            EventList.SelectedValuePath = "Eventid";
            EventList.SelectedIndex = 0;
        }

        private string GetEventName(int i)
        {
            string eventname="";
            for (int j = 0; j < Events.Tables[0].Rows.Count; j++)
            {
                if (Convert.ToInt32(Events.Tables[0].Rows[j][0]) == i)
                {
                    eventname = Events.Tables[0].Rows[j][1].ToString();
                    break;
                }
            }
            return eventname;
        }

        private string GetCollegeName(int i)
        {
            string coll="";
            for (int j = 0; j < Colleges.Tables[0].Rows.Count; j++)
            {
                if (Convert.ToInt32(Colleges.Tables[0].Rows[j][0]) == i)
                {
                    coll = Colleges.Tables[0].Rows[j][1].ToString();
                    break;
                }
            }
            return coll;
        }

        private void AddRowsToList(int i)
        {
            object coll = "", eventname = "";
            object balancedate = "";

            eventname = GetEventName(Convert.ToInt32(Receipts.Tables[0].Rows[i][5]));
            coll = GetCollegeName(Convert.ToInt32(Receipts.Tables[0].Rows[i][4]));    
            

            if (Receipts.Tables[0].Rows[i][10].ToString() != "")
            { 
                balancedate = Convert.ToDateTime(Receipts.Tables[0].Rows[i][10]).Date;
            }

            //ReceiptViewGrid.Items.Add(new viewclass
            ReceiptDetails.Add(new viewclass
            {
                Receipt = Receipts.Tables[0].Rows[i][0],
                Name = Receipts.Tables[0].Rows[i][1],
                Mobile = Receipts.Tables[0].Rows[i][2],
                Email = Receipts.Tables[0].Rows[i][3],
                College = coll,
                Event = eventname,
                Issued = Receipts.Tables[0].Rows[i][6],
                Advance = Receipts.Tables[0].Rows[i][7],
                Balance = Receipts.Tables[0].Rows[i][8],
                AdvanceDate =Convert.ToDateTime(Receipts.Tables[0].Rows[i][9]).Date,
                BalanceDate = balancedate,
                Cancel = Receipts.Tables[0].Rows[i][12]
            });
        }

        private class viewclass : INotifyPropertyChanged
        {
            private object Receiptvalue;
            private object Namevalue;
            private object Mobilevalue;
            private object Emailvalue;
            private object Collegevalue;
            private object Eventvalue;
            private object Issuedvalue;
            private object Advancevalue;
            private object Balancevalue;
            private object AdvanceDatevalue;
            private object BalanceDatevalue;
            private object Cancelvalue;

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public object Receipt { 
                get
                {
                    return this.Receiptvalue;
                }
                set
                {
                    if (value != this.Receiptvalue)
                    {
                        this.Receiptvalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object Name
            {
                get
                {
                    return this.Namevalue;
                }
                set
                {
                    if (value != this.Namevalue)
                    {
                        this.Namevalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object Mobile
            {
                get
                {
                    return this.Mobilevalue;
                }
                set
                {
                    if (value != this.Mobilevalue)
                    {
                        this.Mobilevalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object Email
            {
                get
                {
                    return this.Emailvalue;
                }
                set
                {
                    if (value != this.Emailvalue)
                    {
                        this.Emailvalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object College
            {
                get
                {
                    return this.Collegevalue;
                }
                set
                {
                    if (value != this.Collegevalue)
                    {
                        this.Collegevalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object Event
            {
                get
                {
                    return this.Eventvalue;
                }
                set
                {
                    if (value != this.Eventvalue)
                    {
                        this.Eventvalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object Issued
            {
                get
                {
                    return this.Issuedvalue;
                }
                set
                {
                    if (value != this.Issuedvalue)
                    {
                        this.Issuedvalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object Advance
            {
                get
                {
                    return this.Advancevalue;
                }
                set
                {
                    if (value != this.Advancevalue)
                    {
                        this.Advancevalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object Balance
            {
                get
                {
                    return this.Balancevalue;
                }
                set
                {
                    if (value != this.Balancevalue)
                    {
                        this.Balancevalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object AdvanceDate
            {
                get
                {
                    return this.AdvanceDatevalue;
                }
                set
                {
                    if (value != this.AdvanceDatevalue)
                    {
                        this.AdvanceDatevalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object BalanceDate
            {
                get
                {
                    return this.BalanceDatevalue;
                }
                set
                {
                    if (value != this.BalanceDatevalue)
                    {
                        this.BalanceDatevalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public object Cancel
            {
                get
                {
                    return this.Cancelvalue;
                }
                set
                {
                    if (value != this.Cancelvalue)
                    {
                        this.Cancelvalue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SelectRowIndex = ReceiptViewGrid.SelectedIndex;
            updateReceipt = true;
            ReceiptViewGrid.UnselectAll();
            //only if the balance isn't payed. if the balance is payed, that means that the account has been closed
            // no more edits can be done.
            AddButton.Content = "Save Changes";
            ClearFieldButton.Content = "Undo/Cancel Changes";

            if (ReceiptNoBox.Text.CompareTo("") != 0)
            {
                if (MessageBox.Show("Do you want to add/save the current event?",
                "Save Events", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    AddButton_Click(sender, e);
                }
            }
            else
            {
                Addfields(SelectRowIndex);
            }
        }

        private void Addfields(int index)
        {
            DataRow row = Receipts.Tables[0].Rows[index];

            ReceiptNoBox.Text = row["Receipt_no"].ToString();
            NameBox.Text = row["ParticipantName"].ToString();
            MobileBox.Text = row["Mobile_No"].ToString() ;
            EmailBox.Text =row["Email"].ToString();
            CollegeList.SelectedValue = (int) row["CollegeId"];
            EventList.SelectedValue = (int) row["EventId"];
            IssueBox.Text= row["Issued By"].ToString();
            PaymentBox.Text = row["Advance_Payment"].ToString();
            PaymentDate.SelectedDate =Convert.ToDateTime(row["Advance_payment_date"]);

            ReceiptNoBox.IsReadOnly = true;
            ReceiptBookNoBox.IsReadOnly = true;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Receipts.HasChanges())
            {
                Dbobj.UpdateData(Receipts);
            }
            MessageBox.Show("Saved");
        }

        private void BalancePaidButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiptViewGrid.SelectedItem != null)
            {
                var balancedate = new BalancePaidDateWindow();
                
                if (balancedate.ShowDialog() == true)
                {
                    Receipts.Tables[0].Rows[SelectRowIndex][7] = Convert.ToInt32(Receipts.Tables[0].Rows[SelectRowIndex][7])
                        + Convert.ToInt32(Receipts.Tables[0].Rows[SelectRowIndex][8]);
                    Receipts.Tables[0].Rows[SelectRowIndex][8] = 0;
                    Receipts.Tables[0].Rows[SelectRowIndex][10] = balancedate.PaidDate;

                    ReceiptDetails[SelectRowIndex].Advance = Receipts.Tables[0].Rows[SelectRowIndex][7];
                    ReceiptDetails[SelectRowIndex].Balance = Receipts.Tables[0].Rows[SelectRowIndex][8];
                    ReceiptDetails[SelectRowIndex].BalanceDate = Receipts.Tables[0].Rows[SelectRowIndex][10];
                }
            }
            else
            {
                MessageBox.Show("Select an entry");
            }
            ReceiptViewGrid.UnselectAll();
        }

        private void ReceiptBookNoBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void ReceiptNoBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void NameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void MobileBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void EmailBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void CollegeList_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void EventList_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void IssueBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void PaymentBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

        private void PaymentDate_GotFocus(object sender, RoutedEventArgs e)
        {
            ReceiptViewGrid.UnselectAll();
        }

    }
}
