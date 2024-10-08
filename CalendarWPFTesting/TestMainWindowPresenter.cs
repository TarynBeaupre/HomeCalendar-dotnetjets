﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Calendar;
using HomeCalendarWPF;
using Xunit;

namespace CalendarWPFTesting
{
    /* NOTE: these tests cause thread issues that we do not know how to resolve without redoing hours of work. I tried for 2 hours -Taryn
    public class TestViewMainWindow : ViewInterface
    {
        public bool calledSetCalendarFilePath = false, calledShowMessage = false, calledSetThemeLight = false, calledSetThemeDark = false;

        public void SetCalendarFilePath(string filePath)
        {
            calledSetCalendarFilePath= true;
        }

        public void SetThemeDark()
        {
            calledSetThemeDark= true;
        }

        public void SetThemeLight()
        {
            calledSetThemeLight= true;
        }

        public void ShowMessage(string message)
        {
            calledShowMessage= true;
        }
    }

    public class TestMainWindowPresenter
    {
        [WpfFact]
        public void Initialization_CallsSetCalendarFilePath()
        {
            // Arrange
            var view = new TestViewMainWindow();

            // Act
            var presenter = new MainWindowPresenter(view);

            // Assert
            Assert.True(view.calledSetCalendarFilePath);
        }

        [Fact]
        public void ShowWarning_ShowsMessage()
        {
            // Arrange
            var view = new TestViewMainWindow();
            var presenter = new MainWindowPresenter(view);

            // Act
            presenter.ShowWarning();

            // Assert
            Assert.True(view.calledShowMessage);
        }

        
        [Fact]
        public void LightThemeWasSet()
        {
            // Arrange
            var view = new TestViewMainWindow();
            var presenter = new MainWindowPresenter(view);

            // Act
            RunOnUIThread(() =>
            {
                presenter.SetTheme("button-light-theme");
            });

            // Assert
            Assert.True(view.calledSetThemeLight);
        }

        [Fact]
        public void DarkThemeWasSet()
        {
            // Arrange
            var view = new TestViewMainWindow();
            var presenter = new MainWindowPresenter(view);

            // Act
            RunOnUIThread(() =>
            {
                presenter.SetTheme("button_dark_theme");
            });

            // Assert
            Assert.True(view.calledSetThemeDark);
        }
        private void RunOnUIThread(Action action)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            Task.Run(() =>
            {
                dispatcher.Invoke(action);
            }).Wait();
        }
    }*/
}
