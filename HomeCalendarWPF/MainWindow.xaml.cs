using Calendar;
using Microsoft.Win32;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ViewInterface
    {
        #region Initialization methods

        /// <summary>
        /// Represents initialization parameters used to configure the model.
        /// </summary>
        public struct InitializationParams
        {
            public string filePath;
            public bool newDB;

            /// <summary>
            /// Initializes a new instance of the <see cref="InitializationParams"/> struct with the specified file path and the new database indicator.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <param name="newDB">Indicates whether a new database needs to be created.</param>
            public InitializationParams(string filePath, bool newDB)
            {
                this.filePath = filePath; this.newDB = newDB;
            }
        }
        public static bool darkMode = false;
        bool awaitFilterInput = false;

        // My variables for the grid table
        public List<CalendarItem> eventsGridList = new();
        public List<CalendarItemsByMonth> eventsGridListByMonth = new();
        public List<CalendarItemsByCategory> eventsGridListByCat = new();
        public List<Dictionary<string, object>> eventsGridListByCatAndMonth = new();

        public bool groupByMonthFlag = false, groupByCatFlag = false;

        // Filtering variables
        public bool filterByDateFlag = false, filterByCatFlag = false;

        // -------------------------------------------------
        // ACTUALLY SUPER IMPORTANT DO NOT FORGET ABOUT THIS
        // -------------------------------------------------
        // TODO: Change to DotNetJetsCalendar (Intentional spelling mistake: Calendary)
        // This is for debugging, ONLY REMOVE IF DONE TESTING
        public static readonly string REGISTRY_SUB_KEY_NAME = "DotNetJetsCalendary";

        private MainWindowPresenter presenter;

        /// <summary>
        /// Initializes a new instance of the  <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            presenter = new MainWindowPresenter(this);

            if (calendarFiletxb.Text != "path here")
            {
                if (darkMode)
                    SetThemeDark();
                else
                    SetThemeLight();
                awaitFilterInput = true;
           

                // Output the default events
                RefreshGrid();
                //presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag);
                EventsGrid.ItemsSource = eventsGridList;
                //SetGridColumns();
            }

            //Close();

            //PopulateDataGrid();

            // >> TESTING <<
            //Event event1 = new Event(3, new DateTime(04 / 04 / 04), 2, 15, "hello");
            //List<Event> users = new List<Event>();
            //users.Add(event1);
            //users.Add(event1);
            //users.Add(event1);

            //EventsGrid.ItemsSource = users;
        }

        private void OpenEvent(object sender, RoutedEventArgs e)
        {
            EventsWindow eventWindow = new EventsWindow(presenter.model!, darkMode);
            eventWindow.ShowDialog();
            RefreshGrid();
        }
        private void OpenCategory(object sender, RoutedEventArgs e)
        {
            CategoriesWindow categoryWindow = new CategoriesWindow(presenter.model!, darkMode);
            categoryWindow.ShowDialog();
            RefreshGrid();
        }
        private void Btn_Click_ChangeDBFile(object sender, RoutedEventArgs e)
        {
            presenter = new MainWindowPresenter(this);
        }

        private void Btn_Click_Change_Theme(object sender, RoutedEventArgs e)
        {
            Button? clickedButton = sender as Button;
            MainWindow.darkMode = dark_theme_star.Visibility == Visibility.Collapsed;
            if (clickedButton != null)
            {
                string theme = clickedButton.Name;
                presenter.SetTheme(theme);
            }
        }

        private void Btn_Click_ShowWarning(object sender, RoutedEventArgs e)
        {
            presenter.ShowWarning();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Sets the file path for the calendar and updates the path text block.
        /// </summary>
        /// <param name="filePath">THe path to the chosen file.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetCalendarFilePath("./hello.db");
        /// ]]>
        /// </code></example>
        public void SetCalendarFilePath(string filePath)
        {
            calendarFiletxb.Text = filePath;
        }

        /// <summary>
        /// Shows error messages to the user
        /// </summary>
        /// <param name="message">Error message</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowMessage("Error, you're bad.");
        /// ]]>
        /// </code></example>
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Sets the theme of the application to light mode.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetThemeLight();
        /// ]]>
        /// </code></example>
        public void SetThemeLight()
        {
            // Change the string in Window.Background > ImageSource to light theme image
            background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
            light_theme_star.Visibility = Visibility.Visible;
            light_chicken_image.Visibility = Visibility.Visible;
            light_tree_image.Visibility = Visibility.Visible;

            left_sidemenu_gradient.Color = Colors.LightGreen;
            right_sidemenu_gradient.Color = Colors.LightGreen;
            calendar_gradient.Color = Colors.LightGreen;
            file_sidemenu_gradient.Color = Colors.LightGreen;

            dark_chicken_image.Visibility = Visibility.Collapsed;
            dark_theme_star.Visibility = Visibility.Collapsed;
            dark_tree_image.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Sets the theme of the application to dark mode.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetThemeDark();
        /// ]]>
        /// </code></example>
        public void SetThemeDark()
        {
            // Change the string in ImageSource to dark theme image
            background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop-dark.jpg", UriKind.Relative));
            dark_theme_star.Visibility = Visibility.Visible;
            dark_chicken_image.Visibility = Visibility.Visible;
            dark_tree_image.Visibility = Visibility.Visible;

            right_sidemenu_gradient.Color = Colors.Gray;
            left_sidemenu_gradient.Color = Colors.Gray;
            calendar_gradient.Color = Colors.Gray;
            file_sidemenu_gradient.Color = Colors.Gray;

            light_theme_star.Visibility = Visibility.Collapsed;
            light_chicken_image.Visibility = Visibility.Collapsed;
            light_tree_image.Visibility = Visibility.Collapsed;
        }

        public void SetDefaultDateTime()
        {
            //filterStartDatePicker.SelectedDate = DateTime.Now;
            //filterEndDatePicker.SelectedDate = DateTime.Now;
        }

        public void SetDefaultCategories(List<Category> categoryList)
        {
            filterCategoryCmbx.SelectedIndex = 0;
            filterCategoryCmbx.ItemsSource = categoryList;
        }
        #endregion


        #region Filtering Events
        private void FilterChoiceChanged(object sender, RoutedEventArgs e)
        {
            // Populate the list with the right events
            if (awaitFilterInput)
            {
                FindFilter();
                //if (filterByCatFlag)
                //{
                   int filterCategoryId = filterCategoryCmbx.SelectedIndex + 1;
                 //DateTime oldStart = new DateTime(2015, 1, 1);
                    presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag, filterByCatFlag, filterCategoryId, filterStartDatePicker.SelectedDate, filterEndDatePicker.SelectedDate);
                    SetGridColumns();
                //}
                //else
                //{
                //    DateTime oldStart = new DateTime(2015, 1, 1);
                //    presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag, false, 0, oldStart, filterEndDatePicker.SelectedDate);
                 //   SetGridColumns();
                //}
            }
        }

        private void FindFilter()
        {
            if (filterCategoryToggle.IsChecked == true)
                filterByCatFlag = true;
            else
                filterByCatFlag = false;

            if (filterStartDatePicker.SelectedDate != null && filterEndDatePicker.SelectedDate != null)
                filterByDateFlag = true;
            else
                filterByDateFlag = false;
        }

        #endregion

        // >>>GRID CODE<<<

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            // Calls a method that checks which checkbox is checked and changes value of global variable
            FindGroupBy();

            // Populate the list with the right events
            //presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag);
            int filterCategoryId = filterCategoryCmbx.SelectedIndex + 1;
            presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag, filterByCatFlag, filterCategoryId, filterStartDatePicker.SelectedDate, filterEndDatePicker.SelectedDate);
            SetGridColumns();
        }

        public void SetEventsInGrid<T>(List<T> eventsList)
        {
            EventsGrid.ItemsSource = eventsList;
        }

        private void FindGroupBy()
        {
            if (GroupByMonthToggle.IsChecked == true)
                groupByMonthFlag = true;
            else
                groupByMonthFlag = false;
            if (GroupByCategoryToggle.IsChecked == true)
                groupByCatFlag = true;
            else
                groupByCatFlag = false;
        }

        public void SetEventsInGrid(List<Event> eventsList)
        {
            EventsGrid.ItemsSource = eventsList;
        }

        private void SetGridColumns()
        {
            // Columns to loop over for the normal events grid 
            List<string> columnProperties = new List<string>
            {
                "StartDate",
                "StartTime",
                "Category",
                "ShortDescription",
                "DurationInMinutes",
                "BusyTime"
            };

            // Clear current columns
            EventsGrid.Columns.Clear();
            // Check which group by is active and create the columns depending on that
            if (!groupByMonthFlag && !groupByCatFlag)
            {
                foreach (var propertyName in columnProperties)
                {
                    var column = new DataGridTextColumn();
                    column.Header = propertyName;

                    if (propertyName == "StartDate")
                    {
                        column.Binding = new Binding("StartDateTime");
                        column.Binding.StringFormat = "dd/MM/yyyy";
                    }
                    else if (propertyName == "StartTime")
                    {
                        column.Binding = new Binding("StartDateTime");
                        column.Binding.StringFormat = "hh:mm tt";
                    }
                    else
                        column.Binding = new Binding(propertyName);
                    EventsGrid.Columns.Add(column);
                }
            }
            else if (groupByMonthFlag && groupByCatFlag)
            {
                var monthColumn = new DataGridTextColumn();
                monthColumn.Header = "Month";
                monthColumn.Binding = new Binding("[Month]");
                EventsGrid.Columns.Add(monthColumn);

                // TODO: Make presenter method to do this.
                var cats = presenter.model!.categories.List();

                foreach (var cat in cats)
                {
                    var column = new DataGridTextColumn();
                    column.Header = cat.Description;
                    column.Binding = new Binding($"[{cat.Description}]");
                    EventsGrid.Columns.Add(column);
                }

                // Get the busy time column
                var TBTcolumn = new DataGridTextColumn();
                TBTcolumn.Header = "TotalBusyTime";
                TBTcolumn.Binding = new Binding($"[TotalBusyTime]") { StringFormat = "0.00" };
                EventsGrid.Columns.Add(TBTcolumn);
            }
            else if (groupByMonthFlag)
            {
                // Month column
                var column = new DataGridTextColumn();
                column.Header = "Month";
                column.Binding = new Binding("Month");
                EventsGrid.Columns.Add(column);

                // Total Busy Time
                column = new DataGridTextColumn();
                column.Header = "Total Busy Time";
                column.Binding = new Binding("TotalBusyTime") { StringFormat = "0.00" };
                EventsGrid.Columns.Add(column);
            }

            else if (groupByCatFlag)
            {
                // Category column
                var column = new DataGridTextColumn();
                column.Header = "Category";
                column.Binding = new Binding("Category");
                EventsGrid.Columns.Add(column);

                // Total Busy Time
                column = new DataGridTextColumn();
                column.Header = "Total Busy Time";
                column.Binding = new Binding("TotalBusyTime") { StringFormat = "0.00" };
                EventsGrid.Columns.Add(column);

                int filterCategoryId = filterCategoryCmbx.SelectedIndex + 1;
                presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag, filterByCatFlag, filterCategoryId, filterStartDatePicker.SelectedDate, filterEndDatePicker.SelectedDate);

                //presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag);
                //List<Dictionary<string, object>> columns = new List<Dictionary<string, object>>();
            }
        }

        private void Event_Update_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = EventsGrid.CurrentItem as CalendarItem;

            if (selectedItem is null)
                return;

            var updateEventsWindow = new UpdateEventsWindow(presenter.model!, calendarFiletxb.Text, selectedItem);
            updateEventsWindow.ShowDialog();
            SetGridColumns();
        }

        private void Event_Delete_Click(object sender, RoutedEventArgs e)
        {
            var a = EventsGrid.CurrentItem;
            var eventToDelete = EventsGrid.CurrentItem as CalendarItem;

            var b = a.GetType();

            if (groupByMonthFlag || groupByCatFlag)
            {
                MessageBox.Show("Need to select singular event.");
                return;
            }
            else if (eventToDelete is null)
            {
                MessageBox.Show("Event is null");
                return;
            }

            var choice = MessageBox.Show("Are you sure you want to delete Event?", "Delete Confirmation", MessageBoxButton.YesNo);

            if (choice == MessageBoxResult.Yes)
            {
                presenter.DeleteEvent(eventToDelete);
                // Yeah not efficient repopulating all the list everytime
                presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag);
                SetGridColumns();
            }
            
        }

        private void Event_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EventsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Type that EventsGrid.CurrentItem returns is object, but actual type of
            // the object is Dictionary<string, object> if you check with a.GetType()
            // so I immediately cast it as such
            var a = EventsGrid.CurrentItem as CalendarItem;

            if (a is null)
                return;

            var updateEventsWindow = new UpdateEventsWindow(presenter.model!, calendarFiletxb.Text, a);
            updateEventsWindow.ShowDialog();
            SetGridColumns();
        }
        private void RefreshGrid()
        {
            presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag);
            SetGridColumns();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

}

