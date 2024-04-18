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
    public partial class FirstOpenWindow : Window
    {
        public MainWindow.InitializationParams initParams;
        bool overrideClosing = false;
        public FirstOpenWindow()
        {
            InitializeComponent();

            this.initParams = new MainWindow.InitializationParams();
        }
        // Move this logic in presenter probably
        private void Btn_Click_PickNewFileDir(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog fileSelector = new Microsoft.Win32.SaveFileDialog();

            fileSelector.DefaultExt = ".db";
            fileSelector.Filter = "Database Files (*.db)|*.db|All Files (*)|*";

            bool? result = fileSelector.ShowDialog();

            if (result is not null && result == true)
            {
                string filename = fileSelector.FileName;
                tbDir.Text = filename;

                btnConfirm.IsEnabled = true;

                initParams.filePath = filename;
                initParams.newDB = true;
            }
        }
        private void Btn_Click_PickExistingFileDir(object sender, RoutedEventArgs e)
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
                tbDir.Text = filename;

                btnConfirm.IsEnabled = true;

                initParams.filePath = filename;
                initParams.newDB = false;
            }
        }
        private void Btn_Click_OpenRecentFile(object sender, RoutedEventArgs e)
        {
            // Actual bullshit if this works
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            string? recentFilePath = Registry.GetValue(keyName, "RECENT_FILE", "DOES_NOT_EXIST") as string;

            if (recentFilePath == null || recentFilePath == "DOES_NOT_EXIST")
            {
                MessageBox.Show("Could not open recent file. No such file exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(recentFilePath))
            {
                MessageBox.Show($"Could not find recent file. File may have moved from {recentFilePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            tbDir.Text = recentFilePath;
            initParams.filePath = recentFilePath;
            initParams.newDB = false;

            btnConfirm.IsEnabled = true;
        }
        private void Btn_Click_Confirm(object sender, RoutedEventArgs e)
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "RECENT_FILE", initParams.filePath, RegistryValueKind.String);

            this.Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (btnConfirm.IsEnabled == true || overrideClosing)
                base.OnClosing(e);
            else
                e.Cancel = true;
        }
    }
}
