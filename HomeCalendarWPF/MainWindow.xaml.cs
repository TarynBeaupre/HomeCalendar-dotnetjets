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
        //! No clue how to use this -jh
        public struct InitializationParams
        {
            public string filePath;
            public bool newDB;

            public InitializationParams(string filePath, bool newDB) 
            { 
                this.filePath = filePath; this.newDB = newDB;
            }
        }

        // -------------------------------------------------
        // ACTUALLY SUPER IMPORTANT DO NOT FORGET ABOUT THIS
        // -------------------------------------------------
        // TODO: Change to DotNetJetsCalendar (Intentional spelling mistake: Calendary)
        // This is for debugging, ONLY REMOVE IF DONE TESTING
        public static readonly string REGISTRY_SUB_KEY_NAME = "DotNetJetsCalendary";

        private Presenter presenter;

        public MainWindow()
        {
            InitializeComponent();
            InitializationParams initParams = new InitializationParams(Environment.CurrentDirectory, false);
            presenter = new Presenter(this, initParams.filePath, initParams.newDB);

            presenter.Initialize();
        }

        private void OpenEvent(object sender, RoutedEventArgs e)
        {
            bool darkmode = false;

            // To pass the current theme in the child window - Depends on the visibility of the star
            if (dark_theme_star.Visibility == Visibility.Visible)
                darkmode = true;
            else
                darkmode = false;

            EventsWindow eventWindow = new EventsWindow(presenter, darkmode);
            eventWindow.Show();
        }

        private void Btn_Click_OpenDBFile(object sender, RoutedEventArgs e)
        {
            // Credit: https://stackoverflow.com/a/10315283
            Microsoft.Win32.OpenFileDialog fileSelector = new Microsoft.Win32.OpenFileDialog();

            // Sets default file extension to be used when searching for files
            fileSelector.DefaultExt = ".db";

            // Gives the little box above the open and cancel button the text to allow filtering
            fileSelector.Filter = "Database Files (*.db)|*.db|All Files (*)|*";

            bool? result = fileSelector.ShowDialog();

            if (result is not null && result == true)
            {
                string filename = fileSelector.FileName;
                calendarFiletxb.Text = filename;

                // Create new presenter with chosen file 
                presenter = new Presenter(this, filename, true);
            }
        }

        private void Btn_Click_NewDBFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog fileSelector = new Microsoft.Win32.SaveFileDialog();

            fileSelector.DefaultExt = ".db";
            fileSelector.Filter = "Database Files (*.db)|*.db|All Files (*)|*";

            bool? result = fileSelector.ShowDialog();

            if (result is not null && result == true)
            {
                string filename = fileSelector.FileName;
                calendarFiletxb.Text = filename;

                presenter = new Presenter(this, filename);
            }
        }

        private void Btn_Click_Change_Theme(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                if (clickedButton.Name == "button_dark_theme")
                {
                    // Change the string in ImageSource to dark theme image
                    background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop-dark.jpg", UriKind.Relative));
                    sidemenu_gradient.Color = Colors.Gray;
                    calendar_gradient.Color = Colors.Gray;
                    // Changing only the URISource of the image doesn't work...
                    //star_image.UriSource = new Uri("images/stardew-star-dark.png", UriKind.RelativeOrAbsolute);
                    light_theme_star.Visibility = Visibility.Collapsed;
                    dark_theme_star.Visibility = Visibility.Visible;

                }
                else if (clickedButton.Name == "button_light_theme")
                {
                    // Change the string in Window.Background > ImageSource to light theme image
                    background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
                    sidemenu_gradient.Color = Colors.LightGreen;
                    calendar_gradient.Color = Colors.LightGreen;
                    light_theme_star.Visibility = Visibility.Visible;
                    dark_theme_star.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Btn_Click_ShowWarning(object sender, RoutedEventArgs e)
        {
            presenter.ShowWarning();
            Application.Current.Shutdown();
        }

        public void SetCalendarFilePath(string filePath)
        {
            calendarFiletxb.Text = filePath;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}