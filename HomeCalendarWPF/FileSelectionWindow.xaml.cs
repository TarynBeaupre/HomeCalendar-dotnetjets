using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for FirstOpenWindow.xaml
    /// </summary>
    public partial class FileSelectionWindow : Window, FileSelectionWindowInterface
    {
        private readonly FileSelectionWindowPresenter presenter;
        public MainWindow.InitializationParams initParams;
        bool overrideClosing = false;

        public FileSelectionWindow(bool darkMode)
        {
            InitializeComponent();
            SetTheme(MainWindow.darkMode);
            presenter = new FileSelectionWindowPresenter(this);
            this.initParams = new MainWindow.InitializationParams();
        }

        // Methods called from the view to the presenter
        private void Btn_Click_PickNewFileDir(object sender, RoutedEventArgs e)
        {
            presenter.PickNewFileDir();
        }
        private void Btn_Click_PickExistingFileDir(object sender, RoutedEventArgs e)
        {
            presenter.PickExistingFileDir();
        }
        private void Btn_Click_OpenRecentFile(object sender, RoutedEventArgs e)
        {
            presenter.OpenRecentFile();
        }
        private void Btn_Click_Confirm(object sender, RoutedEventArgs e)
        {
            presenter.Confirm();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (btnConfirm.IsEnabled == true || overrideClosing)
                base.OnClosing(e);
            else
                e.Cancel = true;
        }


        // Methods called by the presenter on the view
        public void SetDirectoryText(string text)
        {
            tbDir.Text = text;
        }

        public void EnableConfirmButton()
        {
            btnConfirm.IsEnabled = true;
        }

        public void SetInitializationParams(string filePath, bool newDB)
        {
            initParams.filePath = filePath;
            initParams.newDB = newDB;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void CloseWindow()
        {
            this.Close();
        }

        public string GetFilePath()
        {
            return tbDir.Text;
        }
        private void SetTheme(bool darkmode)
        {
            if (darkmode)
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop-dark.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.Gray;
                //light_theme_star.Visibility = Visibility.Collapsed;
                //dark_theme_star.Visibility = Visibility.Visible;
            }
            else
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.LightGreen;
                //light_theme_star.Visibility = Visibility.Visible;
                //dark_theme_star.Visibility = Visibility.Collapsed;
            }
        }
    }
}
