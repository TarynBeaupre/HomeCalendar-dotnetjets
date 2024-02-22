using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;

namespace CalendarCodeTests
{
    [Collection("Sequential")]
    public class TestHomeCalendar_GetCalendarItems
    {
        string testInputFile = TestConstants.testEventsInputFile;
        

        // ========================================================================
        // Get Events Method tests
        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItems_NoStartEnd_NoFilter()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB,inFile,false);
            List<Event> listEvents = homeCalendar.events.List();
            List<Category> listCategories = homeCalendar.categories.List();

            // Act
            List<CalendarItem> CalendarItems = homeCalendar.GetCalendarItems(null, null, false, 9);

            // Assert
            Assert.Equal(listEvents.Count, CalendarItems.Count);
            foreach (Event Event in listEvents)
            {
                CalendarItem CalendarItem = CalendarItems.Find(b => b.EventID == Event.Id);
                Category category = listCategories.Find(c => c.Id == Event.Category);
                Assert.Equal(CalendarItem.Category, category.Description);
                Assert.Equal(CalendarItem.CategoryID, Event.Category);
                Assert.Equal(CalendarItem.DurationInMinutes, Event.DurationInMinutes);
                Assert.Equal(CalendarItem.ShortDescription, Event.Details);
            }
        }

        [Fact]
        public void HomeCalendarMethod_GetCalendarItems_NoStartEnd_NoFilter_VerifyBusyTimeProperty()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);

            // Act
            List<CalendarItem> CalendarItems = homeCalendar.GetCalendarItems(null, null, false, 9);

            // Assert
            double busyTime = 0;
            foreach (CalendarItem CalendarItem in CalendarItems)
            {
                busyTime = busyTime + CalendarItem.DurationInMinutes;
                Assert.Equal(busyTime, CalendarItem.BusyTime);
            }

        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItems_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);
            int filterCategory = 2;
            List<Event> listEvents = TestConstants.filteredbyCat2();
            List<Category> listCategories = homeCalendar.categories.List();

            // Act
            List<CalendarItem> CalendarItems = homeCalendar.GetCalendarItems(null, null, true, filterCategory);

            // Assert
            Assert.Equal(listEvents.Count, CalendarItems.Count);
            foreach (Event Event in listEvents)
            {
                CalendarItem CalendarItem = CalendarItems.Find(b => b.EventID == Event.Id);
                Category category = listCategories.Find(c => c.Id == Event.Category);
                Assert.Equal(CalendarItem.Category, category.Description);
                Assert.Equal(CalendarItem.CategoryID, Event.Category);
                Assert.Equal(CalendarItem.DurationInMinutes, Event.DurationInMinutes);
                Assert.Equal(CalendarItem.ShortDescription, Event.Details);
            }
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItems_2018_filterDate()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);
            List<Event> listEvents = TestConstants.filteredbyYear2018();
            List<Category> listCategories = homeCalendar.categories.List();

            // Act
            List<CalendarItem> CalendarItems = homeCalendar.GetCalendarItems(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), false, 0);

            // Assert
            Assert.Equal(listEvents.Count, CalendarItems.Count);
            foreach (Event Event in listEvents)
            {
                CalendarItem CalendarItem = CalendarItems.Find(b => b.EventID == Event.Id);
                Category category = listCategories.Find(c => c.Id == Event.Category);
                Assert.Equal(CalendarItem.Category, category.Description);
                Assert.Equal(CalendarItem.CategoryID, Event.Category);
                Assert.Equal(CalendarItem.DurationInMinutes, Event.DurationInMinutes);
                Assert.Equal(CalendarItem.ShortDescription, Event.Details);
            }
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItems_NoStartEnd_verifyBusyTime()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);
            List<Event> listEvents = TestConstants.filteredbyCat2();
            List<Category> listCategories = homeCalendar.categories.List();

            // Act
            List<CalendarItem> CalendarItems = homeCalendar.GetCalendarItems(null, null,  true, 2);
            double total = CalendarItems[CalendarItems.Count-1].BusyTime;


            // Assert
            Assert.Equal(TestConstants.filteredbyCat2Total, total);
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItems_2018_filterDateAndCat10()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);
            List<Event> listEvents = TestConstants.filteredbyYear2018AndCategory3();
            List<Category> listCategories = homeCalendar.categories.List();

            // Act
            List<CalendarItem> CalendarItems = homeCalendar.GetCalendarItems(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), true, 3);

            // Assert
            Assert.Equal(listEvents.Count, CalendarItems.Count);
            foreach (Event Event in listEvents)
            {
                CalendarItem CalendarItem = CalendarItems.Find(b => b.EventID == Event.Id);
                Category category = listCategories.Find(c => c.Id == Event.Category);
                Assert.Equal(CalendarItem.Category, category.Description);
                Assert.Equal(CalendarItem.CategoryID, Event.Category);
                Assert.Equal(CalendarItem.DurationInMinutes, Event.DurationInMinutes);
                Assert.Equal(CalendarItem.ShortDescription, Event.Details);
            }
        }
    }
}

