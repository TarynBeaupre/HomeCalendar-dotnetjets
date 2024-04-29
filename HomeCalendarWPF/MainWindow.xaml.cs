using Calendar;
using Microsoft.Win32;
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
        public List<Event> eventsGridList = new List<Event>();

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
                Application.Current.Shutdown();

            if (darkMode)
                SetThemeDark();
            else
                SetThemeLight();

            PopulateDataGrid();


            // >> TESTING <<
            //Event event1 = new Event(3, new DateTime(04/04/04), 2, 15, "hello");
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
            sidemenu_gradient.Color = Colors.LightGreen;
            calendar_gradient.Color = Colors.LightGreen;
            light_theme_star.Visibility = Visibility.Visible;
            dark_theme_star.Visibility = Visibility.Collapsed;
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
            sidemenu_gradient.Color = Colors.Gray;
            calendar_gradient.Color = Colors.Gray;
            light_theme_star.Visibility = Visibility.Collapsed;
            dark_theme_star.Visibility = Visibility.Visible;
        }
        private void SaveThemeSettingsToRegistry()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "DARK_THEME", (MainWindow.darkMode == true) ? 1 : 0);
        }

        public void SetEventsInGrid(List<Event> eventsList)
        {

            EventsGrid.ItemsSource = eventsList;
        }
        private void PopulateDataGrid()
        {
            presenter.SetGridEventsList(ref eventsGridList);
            List<Dictionary<string, object>> columns = new List<Dictionary<string, object>>();

            double busyTime = 0;
            for (int i = 0; i < eventsGridList.Count; i++)
            {
                columns.Add(new Dictionary<string, object>());

                var startDateTimeSplit = eventsGridList[i].StartDateTime.ToString().Split(' ');
                var startDateSplit = startDateTimeSplit[0].Split('-');
                busyTime += eventsGridList[i].DurationInMinutes;

                columns[i]["Start Date"] = $"{startDateSplit[2]}/{startDateSplit[1]}/{startDateSplit[0]}";
                columns[i]["Start Time"] = startDateTimeSplit[1];
                columns[i]["Category"] = eventsGridList[i].Category;
                columns[i]["Description"] = eventsGridList[i].Details;
                columns[i]["Duration"] = eventsGridList[i].DurationInMinutes;
                columns[i]["Busy Time"] = busyTime;
            }

            EventsGrid.ItemsSource = columns;
            EventsGrid.Columns.Clear();

            foreach (string key in columns[0].Keys)
            {
                var column = new DataGridTextColumn();
                column.Header = key;
                column.Binding = new Binding($"[{key}]");
                EventsGrid.Columns.Add(column);
            }
        }

        private void EventsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Type that EventsGrid.CurrentItem returns is object, but actual type of
            // the object is Dictionary<string, object> if you check with a.GetType()
            // so I immediately cast it as such
            var a = EventsGrid.CurrentItem as Dictionary<string, object>;

            if (a is null)
                return;

            var updateEventsWindow = new UpdateEventsWindow(presenter.model, calendarFiletxb, a);
            updateEventsWindow.ShowDialog();
        }
    }
}