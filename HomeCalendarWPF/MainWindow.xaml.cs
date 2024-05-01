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

        // My variables for the grid table
        public List<CalendarItem> eventsGridList = new();
        public List<CalendarItemsByMonth> eventsGridListByMonth = new();
        public List<CalendarItemsByCategory> eventsGridListByCat = new();
        public List<Dictionary<string, object>> eventsGridListByCatAndMonth = new();

        public bool groupByMonthFlag = false, groupByCatFlag = false;

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

            if (calendarFiletxb.Text == "path here")
            {
                Trace.WriteLine("Should close");
                Close();
                Application.Current.Shutdown();
                Trace.WriteLine("Did not close");
            }
            else
            {
                if (darkMode)
                    SetThemeDark();
                else
                    SetThemeLight();

                // Output the default events
                presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag);
                EventsGrid.ItemsSource = eventsGridList;
                SetGridColumns();
            }

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
            EventsWindow eventWindow = new EventsWindow(darkMode);
            eventWindow.Show();
        }
        private void OpenCategory(object sender, RoutedEventArgs e)
        {
            CategoriesWindow categoryWindow = new CategoriesWindow(darkMode);
            categoryWindow.Show();
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
                SaveThemeSettingsToRegistry();
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
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void SaveThemeSettingsToRegistry()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "DARK_THEME", (MainWindow.darkMode == true) ? 1 : 0);
        }

        // >>>GRID CODE<<<

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            // Calls a method that checks which checkbox is checked and changes value of global variable
            FindGroupBy();

            // Populate the list with the right events
            presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag);
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
                // Puts all the categories as columns
                foreach (string key in eventsGridListByCatAndMonth[3].Keys)
                {
                    var column = new DataGridTextColumn();
                    column.Header = key;
                    column.Binding = new Binding($"[{key}]");
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
                // Month column
                var column = new DataGridTextColumn();
                column.Header = "Category";
                column.Binding = new Binding("Category");
                EventsGrid.Columns.Add(column);

                // Total Busy Time
                column = new DataGridTextColumn();
                column.Header = "Total Busy Time";
                column.Binding = new Binding("TotalBusyTime") { StringFormat = "0.00" };
                EventsGrid.Columns.Add(column);
            }
        }

        private void EventsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Type that EventsGrid.CurrentItem returns is object, but actual type of
            // the object is Dictionary<string, object> if you check with a.GetType()
            // so I immediately cast it as such
            var a = EventsGrid.CurrentItem as CalendarItem;

            if (a is null)
                return;

            var updateEventsWindow = new UpdateEventsWindow(presenter.model, calendarFiletxb.Text, a);
            updateEventsWindow.ShowDialog();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}