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
using System.Web.UI.WebControls;

namespace Tesla
{
    public partial class EventPage : Page
    {
        private static DatabaseQuery Dbobj;
        private static DataSet Segment;
        private static DataSet Events;
        private static bool UpdateEvent, NewEvent;
        private static int EventCount, SelectRowIndex;
        private static int PreviousSegment;

        public EventPage()
        {
            InitializeComponent();
            Dbobj = new DatabaseQuery();
            UpdateEvent = false;
            Segment = new DataSet();
            Events = new DataSet();
            NewEvent = false;
            PreviousSegment = -1;
        }

        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            PreviousSegment = (int) SegmentList.SelectedIndex;
            SelectRowIndex = EventViewGrid.SelectedIndex;
           
            if (EventViewGrid.SelectedItem != null)
            {
                UpdateEvent = true;
                AddEventButton.Content = "Save Changes";
                ClearButton.Content = "Undo/Cancel Changes";

                if (EventName.Text.CompareTo("") != 0)
                {
                    if (MessageBox.Show("Do you want to add/save the current event?",
                    "Save Events", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        AddEvent_Click(sender, e);
                    }
                }
                else
                {
                    DataRowView dgrow = (DataRowView)EventViewGrid.SelectedItems[0];
                    Addfields(dgrow);
                }
            }
            else
            {
                MessageBox.Show("Select an event to edit");
            }
            EventViewGrid.UnselectAll();
        }

        private void EventWindow_Loaded(object sender, RoutedEventArgs e)
        {
            String Query = "SELECT * FROM tesla.segments where hasevents like 'Y'";
            Dbobj.dbfetch(Query, Segment);

            SegmentList.ItemsSource = Segment.Tables[0].DefaultView;
            SegmentList.DisplayMemberPath = "name";
            SegmentList.SelectedValuePath = "id";
            SegmentList.SelectedIndex = 0;

        }

        private void SegmentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!UpdateEvent)
            {
                if (Events.HasChanges())
                {
                    if (MessageBox.Show("Do you want to save Changes for previous Segment?",
                        "Save Events", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        Dbobj.UpdateData(Events);
                    }
                }
                EventViewGridRefresh((int)SegmentList.SelectedValue);
            }
        }

        private void EventViewGridRefresh(int DomainNo)
        {
            Events.Clear();
            String Query = "Select EventName,TeamSize,Fees,MitcoeConcession,DomainId,cancel,EventId " +
                   "from tesla.events where DomainId =" + DomainNo;
            Dbobj.dbfetch(Query, Events);

            EventViewGrid.ItemsSource = null;
            EventViewGrid.Items.Clear();

            EventViewGrid.ItemsSource = Events.Tables[0].DefaultView;
            EventViewGrid.Columns[4].Visibility = System.Windows.Visibility.Hidden;
            EventViewGrid.Columns[6].Visibility = System.Windows.Visibility.Hidden;
            EventViewGrid.AutoGenerateColumns = true;
            

            EventCount = Events.Tables[0].Rows.Count;
        }

        private void SaveEvent_Click(object sender, RoutedEventArgs e)
        {
            EventViewGrid.UnselectAll();
            if (Events.HasChanges())
            {
                Dbobj.UpdateData(Events);
                MessageBox.Show("Changes Saved");
            }
            else
            {
                MessageBox.Show("No Changes to be saved");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateEvent)
            {
                SegmentList.SelectedIndex = PreviousSegment;
                EventViewGridRefresh((int)SegmentList.SelectedValue);
                UpdateEvent = false;
                AddEventButton.Content = "Add Event";
                ClearButton.Content = "Clear Fields";
            }

            EventViewGrid.UnselectAll(); 
            EventName.Text = "";
            Fee.Text = "";
            TeamSize.Text = "";
            Mitcoe.IsChecked = false;
        }

