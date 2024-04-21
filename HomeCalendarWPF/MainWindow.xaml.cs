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
        //! We should probably remove this (i wrote this) -ec
        public struct InitializationParams
        {
            public string filePath;
            public bool newDB;

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

        private Presenter presenter;

        public MainWindow()
        {
            InitializeComponent();
            presenter = new Presenter(this);
            if (darkMode)
                SetThemeDark();
            else
                SetThemeLight();
        }

        private void OpenEvent(object sender, RoutedEventArgs e)
        {
            EventsWindow eventWindow = new EventsWindow(presenter, darkMode);
            eventWindow.Show();
        }
        private void Btn_Click_ChangeDBFile(object sender, RoutedEventArgs e)
        {
            presenter = new Presenter(this);
        }

        private void Btn_Click_Change_Theme(object sender, RoutedEventArgs e)
        {
            Button? clickedButton = sender as Button;
            MainWindow.darkMode = dark_theme_star.Visibility == Visibility.Collapsed;
            if (clickedButton != null)
            {
                if (clickedButton.Name == "button_dark_theme")
                {
                    SetThemeDark();
                }
                else if (clickedButton.Name == "button_light_theme")
                {
                    SetThemeLight();
                }
                SaveThemeSettingsToRegistry();
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

        private void SaveThemeSettingsToRegistry()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "DARK_THEME", (MainWindow.darkMode == true) ? 1 : 0);
        }
        private void SetThemeLight()
        {
            // Change the string in Window.Background > ImageSource to light theme image
            background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
            sidemenu_gradient.Color = Colors.LightGreen;
            calendar_gradient.Color = Colors.LightGreen;
            light_theme_star.Visibility = Visibility.Visible;
            dark_theme_star.Visibility = Visibility.Collapsed;
        }
        private void SetThemeDark()
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
    }
}