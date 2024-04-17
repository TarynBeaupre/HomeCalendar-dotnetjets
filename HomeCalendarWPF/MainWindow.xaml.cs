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
        private Presenter presenter;

        public MainWindow()
        {
            string baseDir = Environment.CurrentDirectory;
            InitializeComponent();
            calendarFiletxb.Text = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\CalendarApp\\test.calendar"));

            if (IsFirstUse())
            {
                ReadyForFirstUse();
            }
        }
        private void OpenEvent(object sender, RoutedEventArgs e)
        {
            EventsWindow eventWindow = new EventsWindow();
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
            throw new NotImplementedException();
        }
        private bool IsFirstUse()
        {
            // Credit for how to check if key exists in registry https://stackoverflow.com/a/4276150

            // Open software folder under HKEY_CURRENT_USER
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true)!;

            // Check if software folder in registry has our program's info (if not, must be first use)
            return !rKey.GetSubKeyNames().Contains("DotNetJetsCalendar");
        }
        private void ReadyForFirstUse()
        {
            // Credit for how to create & write to registry: https://stackoverflow.com/a/7230427 as well as C# Docs

            // Open software folder under HKEY_CURRENT_USER
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true)!;

            // -------------------------------------------------
            // ACTUALLY SUPER IMPORTANT DO NOT FORGET ABOUT THIS
            // -------------------------------------------------
            // TODO: Change to DotNetJetsCalendar (Intentional spelling mistake: Calendary)
            // This is for debugging, ONLY REMOVE IF DONE TESTING

            // creates our folder in the software folder
            const string SUB_KEY_NAME = "DotNetJetsCalendary";
            rKey.CreateSubKey(SUB_KEY_NAME);

            // Have to do it this way because just rKey.CreateSubKey("FIRST_USE", 0) doesn't work (should work)
            const string keyName = @$"HKEY_CURRENT_USER\Software\{SUB_KEY_NAME}";
            Registry.SetValue(keyName, "FIRST_USE", 0);
        }

        public void NewCalendar()
        {
            throw new NotImplementedException();
        }

        public void AddNewEvent()
        {
            throw new NotImplementedException();
        }

        public void AddNewCategory()
        {
            throw new NotImplementedException();
        }

        public void ShowError(string msg)
        {
            throw new NotImplementedException();
        }

        public void OpenExistingCalendar(string filename, bool existingDB)
        {
            throw new NotImplementedException();
        }
    }
}