        private void Addfields(DataRowView Dgrow)
        {
            EventName.Text = Dgrow.Row.ItemArray[0].ToString();
            TeamSize.Text = Dgrow.Row.ItemArray[1].ToString();
            Fee.Text = Dgrow.Row.ItemArray[2].ToString();
            if (Dgrow.Row.ItemArray[3].ToString().CompareTo("Y") == 0)
            {
                Mitcoe.IsChecked = true;
            }
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            int fee, size;
            char mitcoeconcession;

            if (EventName.Text == "")
            {
                MessageBox.Show("Enter Event Name");
                return;
            }
            if (!int.TryParse(TeamSize.Text, out size))
            {
                MessageBox.Show("Invalid entry for the field Team Size");
                return;
            }
            if (!int.TryParse(Fee.Text, out fee))
            {
                MessageBox.Show("Invalid entry for the field fees");
                return;
            }

            if (Mitcoe.IsChecked == true)
            {
                mitcoeconcession = 'Y';
            }
            else
            {
                mitcoeconcession = 'N';
            }
            
            if (UpdateEvent)
            {
                Events.Tables[0].Rows[SelectRowIndex]["eventname"] = EventName.Text.ToUpper(); ;
                Events.Tables[0].Rows[SelectRowIndex]["TeamSize"] = size;
                Events.Tables[0].Rows[SelectRowIndex]["Fees"] = fee;
                Events.Tables[0].Rows[SelectRowIndex]["MitcoeConcession"] = mitcoeconcession;
                Events.Tables[0].Rows[SelectRowIndex]["DomainId"] = (int)SegmentList.SelectedValue;
                Dbobj.UpdateData(Events);

                MessageBox.Show("Changes Saved");

                EventViewGridRefresh((int)SegmentList.SelectedValue);

            }
            else
            {

                if (!VerifyNewEntry(EventName.Text.ToUpper(),size,fee,mitcoeconcession))
                {
                    MessageBox.Show("This event exists.");
                    return;
                }

                DataRow row = Events.Tables[0].NewRow();
                row["eventname"] = EventName.Text.ToUpper();
                row["TeamSize"] = size;
                row["Fees"] = fee;
                row["MitcoeConcession"] = mitcoeconcession;
                row["DomainId"] = (int)SegmentList.SelectedValue;
                row["cancel"] = 'N';
                Events.Tables[0].Rows.Add(row);
            }
            EventViewGrid.Items.Refresh();

            UpdateEvent = false;
            AddEventButton.Content = "Add Event";
            ClearButton.Content = "Clear Fields";

            EventViewGrid.UnselectAll(); 
            Clear_Click(sender, e);
        }

        private bool VerifyNewEntry(string eventname,int size,int fee,char mitcoeconcession)
        {
            for (int i = 0; i < Events.Tables[0].Rows.Count; i++)
            {
                if (Events.Tables[0].Rows[i]["eventname"].ToString().CompareTo(EventName.Text.ToUpper()) == 0)
                {
                    if (Convert.ToInt32(Events.Tables[0].Rows[i]["TeamSize"])==size &&
                        Convert.ToInt32(Events.Tables[0].Rows[i]["Fees"]) == fee &&
                        Convert.ToChar(Events.Tables[0].Rows[i]["MitcoeConcession"])== mitcoeconcession)
                    {
                        return (false);
                    }
                }
            }
            return (true);
        }
        
        private void EventViewGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!UpdateEvent)
            {
                SelectRowIndex = EventViewGrid.SelectedIndex;
                if (SelectRowIndex > -1)
                {
                    if (Events.Tables[0].Rows[SelectRowIndex][5].ToString().CompareTo("Y") == 0)
                    {
                        RestoreEvent.IsEnabled = true;
                    }
                    else
                    {
                        RestoreEvent.IsEnabled = false;
                    }
                }
                else
                {
                    RestoreEvent.IsEnabled = false;
                }

                if (SelectRowIndex >= EventCount)
                {
                    CancelEvent.Content = "Delete Event";
                    NewEvent = true;
                }
                else
                {
                    CancelEvent.Content = "Cancel Event";
                    NewEvent = false;
                }
            }
        }

        private void RestoreEvent_Click(object sender, RoutedEventArgs e)
        {
             Events.Tables[0].Rows[SelectRowIndex][5] = 'N';
             MessageBox.Show("Event Restored");
        }

        private void CancelEvent_Click_1(object sender, RoutedEventArgs e)
        {
            if (EventViewGrid.SelectedItem != null)
            {
                if (NewEvent)
                {
                    Events.Tables[0].Rows.RemoveAt(SelectRowIndex);
                }
                else
                {
                    Events.Tables[0].Rows[SelectRowIndex][5] = 'Y';
                    MessageBox.Show("Event Cancelled");
                }
            }
            else
            {
                MessageBox.Show("No event selected");
            }
            EventViewGrid.UnselectAll();

        }

        private void EventPage_Loaded(object sender, RoutedEventArgs e)
        {
            String Query = "SELECT * FROM tesla.segments where hasevents like 'Y'";
            Dbobj.dbfetch(Query, Segment);

            SegmentList.ItemsSource = Segment.Tables[0].DefaultView;
            SegmentList.DisplayMemberPath = "name";
            SegmentList.SelectedValuePath = "id";
            SegmentList.SelectedIndex = 0;

            RestoreEvent.IsEnabled = false;
        }

        private void EventPage1_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Events.HasChanges())
            {
                if (MessageBox.Show("Do you want to save Changes for previous Segment?",
                    "Save Events", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Dbobj.UpdateData(Events);
                }
            }
        }

        private void EventName_GotMouseCapture(object sender, MouseEventArgs e)
        {
            EventViewGrid.UnselectAll(); 
        }

        private void TeamSize_GotMouseCapture(object sender, MouseEventArgs e)
        {
            EventViewGrid.UnselectAll(); 
        }

        private void Fee_GotMouseCapture(object sender, MouseEventArgs e)
        {
            EventViewGrid.UnselectAll(); 
        }

        private void Mitcoe_GotMouseCapture(object sender, MouseEventArgs e)
        {
            EventViewGrid.UnselectAll(); 
        }

        private void SegmentList_GotMouseCapture(object sender, MouseEventArgs e)
        {
            EventViewGrid.UnselectAll(); 
        }

    }
}