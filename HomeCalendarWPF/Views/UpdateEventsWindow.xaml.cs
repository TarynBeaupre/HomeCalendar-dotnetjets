﻿using Calendar;
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
        public static int previousCategoryIndex = 0;

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
            CategoriesWindow cw = new CategoriesWindow(presenter.model, MainWindow.darkMode);
            cw.ShowDialog();

            previousCategoryIndex = -1;
            presenter.GetDefaultCategories();
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
        /// <summary>
        /// Displays the default categories in the categories dropdown list.
        /// </summary>
        /// <param name="categoriesList">The list of default categories to display.</param>
        /// <example>
        /// <code>
        /// For this example, assume we have implemented a categoriesList
        /// <![CDATA[
        /// ShowDefaultCategories(categoriesList);
        /// ]]></code></example>
        public void ShowDefaultCategories(List<Category> categoriesList)
        {
            categoriescmb.ItemsSource = categoriesList;

            if (previousCategoryIndex == -1) previousCategoryIndex = categoriesList.Count - 1;
            categoriescmb.SelectedIndex = previousCategoryIndex;
        }
        /// <summary>
        /// Prefills all of the data fields with the information from the event the user decided to update.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// PopulateFields();
        /// ]]></code></example>
        public void PopulateFields()
        {
            txbEventDescription.Text = eventToUpdate.ShortDescription;
            txbCalendarFileinEvents.Text = dbPath;
            categoriescmb.SelectedIndex = eventToUpdate.CategoryID - 1;

            startdp.SelectedDate = eventToUpdate.StartDateTime;

            // Adding 24 then mod by 24 to avoid edge case where time is 12am would cause no index to be selected.
            cmbStartTimeHour.SelectedIndex = (eventToUpdate.StartDateTime.Hour - 1 + 24) % 24;

            cmbStartTimeMins.SelectedIndex = eventToUpdate.StartDateTime.Minute / 15;
            txbDuration.Text = eventToUpdate.DurationInMinutes.ToString();
        }
        /// <summary>
        /// Sets the default date and time values on the window.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowDefaultDateTime();
        /// ]]></code></example>
        public void ShowDefaultDateTime()
        {
            //Looks like a lot of logic but only deals with UI tasks
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
        /// <summary>
        /// Shows a message to the user.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowMessage("You're doing good, lad!");]]></code></example>
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButton.OK);
        }
        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="error">The error message to be shown.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowError("Error! Chat, is he being fr right now?");]]></code></example>
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
