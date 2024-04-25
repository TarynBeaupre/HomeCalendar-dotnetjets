using Calendar;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            if (darkMode)
                SetThemeDark();
            else
                SetThemeLight();
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

        private void filterCategoryToggle_Click(object sender, RoutedEventArgs e)
        {
            int chosenCategoryIndex = filterCategoryCmbx.SelectedIndex;
         
            if ((bool)filterCategoryToggle.IsChecked)
            {
                presenter.FilterByCategory(chosenCategoryIndex);
            }
            else
            {
                presenter.ShowAllEvents();
            }
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
            light_tree_image.Visibility= Visibility.Collapsed;
        }

        public void SetDefaultDateTime()
        {
            filterStartDatePicker.SelectedDate = DateTime.Now;
            filterEndDatePicker.SelectedDate = DateTime.Now;
        }

        public void SetDefaultCategories(List<Category> categoryList)
        {
            filterCategoryCmbx.SelectedIndex = 0;
            filterCategoryCmbx.ItemsSource = categoryList;
        }

    }
}