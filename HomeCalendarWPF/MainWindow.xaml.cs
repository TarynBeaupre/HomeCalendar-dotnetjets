﻿using Microsoft.Win32;
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
        public struct InitializationParams
        {
            public string filePath;
            public bool newDB;

            public InitializationParams(string filePath, bool newDB) 
            { 
                this.filePath = filePath; this.newDB = newDB;
            }
        }
        private Presenter presenter;

        // -------------------------------------------------
        // ACTUALLY SUPER IMPORTANT DO NOT FORGET ABOUT THIS
        // -------------------------------------------------
        // TODO: Change to DotNetJetsCalendar (Intentional spelling mistake: Calendary)
        // This is for debugging, ONLY REMOVE IF DONE TESTING
        public static readonly string REGISTRY_SUB_KEY_NAME = "DotNetJetsCalendary";

        public MainWindow()
        {
            InitializeComponent();

            //TODO: This makes the view have too much logic, need to change it to MVP
            InitializationParams initParams = new InitializationParams(Environment.CurrentDirectory, false);

            if (IsFirstUse())
            {
                ReadyForFirstUse();
            }

            FirstOpenWindow fop = new FirstOpenWindow();
            fop.ShowDialog();

            presenter = new Presenter(this, fop.initParams.filePath, fop.initParams.newDB);
            calendarFiletxb.Text = fop.initParams.filePath;
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

        private void Btn_OpenNewFile(object sender, RoutedEventArgs e)
        {
            string defaultFilename = "newDB.db";
            presenter = new Presenter(this, defaultFilename, true);
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
            MessageBox.Show("If you close the next window without saving, your changes will be lost.", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
            Application.Current.Shutdown();
        }

        private bool IsFirstUse()
        {
            // Credit for how to check if key exists in registry https://stackoverflow.com/a/4276150

            // Open software folder under HKEY_CURRENT_USER
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true)!;

            // Check if software folder in registry has our program's info (if not, must be first use)
            return !rKey.GetSubKeyNames().Contains(MainWindow.REGISTRY_SUB_KEY_NAME);
        }

        private void ReadyForFirstUse()
        {
            // Credit for how to create & write to registry: https://stackoverflow.com/a/7230427 as well as C# Docs

            // Open software folder under HKEY_CURRENT_USER
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true)!;

            // creates our folder in the software folder
            rKey.CreateSubKey(MainWindow.REGISTRY_SUB_KEY_NAME);

            // Have to do it this way because just rKey.SetValue("FIRST_USE", 0) doesn't work (should work)
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "FIRST_USE", 0);
        }

        public void NewCalendar()
        {
            //For now, does nothing. Will eventually clear the current calendar and show the new one and its events
            throw new NotImplementedException();
        }

        public void OpenExistingCalendar(string filename, bool existingDB)
        {
            //Opens a calendar and displays its events
            throw new NotImplementedException();
        }

        public void AddNewEvent()
        {
            //Will eventually update the calendar with the events
            throw new NotImplementedException();
        }

        public void AddNewCategory()
        {
            //Does nothing in the calendar view
            throw new NotImplementedException();
        }

        public void ShowError(string msg)
        {
            //TODO: Displays errors in a message box
            throw new NotImplementedException();
        }

    }
}