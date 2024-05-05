using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using HomeCalendarWPF.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CalendarWPFTesting
{
    public class TestUpdateView : UpdateEventsWindowInterface
    {
        public bool calledPopulateFields = false, calledShowDefaultCategories = false, calledShowDefaultTime = false, calledShowError = false, calledShowMessage = false;
        public void PopulateFields()
        {
            calledPopulateFields = true;
        }

        public void ShowDefaultCategories(List<Category> categoriesList)
        {
            calledShowDefaultCategories = true;
        }

        public void ShowDefaultDateTime()
        {
            calledShowDefaultTime = true;
        }

        public void ShowError(string error)
        {
            calledShowError = true;
        }

        public void ShowMessage(string message)
        {
            calledShowMessage = true;
        }
    }

    public class UnitTestUpateEventPresenter
    {
        [Fact]
        public void Initialization_CallsShowDefaults()
        {
            // Arrange
            TestUpdateView testview = new TestUpdateView();

            // Act
            UpdateEventsWindowPresenter presenter = new UpdateEventsWindowPresenter(testview, new HomeCalendar("testPath"));

            // Assert
            Assert.True(testview.calledShowDefaultTime);
            Assert.True(testview.calledShowDefaultCategories);
            Assert.True(testview.calledPopulateFields);
        }

        [Fact]
        public void UpdateEvent_ValidInput()
        {
            // Arrange
            TestUpdateView testview = new TestUpdateView();
            UpdateEventsWindowPresenter presenter = new UpdateEventsWindowPresenter(testview, new HomeCalendar("testPath"));
            DatePicker datedp = new DatePicker();
            datedp.SelectedDate = DateTime.Now;
            TextBox duration = new TextBox();
            duration.Text = "30";

            ComboBox startHour = new ComboBox();
            ComboBox startMin = new ComboBox();

            startHour.ItemsSource = new List<int> { 0 }; // Providing a list with a single value 0

            startHour.SelectedIndex = 0;
            startMin.SelectedIndex = 0;

            // Act
            presenter.UpdateEvent(0, datedp, 0, duration, "Details", startHour, startMin);

            // Assert
            Assert.True(testview.calledShowMessage);
        }

        [Fact]
        public void UpdateEvent_InvalidDurationInputShowsError()
        {
            // Arrange
            TestUpdateView testview = new TestUpdateView();
            UpdateEventsWindowPresenter presenter = new UpdateEventsWindowPresenter(testview, new HomeCalendar("testPath"));
            DatePicker datedp = new DatePicker();
            datedp.SelectedDate = DateTime.Now;
            TextBox duration = new TextBox();
            // negative duration should not be allowed
            duration.Text = "-30";

            ComboBox startHour = new ComboBox();
            ComboBox startMin = new ComboBox();

            startHour.ItemsSource = new List<int> { 0 }; 
            startHour.SelectedIndex = 0;
            startMin.SelectedIndex = 0;

            // Act
            presenter.UpdateEvent(0, datedp, 0, duration, "Details", startHour, startMin);

            // Assert
            Assert.True(testview.calledShowError);
        }

        [Fact]
        public void UpdateEvent_InvalidDateInputShowsError()
        {
            // Arrange
            TestUpdateView testview = new TestUpdateView();
            UpdateEventsWindowPresenter presenter = new UpdateEventsWindowPresenter(testview, new HomeCalendar("testPath"));
            DatePicker datedp = new DatePicker();
            // No date should not be allowed
            datedp.SelectedDate = null;
            TextBox duration = new TextBox();
            duration.Text = "30";

            ComboBox startHour = new ComboBox();
            ComboBox startMin = new ComboBox();

            startHour.ItemsSource = new List<int> { 0 };
            startHour.SelectedIndex = 0;
            startMin.SelectedIndex = 0;

            // Act
            presenter.UpdateEvent(0, datedp, 0, duration, "Details", startHour, startMin);

            // Assert
            Assert.True(testview.calledShowError);
        }
    }
}
