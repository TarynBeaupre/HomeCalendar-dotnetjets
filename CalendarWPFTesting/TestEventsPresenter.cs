using Calendar;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows;
using HomeCalendarWPF.Interfaces.Views;
using HomeCalendarWPF.Presenters;

namespace CalendarWPFTesting
{
    public class TestView : AddEventsViewInterface
    {
        public bool calledView_ShowError, calledView_ShowMessage, calledView_ShowDefaultCat, calledView_ShowDefaultDate, calledView_Reset = false, calledView_HasSelectedDate, calledView_HasDuration = false;

        public void ShowError(string message)
        {
            calledView_ShowError = true;
        }
        public void ShowMessage(string message)
        {
            calledView_ShowMessage = true;
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

        public bool HasSelectedDate()
        {
            calledView_HasSelectedDate = true;
            return calledView_HasSelectedDate;
        }

        public bool HasDurationValue()
        {
            calledView_HasDuration = true;
            return calledView_HasDuration;
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
            EventsPresenter presenter = new EventsPresenter(view, new HomeCalendar("testPath"), "testPath");

            // Assert
            Assert.True(view.calledView_ShowDefaultDate);
        }

        [Fact]
        public void AddNewEvent_ValidInput_CallsModelAddEventAndShowsMessage()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, new HomeCalendar("testPath"), "testPath");

            // Act
            presenter.AddNewEvent("Event details", 1, DateTime.Now, 60, "");

            // Assert
            Assert.True(view.calledView_ShowMessage);
            Assert.True(view.calledView_Reset);
        }

        [Fact]
        public void AddNewCategory_ValidInput_CallsShowMessage()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, new HomeCalendar("testPath"), "testPath");

            // Act
            presenter.AddNewCategory("TestCategory");
          
            // Assert
            Assert.True(view.calledView_ShowMessage);
        }

        [Fact]
        public void GetDefaultCategories_CallsGetDefaultCategories()
        {
            // Arrange
            TestView view = new TestView();
            EventsPresenter presenter = new EventsPresenter(view, new HomeCalendar("testPath"), "testPath");

            // Act
            presenter.GetDefaultCategories();

            // Assert
            Assert.True(view.calledView_ShowDefaultCat);
        }

        /*[Fact]
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
    
        }*/
    }
}
