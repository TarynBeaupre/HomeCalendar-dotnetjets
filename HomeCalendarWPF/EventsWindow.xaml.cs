using Calendar;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class EventsWindow : Window, EventsViewInterface
    {
        private int defaultCategoryIndex = 0;
        private EventsPresenter presenter;

        public EventsWindow(EventsPresenter p, bool darkmode)
        {
            this.presenter = p;
            InitializeComponent();
            startdp.SelectedDate = System.DateTime.Now;
            enddp.SelectedDate = System.DateTime.Now;
            categoriescmb.SelectedIndex = defaultCategoryIndex;
            List<string> categoriesList = new List<string>
            {
                "Homework", "Event", "Work", "Meeting"
            };
            categoriescmb.ItemsSource = categoriesList;
            txbCalendarFileinEvents.Text = ((MainWindow)Application.Current.MainWindow).calendarFiletxb.Text;
            SetDefaultTime();

            // Set the theme from the mainWindow
            SetTheme(darkmode);
        }

        public void AddNewCategory()
        {
            throw new NotImplementedException();
        }

        private void AddNewEvent()
        {
            //! Change for better UI implementation
            MessageBox.Show("Event successfully added!");
            this.Close();
        }

        private void Btn_Click_Add_Event(object sender, RoutedEventArgs e)
        {
            //Add the event to the database and the view calendar via the presenter
            string details = eventDescription.Text;
            int categoryId = categoriescmb.SelectedIndex;

            if (!startdp.SelectedDate.HasValue)
                startdp.SelectedDate = System.DateTime.Now;
            if (!enddp.SelectedDate.HasValue)
                enddp.SelectedDate = System.DateTime.Now;
            DateTime? start = startdp.SelectedDate;
            DateTime? end = enddp.SelectedDate;
            string fileName = "";
            AddNewEvent();
            //presenter.AddEvent(details, categoryId, start, end, fileName);

        }
        public void Btn_Click_Cancel_Event(object sender, EventArgs e)
        {
            // if user cancels addition, resent the default values for the category and the dates
            this.Close();
        }

        public void SetDefaultTime()
        {
            int startHour, endMinsIndex, endHour;
            DateTime date = System.DateTime.Now;

            List<string> hourList = new List<string> { };
            for (int i = 1; i <= 24; i++)
            {
                hourList.Add(i.ToString());
            }
            List<string> minList = new List<string> 
            {
                "00", "15", "30", "45"
            };

            cmbStartTimeHour.ItemsSource = hourList;
            cmbEndTimeHour.ItemsSource = hourList;


            cmbStartTimeMins.ItemsSource = minList; 
            if (date.Minute < 30)
            {
                startHour = date.Hour;
                endHour = startHour + 1;
                endMinsIndex = 2;
                cmbStartTimeMins.SelectedIndex = 2;
            }
            else
            {
                startHour = date.Hour + 1;
                endHour = startHour + 1;
                endMinsIndex = 0;
                cmbStartTimeMins.SelectedIndex = 0;
            }
            cmbStartTimeHour.SelectedIndex = startHour - 1;
            cmbEndTimeHour.SelectedIndex = endHour - 1;
            cmbEndTimeMins.ItemsSource = minList;
            cmbEndTimeMins.SelectedIndex = endMinsIndex;
        }
        private void SetTheme(bool darkmode)
        {
            if (darkmode)
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop-dark.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.Gray;
                light_theme_star.Visibility = Visibility.Collapsed;
                dark_theme_star.Visibility = Visibility.Visible;
            }
            else
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.LightGreen;
                light_theme_star.Visibility = Visibility.Visible;
                dark_theme_star.Visibility = Visibility.Collapsed;
            }
        }

        public void AddNewCategory()
        {
            throw new NotImplementedException();
        }

        public void AddNewEvent()
        {
            throw new NotImplementedException();
        }

        public void SetDefaults()
        {
            throw new NotImplementedException();
        }

        public void ShowError(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
