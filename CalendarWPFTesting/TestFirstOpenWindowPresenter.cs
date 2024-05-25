using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeCalendarWPF;
using System.Data.SQLite;
using System.Windows.Controls;
using Microsoft.Win32;
using HomeCalendarWPF.Interfaces.Views;
using HomeCalendarWPF.Presenters;

namespace CalendarWPFTesting
{
    public class TestFirstOpenWindowPresenter : FileSelectionWindowInterface
    {
        public bool calledView_CloseWindow, calledView_EnableConfirmButton, calledView_GetFilePath, calledView_SetDirectoryText, calledView_SetInitializationParams, calledView_ShowError, calledView_ShowMessage;

        public void CloseWindow()
        {
            calledView_CloseWindow = true;
        }

        public void EnableConfirmButton()
        {
            calledView_EnableConfirmButton = true;
        }

        public string GetFilePath()
        {
            calledView_GetFilePath = true;
            return "test";
        }

        public void SetDirectoryText(string text)
        {
            calledView_SetDirectoryText = true;
        }

        public void SetInitializationParams(string filePath, bool isNewDB)
        {
            calledView_SetInitializationParams = true;
        }

        public void ShowError(string message)
        {
            calledView_ShowError = true;
        }

        public void ShowMessage(string message)
        {
            calledView_ShowMessage = true;
        }

        [Fact]
        public void PickNewFileDir_ValidInput_UpdatesView()
        {
            //Arrange
            TestFirstOpenWindowPresenter view = new TestFirstOpenWindowPresenter();
            FileSelectionWindowPresenter presenter = new FileSelectionWindowPresenter(view);

            // Act & Assert
            Assert.NotNull(() => presenter.PickNewFileDir());
        }

        [Fact]
        public void OpenRecentFile_ValidInput_UpdatesViewWithNoErrors()
        {
            // Caution, for this test you need to have opened a recent file
            // Arrange
            TestFirstOpenWindowPresenter view = new TestFirstOpenWindowPresenter();
            FileSelectionWindowPresenter presenter = new FileSelectionWindowPresenter(view);

            // Act
            presenter.OpenRecentFile();

            // Assert
            Assert.True(view.calledView_SetDirectoryText);
            Assert.True(view.calledView_SetInitializationParams);
            Assert.True(view.calledView_EnableConfirmButton);
            Assert.False(view.calledView_ShowError);
        }

        [Fact]
        public void Confirm_ClosesWindow()
        {
            // Arrange
            TestFirstOpenWindowPresenter view = new TestFirstOpenWindowPresenter();
            FileSelectionWindowPresenter presenter = new FileSelectionWindowPresenter(view);

            // Act
            presenter.Confirm();

            // Assert
            Assert.True(view.calledView_CloseWindow);
        }

        [Fact]
        public void Confirm_CallsGetFilePath()
        {
            // Arrange
            TestFirstOpenWindowPresenter view = new TestFirstOpenWindowPresenter();
            FileSelectionWindowPresenter presenter = new FileSelectionWindowPresenter(view);

            // Act
            presenter.Confirm();

            // Assert
            Assert.True(view.calledView_GetFilePath);
        }

        [Fact]
        public void Confirm_SetsRecentFilePathInRegistry()
        {
            // Arrange
            TestFirstOpenWindowPresenter view = new TestFirstOpenWindowPresenter();
            FileSelectionWindowPresenter presenter = new FileSelectionWindowPresenter(view);

            // Act
            presenter.Confirm();

            // Assert
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            string recentFilePath = Registry.GetValue(keyName, "RECENT_FILE", "DOES_NOT_EXIST") as string;
            Assert.NotNull(recentFilePath);
            Assert.Equal("test", recentFilePath);
        }

      
    }
}
