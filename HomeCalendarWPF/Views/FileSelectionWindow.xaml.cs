using HomeCalendarWPF.Interfaces.Views;
using HomeCalendarWPF.Presenters;
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

        /// <summary>
        /// Initializes a new instance of the  <see cref="FileSelectionWindow"/> class.
        /// </summary>
        /// <param name="darkMode">Indicates which theme to be used in Windows.</param>
        public FileSelectionWindow(bool darkMode)
        {
            InitializeComponent();
            SetThemeFilePopup(MainWindow.darkMode);
            presenter = new FileSelectionWindowPresenter(this);
            this.initParams = new MainWindow.InitializationParams();
        }

        #region Event Handlers
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
        #endregion

        #region Interface Methods
        /// <summary>
        /// Sets the file path for the calendar and updates the path text block.
        /// </summary>
        /// <param name="path">THe path to the chosen file.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetDirectoryText("./hello.db");
        /// ]]>
        /// </code></example>
        public void SetDirectoryText(string path)
        {
            tbDir.Text = path;
        }
        /// <summary>
        /// Enables confirm button, once file is chosen or created
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath)
        ///     EnableConfirmButton();
        /// ]]>
        /// </code></example>
        public void EnableConfirmButton()
        {
            btnConfirm.IsEnabled = true;
        }
        /// <summary>
        /// Sets the filePath and if new database to params
        /// </summary>
        /// <param name="filePath">Path to the chosen file</param>
        /// <param name="newDB">Indicates if need new database, if true create new database</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath && newDB)
        ///     SetInitializationParams(filepath, newDB);
        /// ]]>
        /// </code></example>
        public void SetInitializationParams(string filePath, bool newDB)
        {
            initParams.filePath = filePath;
            initParams.newDB = newDB;
        }
        /// <summary>
        /// Closes the current window
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath && newDB)
        ///     SetInitializationParams(filepath, newDB);
        /// else
        ///     ShowError("Error: FilePath is null");
        ///     CloseWindow();
        /// ]]>
        /// </code></example>
        public void CloseWindow()
        {
            this.Close();
        }
        /// <summary>
        /// Gets the path of the directory depending on path text block
        /// </summary>
        /// <returns>A string of the path to the path</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath && newDB)
        ///     SetInitializationParams(filepath, newDB);
        /// path = GetFilePath()
        /// ]]>
        /// </code></example>
        public string GetFilePath()
        {
            return tbDir.Text;
        }
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// Shows error messages to the user
        /// </summary>
        /// <param name="error">Error message</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath && newDB)
        ///     SetInitializationParams(filepath, newDB);
        /// else
        ///     ShowError("Error: FilePath is null");
        /// ]]>
        /// </code></example>
        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region Private Methods
        private void SetThemeFilePopup(bool darkmode)
        {
            if (darkmode)
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop-dark.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.Gray;
            }
            else
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.LightGreen;
            }
        }
        #endregion
    }
}
