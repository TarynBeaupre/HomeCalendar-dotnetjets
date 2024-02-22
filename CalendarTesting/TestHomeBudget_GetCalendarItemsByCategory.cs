using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;

namespace CalendarCodeTests
{
    [Collection("Sequential")]
    public class TestHomeCalendar_GetCalendarItemsByCategory
    {
        string testInputFile = TestConstants.testEventsInputFile;
        
        // ========================================================================
        // Get Events By Month Method tests
        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItemsByCategory_NoStartEnd_NoFilter()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);
            int maxRecords = TestConstants.CalendarItemsByCategory_MaxRecords;
            CalendarItemsByCategory firstRecord = TestConstants.CalendarItemsByCategory_FirstRecord;

            // Act
            List<CalendarItemsByCategory> CalendarItemsByCategory = homeCalendar.GetCalendarItemsByCategory(null, null, false, 9);
            CalendarItemsByCategory firstRecordTest = CalendarItemsByCategory[0];

            // Assert
            Assert.Equal(maxRecords, CalendarItemsByCategory.Count);

            // verify 1st record
            Assert.Equal(firstRecord.Category, firstRecordTest.Category);
            Assert.Equal(firstRecord.TotalBusyTime, firstRecordTest.TotalBusyTime);
            Assert.Equal(firstRecord.Items.Count, firstRecordTest.Items.Count);
            for (int record = 0; record < firstRecord.Items.Count; record++)
            {
                CalendarItem validItem = firstRecord.Items[record];
                CalendarItem testItem = firstRecordTest.Items[record];
                Assert.Equal(validItem.DurationInMinutes, testItem.DurationInMinutes);
                Assert.Equal(validItem.CategoryID, testItem.CategoryID);
                Assert.Equal(validItem.EventID, testItem.EventID);

            }
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItemsByCategory_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);
            int maxRecords14 = TestConstants.CalendarItemsByCategory11;
            int maxRecords20 = TestConstants.CalendarItemsByCategory20;

            // Act
            List<CalendarItemsByMonth> CalendarItemsByCategory = homeCalendar.GetCalendarItemsByMonth(null, null, true, 11);

            // Assert
            Assert.Equal(maxRecords14, CalendarItemsByCategory.Count);


            // Act
            CalendarItemsByCategory = homeCalendar.GetCalendarItemsByMonth(null, null, true, 20);

            // Assert
            Assert.Equal(maxRecords20, CalendarItemsByCategory.Count);

        }
        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_GetCalendarItemsByCategory_2018_filterDateAndCat2()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);
        List<CalendarItemsByCategory> validCalendarItemsByCategory = TestConstants.getCalendarItemsByCategory2018_Cat2();
        CalendarItemsByCategory firstRecord = validCalendarItemsByCategory[0];

        // Act
        List<CalendarItemsByCategory> CalendarItemsByCategory = homeCalendar.GetCalendarItemsByCategory(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), true, 2);
        CalendarItemsByCategory firstRecordTest = CalendarItemsByCategory[0];

        // Assert
        Assert.Equal(validCalendarItemsByCategory.Count, CalendarItemsByCategory.Count);

        // verify 1st record
        Assert.Equal(firstRecord.Category, firstRecordTest.Category);
        Assert.Equal(firstRecord.TotalBusyTime, firstRecordTest.TotalBusyTime);
        Assert.Equal(firstRecord.Items.Count, firstRecordTest.Items.Count);
        for (int record = 0; record < firstRecord.Items.Count; record++)
        {
            CalendarItem validItem = firstRecord.Items[record];
            CalendarItem testItem = firstRecordTest.Items[record];
            Assert.Equal(validItem.DurationInMinutes, testItem.DurationInMinutes);
            Assert.Equal(validItem.CategoryID, testItem.CategoryID);
            Assert.Equal(validItem.EventID, testItem.EventID);

        }
    }


    // ========================================================================

    [Fact]
        public void HomeCalendarMethod_GetCalendarItemsByCategory_2018_filterDate()
        {
            // Arrange
            string folder = TestConstants.GetSolutionDir();
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeCalendar homeCalendar = new HomeCalendar(messyDB, inFile, false);
            List<CalendarItemsByCategory> validCalendarItemsByCategory = TestConstants.getCalendarItemsByCategory2018();
            CalendarItemsByCategory firstRecord = validCalendarItemsByCategory[0];


            // Act
            List<CalendarItemsByCategory> CalendarItemsByCategory = homeCalendar.GetCalendarItemsByCategory(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), false, 9);
            CalendarItemsByCategory firstRecordTest = CalendarItemsByCategory[0];

            // Assert
            Assert.Equal(validCalendarItemsByCategory.Count, CalendarItemsByCategory.Count);

            // verify 1st record
            Assert.Equal(firstRecord.Category, firstRecordTest.Category);
            Assert.Equal(firstRecord.TotalBusyTime, firstRecordTest.TotalBusyTime);
            Assert.Equal(firstRecord.Items.Count, firstRecordTest.Items.Count);
            for (int record = 0; record < firstRecord.Items.Count; record++)
            {
                CalendarItem validItem = firstRecord.Items[record];
                CalendarItem testItem = firstRecordTest.Items[record];
                Assert.Equal(validItem.DurationInMinutes, testItem.DurationInMinutes);
                Assert.Equal(validItem.CategoryID, testItem.CategoryID);
                Assert.Equal(validItem.EventID, testItem.EventID);

            }
        }
    }
}

