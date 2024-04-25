﻿using Calendar;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class EventsWindow : Window, EventsViewInterface
    {
        private EventsPresenter presenter;
        private static DateTime previousDate = System.DateTime.Now;
        public static int previousCategoryIndex = 0;
        private bool darkMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsWindow"/> class.
        /// </summary>
        /// <param name="darkmode">Specifies which theme should be picked for Window, if true then display dark mode.</param>
        public EventsWindow(bool darkmode)
        {
            InitializeComponent();
            txbCalendarFileinEvents.Text = ((MainWindow)Application.Current.MainWindow).calendarFiletxb.Text;
            string filePath = txbCalendarFileinEvents.Text;
            this.presenter = new EventsPresenter(this, filePath);
            this.darkMode = darkmode;

            // Sets default date and times on the window
            ShowDefaultDateTime();
            // Sets default categories on the window
            presenter.GetDefaultCategories();
            // Set the theme from the mainWindow
            SetTheme(this.darkMode);
        }

        // If user types in a category that doesn't exist, runs the addCategory code
        /// <summary>
        /// Adds a new category based on the user selection.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// AddNewCategory();
        /// ]]></code></example>
        public void AddNewCategory()
        {
            ComboBoxItem typeItem = (ComboBoxItem)categoriescmb.SelectedItem;
            string categoryName = typeItem.Content.ToString()!;
            presenter.AddNewCategory(categoryName);
        }

        private void Btn_Click_Add_Event(object sender, RoutedEventArgs e)
        {
            //Validating form data
            if (!ValidateEventForm())
                return;

            //Add the event to the database via the presenter
            string details = txbEventDescription.Text;
            int categoryId = categoriescmb.SelectedIndex;

            previousDate = (DateTime)startdp.SelectedDate!;

            double duration = Convert.ToDouble(txbDuration.Text);

            //Replacing previous options
            // TODO: maybe do categoylist.length
            previousCategoryIndex = categoryId;

            presenter.AddNewEvent(details, categoryId, startdp.SelectedDate, duration, categoriescmb.Text);

        }
        private void Btn_Click_Cancel_Event(object sender, EventArgs e)
        {
            // if user cancels addition, resent the default values for the category and the dates
            this.Close();
        }

        private bool ValidateEventForm()
        {
            //Check that start date has a value
            if (!startdp.SelectedDate.HasValue)
            {
                ShowError("Please select a start date.");
                return false;
            }

            // Check that end date has a value
            if (!enddp.SelectedDate.HasValue)
            {
                ShowError("Please select an end date.");
                return false;
            }

            // Check if duration is provided and is a positive double
            if (!double.TryParse(txbDuration.Text, out double duration) || duration <= 0)
            {
                ShowError("Please provide a valid duration in minutes. The duration should be a positive number.");
                return false;
            }
            return true;
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

        //===== VIEW INTERFACE METHODS =====
        /// <summary>
        /// Shows an error message in message box.
        /// </summary>
        /// <param name="message">Error message to display.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowError("You're bad");
        /// ]]></code></example>
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// Shows a message in message box.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowMessage("You're cool");
        /// ]]></code></example>
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
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
            // if previousCategoryIndex is -1 this means that a new category was added so
            // we can just set it to the length of the list -1 as it's added at the end.
            categoriescmb.SelectedIndex = previousCategoryIndex != -1 ? previousCategoryIndex : categoriesList.Count - 1;
            categoriescmb.ItemsSource = categoriesList;
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
            //=== Set Start/End date defaults ===
            startdp.SelectedDate = previousDate;
            enddp.SelectedDate = previousDate;


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
        /// Resets the event form by clearing input fields and setting default values.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ResetEventForm();
        /// ]]></code></example>
        public void ResetEventForm()
        {
            //Set details textbox to empty
            txbEventDescription.Text = "";

            //Set duration back to 30
            txbDuration.Text = "30";

            //Reset time and date 
            ShowDefaultDateTime();
        }

        private void Btn_Click_AddNewCategory(object sender, RoutedEventArgs e)
        {
            CategoriesWindow categoryWindow = new CategoriesWindow(darkMode);
            categoryWindow.ShowDialog();

            // this is so ugly and definitely doesn't follow mvp, but it's the only thing i found would work, sorry! -ec
            string filePath = txbCalendarFileinEvents.Text;
            this.presenter = new EventsPresenter(this, filePath);
            presenter.GetDefaultCategories();
        }
    }
}
