using Microsoft.Win32;
using System.IO;


namespace HomeCalendarWPF
{
    internal class FirstOpenWindowPresenter
    {
        private readonly FirstOpenWindowInterface fopView;

        // Constructor
        public FirstOpenWindowPresenter(FirstOpenWindowInterface view)
        {
            this.fopView = view;
        }

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

        public void OpenRecentFile()
        {
            // Actual bullshit if this works       -- Why is there a disgusting curse word on the first line
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

        public void Confirm()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "RECENT_FILE", fopView.GetFilePath(), RegistryValueKind.String);

            fopView.CloseWindow();
        }

        // Methods that get necessitating the view
        private void ChangeViewMethods(string filename, bool newDB = false)
        {
            fopView.SetDirectoryText(filename);
            fopView.SetInitializationParams(filename, newDB);
            fopView.EnableConfirmButton();
        }
    }
}
