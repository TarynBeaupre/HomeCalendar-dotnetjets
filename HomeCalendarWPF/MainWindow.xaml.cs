﻿using System.Text;
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
        }
        private void OpenEvent(object sender, RoutedEventArgs e)
        {
            EventsWindow eventWindow = new EventsWindow(presenter);
            eventWindow.Show();
        }

        private void Btn_OpenFileExplorer(object sender, RoutedEventArgs e)
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
            throw new NotImplementedException();
        }

        public void NewCalendar()
        {
            throw new NotImplementedException();
        }

        public void OpenExistingCalendar(string filename, bool existingDB)
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

    }
}