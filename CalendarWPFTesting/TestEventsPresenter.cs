using Calendar;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeCalendarWPF;
using System.Data.SQLite;
using System.Windows.Controls;

namespace CalendarWPFTesting
{
    public class TestView : EventsViewInterface
    {
        public bool calledShowError, calledShowMessage, calledShowDefaultCat, calledShowDefaultDate, calledReset;

        public void ShowError(string message)
        {
            calledShowError = true;
        }
        public void ShowMessage(string message)
        {
            calledShowError = true;
        }
        public void ShowDefaultCategories(List<Category> categoriesList)
        {
            calledShowDefaultCat = true;
        }
        public void ShowDefaultDateTime()
        {
            calledShowDefaultDate = true;
        }
        public void ResetEventForm()
        {
            calledReset = true;
        }

        [Fact]
        public void AddNewEvent_ValidInput_CallsModelAddEventAndShowsMessage()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.AddNewEvent("Event details", 1, DateTime.Now, 60);

            // Assert
            Assert.True(view.calledShowMessage);
            Assert.True(view.calledReset);
        }

        [Fact]
        public void AddNewEvent_ThrowsException_ShowsError()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.AddNewEvent("Event details", 1, DateTime.Now, 60);

            // Assert
            Assert.True(view.calledShowError);
        }

        [Fact]
        public void AddNewCategory_ValidInput_CallsModelAddCategoryAndShowsMessage()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.AddNewCategory("TestCategory");

            // Assert
            Assert.True(view.calledShowMessage);
        }

        [Fact]
        public void AddNewCategory_ThrowsException_ShowsError()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.AddNewCategory("TestCategory");

            // Assert
            Assert.True(view.calledShowError);
        }

        [Fact]
        public void GetDefaultCategories_CallsModelListAndShowsCategories()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.GetDefaultCategories();

            // Assert
            Assert.True(view.calledShowDefaultCat);
        }
    }
}