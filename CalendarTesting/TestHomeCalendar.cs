using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;

namespace CalendarCodeTests
{
    public class TestHomeCalendar
    {
        string testInputFile = TestConstants.testCalendarFile;
        

        // ========================================================================

        [Fact]
        public void HomeCalendarObject_New_NoFileSpecified()
        {
            // Arrange

            // Act
            HomeCalendar homeCalendar  = new HomeCalendar();

            // Assert 
            Assert.IsType<HomeCalendar>(homeCalendar);

            Assert.True(typeof(HomeCalendar).GetProperty("FileName").CanWrite == false, "Filename read only");
            Assert.True(typeof(HomeCalendar).GetProperty("DirName").CanWrite == false, "Dirname read only");
            Assert.True(typeof(HomeCalendar).GetProperty("PathName").CanWrite == false, "Pathname read only");
            Assert.True(typeof(HomeCalendar).GetProperty("categories").CanWrite == false, "categories read only");
            Assert.True(typeof(HomeCalendar).GetProperty("events").CanWrite == false, "Events read only");

            Assert.Empty(homeCalendar.events.List());
            Assert.NotEmpty(homeCalendar.categories.List());
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarObject_New_WithFilename()
        {
            // Arrange
            string file = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            int numEvents = TestConstants.numberOfEventsInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;

            // Act
            HomeCalendar homeCalendar = new HomeCalendar(file);

            // Assert 
            Assert.IsType<HomeCalendar>(homeCalendar);
            Assert.Equal(numEvents, homeCalendar.events.List().Count);
            Assert.Equal(numCategories, homeCalendar.categories.List().Count);

        }

        // ========================================================================

        [Fact]
        public void HomeBudgeMethod_ReadFromFile_ReadsCorrectData()
        {
            // Arrange
            string file = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            int numEvents = TestConstants.numberOfEventsInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;
            Event firstEventInFile = TestConstants.firstEventInFile;
            Category firstCategoryInFile = TestConstants.firstCategoryInFile;
            HomeCalendar homeCalendar = new HomeCalendar();

            // Act
            homeCalendar.ReadFromFile(file);
            Event firstEvent = homeCalendar.events.List()[0];
            Category firstCategory = homeCalendar.categories.List()[0];


            // Assert 
            Assert.Equal(numEvents, homeCalendar.events.List().Count);
            Assert.Equal(numCategories, homeCalendar.categories.List().Count);
            Assert.Equal(firstEventInFile.Details, firstEvent.Details);
            Assert.Equal(firstCategoryInFile.Description, firstCategory.Description);
        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_SaveToFile_FilesAreCreated()
        {
            // Arrange
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            int numEvents = TestConstants.numberOfEventsInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;

            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            String outputFile = TestConstants.GetSolutionDir() + "\\" + TestConstants.outputTestCalendarFile;

            String path = Path.GetDirectoryName(Path.GetFullPath(outputFile));
            String file = Path.GetFileNameWithoutExtension(outputFile);
            String ext = Path.GetExtension(outputFile);
            String output_Calendar = outputFile;
            String output_Events = Path.Combine(path, file + "_events.evts");
            String output_categories = Path.Combine(path, file + "_categories.cats");

            File.Delete(output_Calendar);
            File.Delete(output_Events);
            File.Delete(output_categories);

            // Act
            homeCalendar.SaveToFile(outputFile);

            // Assert 

            Assert.True(File.Exists(output_Calendar), output_Calendar + " file exists");
            Assert.True(File.Exists(output_Events), output_Events + " file exists");
            Assert.True(File.Exists(output_categories), output_categories + "file exists");

        }

        // ========================================================================

        [Fact]
        public void HomeCalendarMethod_SaveToFile_FilesAreWrittenTo()
        {
            // Arrange
            string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
            int numEvents = TestConstants.numberOfEventsInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;

            HomeCalendar homeCalendar = new HomeCalendar(inFile);
            String outputFile = TestConstants.GetSolutionDir() + "\\" + TestConstants.outputTestCalendarFile;

            String path = Path.GetDirectoryName(Path.GetFullPath(outputFile));
            String file = Path.GetFileNameWithoutExtension(outputFile);
            String ext = Path.GetExtension(outputFile);
            String output_Calendar = outputFile;
            String output_Events = Path.Combine(path, file + "_events.evts");
            String output_categories = Path.Combine(path, file + "_categories.cats");
            string input_Events = Path.Combine(TestConstants.GetSolutionDir(), TestConstants.testEventsInputFile);
            string input_categories = Path.Combine(TestConstants.GetSolutionDir(), TestConstants.testCategoriesInputFile);

            File.Delete(output_Calendar);
            File.Delete(output_Events);
            File.Delete(output_categories);

            // Act
            homeCalendar.SaveToFile(outputFile);

            // Assert 
            Assert.True(File.Exists(output_Calendar), output_Calendar + " file exists");
            Assert.True(File.Exists(output_Events), output_Events + " file exists");
            Assert.True(File.Exists(output_categories), output_categories + "file exists");

            string[] contents = File.ReadAllLines(output_Calendar);
            Assert.True(contents.Length==2);
            Assert.True(contents[0] == file + "_categories.cats", "categorie file " + contents[0]);
            Assert.True(contents[1] == file + "_events.evts", "Events file " + contents[1]);

            Assert.True(File.Exists(output_Calendar));
            Assert.True( TestConstants.FileSameSize(input_categories, output_categories),
                "Same number of bytes in categories file, assume files are same - " +
                "testing for accuracy is in categories test file");
            Assert.True(TestConstants.FileSameSize(input_Events, output_Events),
                 "Same number of bytes in Events file, assume files are same - " +
                 "testing for accuracy is in Events test file");

        }
    }
}

