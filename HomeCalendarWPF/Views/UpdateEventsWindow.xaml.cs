using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using HomeCalendarWPF.Presenters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
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

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for UpdateEventsWindow.xaml
    /// </summary>
    public partial class UpdateEventsWindow : Window, UpdateEventsWindowInterface
    {
        readonly private UpdateEventsWindowPresenter presenter;
        readonly private string dbPath;
        readonly private CalendarItem eventToUpdate;
        //public static int previousCategoryIndex = 0;

        public UpdateEventsWindow(HomeCalendar model, string filePath, CalendarItem eventToUpdate)
        {
            InitializeComponent();
            this.eventToUpdate = eventToUpdate;
            this.dbPath = filePath;
            presenter = new UpdateEventsWindowPresenter(this, model);
            this.SetTheme(MainWindow.darkMode);
        }

        #region Event Handlers
        private void Btn_Click_AddNewCategory(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Btn_Click_UpdateEvent(object sender, RoutedEventArgs e)
        {
            presenter.UpdateEvent(eventToUpdate.EventID, startdp, categoriescmb.SelectedIndex, txbDuration, txbEventDescription.Text, cmbStartTimeHour, cmbStartTimeMins);
            this.Close();
        }
        private void Btn_Click_CancelUpdate(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Interface Methods
        public void ShowDefaultCategories(List<Category> categoriesList)
        {
            categoriescmb.ItemsSource = categoriesList;
        }
        public void PopulateFields()
        {
            txbEventDescription.Text = eventToUpdate.ShortDescription;
            txbCalendarFileinEvents.Text = dbPath;
            categoriescmb.SelectedIndex = eventToUpdate.CategoryID - 1;

            startdp.SelectedDate = eventToUpdate.StartDateTime;
            cmbStartTimeHour.SelectedIndex = eventToUpdate.StartDateTime.Hour - 1;
            cmbStartTimeMins.SelectedIndex = eventToUpdate.StartDateTime.Minute / 15;
            txbDuration.Text = eventToUpdate.DurationInMinutes.ToString();
        }
        public void ShowDefaultDateTime()
        {
            //Creating a drop down for 24 hour selection
            List<string> hourList = new List<string> { };
            for (int i = 1; i <= 24; i++)
            {
                hourList.Add(i.ToString());
            }
            cmbStartTimeHour.ItemsSource = hourList;

            //Creating a drop down for the start time minutes
            List<string> minList = new List<string>
            {
                "00", "15", "30", "45"
            };
            cmbStartTimeMins.ItemsSource = minList;

            //=== Set start time default (the next 30 min block) ===
            int startHour;
            DateTime date = System.DateTime.Now;
            if (date.Minute < 30)
            {
                startHour = date.Hour;
                cmbStartTimeMins.SelectedIndex = 2;
            }
            else
            {
                startHour = date.Hour + 1;
                cmbStartTimeMins.SelectedIndex = 0;
            }
            cmbStartTimeHour.SelectedIndex = startHour - 1; //-1 because it index 0 in the cmb is hour 1 of the day

            //=== Set default duration (30 mins) ===
            txbDuration.Text = "30";
        }
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region Private Methods
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
        
        #endregion
    }
}
