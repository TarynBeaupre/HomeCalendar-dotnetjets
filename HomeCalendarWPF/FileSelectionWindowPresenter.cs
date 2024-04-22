using Microsoft.Win32;
using System.IO;


namespace HomeCalendarWPF
{
    /// <summary>
    /// Manages file selection from WPF Window
    /// </summary>
    public class FileSelectionWindowPresenter
    {
        private readonly FileSelectionWindowInterface fopView;

        /// <summary>
        /// Creates a new instance of the <see cref="FileSelectionWindowPresenter"/> class with specified properties.
        /// </summary>
        /// <param name="view">View interface.</param>
        /// <example>
        /// <code>
        /// For this example, assume we have well implemented IView
        /// <![CDATA[
        /// view = IView;
        /// presenter = new FileSelectionWindowPresenter(view);
        /// ]]></code></example>
        public FileSelectionWindowPresenter(FileSelectionWindowInterface view)
        {
            this.fopView = view;
        }

        /// <summary>
        /// Picks new file directory.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// private void Btn_Click_PickNewFileDir(object sender, RoutedEventArgs e)
        /// {
        ///    presenter.PickNewFileDir();
        /// }
        /// ]]>
        /// </code></example>
        public void PickNewFileDir()
        {
            Microsoft.Win32.SaveFileDialog fileSelector = new Microsoft.Win32.SaveFileDialog();

            fileSelector.DefaultExt = ".db";
            fileSelector.Filter = "Database Files (*.db)|*.db|All Files (*)|*";

            bool? result = fileSelector.ShowDialog();

            if (result is not null && result == true)
            {
                string filename = fileSelector.FileName;

                // Calls methods in view that make action with view variable names
                ChangeViewMethods(filename, true);
            }
        }

        /// <summary>
        /// Picks existing file directory.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// private void Btn_Click_PickExistingFileDir(object sender, RoutedEventArgs e)
        /// {
        ///    presenter.PickExistingFileDir();
        /// }
        /// ]]>
        /// </code></example>
        public void PickExistingFileDir()
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

                // Calls methods in view that make action with view variable names
                ChangeViewMethods(filename, false);
            }
        }

        /// <summary>
        /// Picks most recently opened file directory.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// private void Btn_Click_OpenRecentFile(object sender, RoutedEventArgs e)
        /// {
        ///    presenter.OpenRecentFile();
        /// }
        /// ]]>
        /// </code></example>
        public void OpenRecentFile()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            string? recentFilePath = Registry.GetValue(keyName, "RECENT_FILE", "DOES_NOT_EXIST") as string;

            if (recentFilePath == null || recentFilePath == "DOES_NOT_EXIST")
            {
                fopView.ShowError("Could not open recent file. No such file exists.");
                return;
            }

            if (!File.Exists(recentFilePath))
            {
                fopView.ShowError($"Could not find recent file. File may have moved from {recentFilePath}");
                return;
            }

            ChangeViewMethods(recentFilePath, false);
        }

        /// <summary>
        /// Confirms the file selection and closes the current WPF window.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// private void Btn_Click_Confirm(object sender, RoutedEventArgs e)
        /// {
        ///    presenter.Confirm();
        /// }
        /// ]]>
        /// </code></example>
        public void Confirm()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "RECENT_FILE", fopView.GetFilePath(), RegistryValueKind.String);

            fopView.CloseWindow();
        }

        // Methods that get objects necessitating the view
        private void ChangeViewMethods(string filename, bool newDB = false)
        {
            fopView.SetDirectoryText(filename);
            fopView.SetInitializationParams(filename, newDB);
            fopView.EnableConfirmButton();
        }
    }
}
