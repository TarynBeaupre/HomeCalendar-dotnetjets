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
        public bool calledView_ShowError = false, calledView_ShowMessage = false, calledView_ShowDefaultCat = false, calledView_ShowDefaultDate = false, calledView_Reset = false;

        public void ShowError(string message)
        {
            calledView_ShowError = true;
        }
        public void ShowMessage(string message)
        {
            calledView_ShowError = true;
        }
        public void ShowDefaultCategories(List<Category> categoriesList)
        {
            calledView_ShowDefaultCat = true;
        }
        public void ShowDefaultDateTime()
        {
            calledView_ShowDefaultDate = true;
        }
        public void ResetEventForm()
        {
            calledView_Reset = true;
        }
    }

    public class UnitTest
    {
        [Fact]
        public void Initialization_CallsShowDefaultDate()
        {
            // Arrange
            TestView view = new TestView();

            // Act
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Assert
            Assert.True(view.calledView_ShowDefaultDate);
        }

        [Fact]
        public void AddNewEvent_ValidInput_CallsModelAddEventAndShowsMessage()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.AddNewEvent("Event details", 1, DateTime.Now, 60, "");

            // Assert
            Assert.True(view.calledView_ShowMessage);
            Assert.True(view.calledView_Reset);
        }

        [Fact]
        public void AddNewEvent_ThrowsException_ShowsError()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.AddNewEvent("Event details", 1, DateTime.Now, 60, "");

            // Assert
            Assert.True(view.calledView_ShowError);
        }

        [Fact]
        public void AddNewCategory_ValidInput_CallsShowMessage()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.AddNewCategory("TestCategory");

            // Assert
            Assert.True(view.calledView_ShowMessage);
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
            Assert.True(view.calledView_ShowError);
        }

        [Fact]
        public void GetDefaultCategories_CallsGetDefaultCategories()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, "testPath");

            // Act
            presenter.GetDefaultCategories();

            // Assert
            Assert.True(view.calledView_ShowDefaultCat);
        }

        [Fact]
        public void AddNewEvent_EventAddedToModel()
        {
            // Arrange
            TestView view = new TestView();
            string path = "testPath";
            EventsPresenter presenter = new EventsPresenter(view, path);
            string details = "Test event";
            int categoryId = 1; // Assuming categoryId for test purpose
            DateTime start = DateTime.Now;
            double duration = 60;

            // Act
            presenter.AddNewEvent(details, categoryId, start, duration, "");

        }

        [Fact]
        public void AddNewCategory_InvalidDuration_DoesNotCallAddEvent()
        {
    
        }
    }
}
