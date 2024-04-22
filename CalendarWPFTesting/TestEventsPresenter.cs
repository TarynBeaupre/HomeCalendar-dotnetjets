using Calendar;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendarWPFTesting
{
    public class TestEventsPresenter : EventsViewInterface
    {
        public bool calledShowError, calledShowMessage, calledShowDefaultCat, calledShowDefaultDate, calledReset;

        public void ShowError(string message)
        {

        }
        public void ShowMessage(string message)
        {

        }
        public void ShowDefaultCategories(List<Category> categoriesList)
        {

        }
        public void ShowDefaultDateTime()
        {

        }
        void ResetEventForm();

        [Fact]
        public void AddNewEvent_ValidInput_CallsModelAddEventAndShowsMessage()
        {
            // Arrange
            string details = "Test event";
            int categoryId = 1;
            DateTime start = DateTime.Now;
            double duration = 60;

            // Act
            presenter.AddNewEvent(details, categoryId, start, duration);

            // Assert
            mockView.Verify(v => v.ShowMessage("Event successfully added!"), Times.Once);
            mockView.Verify(v => v.ResetEventForm(), Times.Once);
        }

        [Fact]
        public void AddNewEvent_NullStart_CallsModelAddEventWithCurrentDate()
        {
            // Arrange
            string details = "Test event";
            int categoryId = 1;
            double duration = 60;

            // Act
            presenter.AddNewEvent(details, categoryId, null, duration);

            // Assert
            mockView.Verify(v => v.ShowMessage("Event successfully added!"), Times.Once);
            mockView.Verify(v => v.ResetEventForm(), Times.Once);
        }

        [Fact]
        public void AddNewEvent_InvalidInput_ShowsError()
        {
            // Arrange
            string details = "Test event";
            int categoryId = 1;
            DateTime start = DateTime.Now;
            double duration = -10; // Invalid duration

            // Act
            presenter.AddNewEvent(details, categoryId, start, duration);

            // Assert
            mockView.Verify(v => v.ShowError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void AddNewCategory_ValidInput_CallsModelAddCategoryAndShowsMessage()
        {
            // Arrange
            string categoryName = "Test Category";

            // Act
            presenter.AddNewCategory(categoryName);

            // Assert
            mockView.Verify(v => v.ShowMessage($"A new category {categoryName} has been added!"), Times.Once);
        }

        [Fact]
        public void AddNewCategory_InvalidInput_ShowsError()
        {
            // Arrange
            string categoryName = null; // Invalid category name

            // Act
            presenter.AddNewCategory(categoryName);

            // Assert
            mockView.Verify(v => v.ShowError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetDefaultCategories_CallsModelListAndShowsCategories()
        {
            // Arrange
            var categoriesList = new List<Category>(); // Mock categories list
            mockView.Setup(v => v.ShowDefaultCategories(categoriesList)).Verifiable();

            // Act
            presenter.GetDefaultCategories();

            // Assert
            mockView.Verify();
        }
    }
}