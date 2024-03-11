using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Calendar;
using System.Data.SQLite;

namespace CalendarCodeTests
{
    public class TestEvents
    {
        int numberOfEventsInFile = TestConstants.numberOfEventsInFile;
        String testInputFile = TestConstants.testEventsInputFile;
        int maxIDInEventFile = TestConstants.maxIDInEventFile;
        Event firstEventInFile = new Event(1, new DateTime(2021, 1, 10), 3, 40, "App Dev Homework");


        // ========================================================================

        [Fact]
        public void EventsObject_New()
        {
            // Arrange

            // Act
            Events Events = new Events();

            // Assert 
            Assert.IsType<Events>(Events);

            Assert.True(typeof(Events).GetProperty("Connection").CanWrite == false);

        }


        // ========================================================================

        [Fact]
        public void EventsMethod_ReadFromFile_NotExist_ThrowsException()
        {
            // Arrange
            String badDB = "CeciNestPasUneDB.txt";

            // Act and Assert
            Assert.Throws<System.IO.FileNotFoundException>(() => new Events(badDB));

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_ReadFromFile_ValidateCorrectDataWasRead()
        {
            //// Arrange
            //String dir = TestConstants.GetSolutionDir();
            //Events Events = new Events();

            //// Act
            //Events.ReadFromFile(dir + "\\" + testInputFile);
            //List<Event> list = Events.List();
            //Event firstEvent = list[0];


            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String goodDBCopy = $"{folder}\\testDBCopy.db";
            System.IO.File.Copy(goodDB, goodDBCopy, true);
            Database.existingDatabase(goodDBCopy);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            var events = new Events(conn);

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);
            Assert.Equal(firstEventInFile.Id, firstEvent.Id);
            Assert.Equal(firstEventInFile.DurationInMinutes, firstEvent.DurationInMinutes);
            Assert.Equal(firstEventInFile.Details, firstEvent.Details);
            Assert.Equal(firstEventInFile.Category, firstEvent.Category);

            String fileDir = Path.GetFullPath(Path.Combine(Events.DirName, ".\\"));
            Assert.Equal(dir, fileDir);
            Assert.Equal(testInputFile, Events.FileName);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_List_ReturnsListOfEvents()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Events Events = new Events();
            Events.ReadFromFile(dir + "\\" + testInputFile);

            // Act
            List<Event> list = Events.List();

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_List_ModifyListDoesNotModifyEventsInstance()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Events Events = new Events();
            Events.ReadFromFile(dir + "\\" + testInputFile);
            List<Event> list = Events.List();

            // Act
            list[0].DurationInMinutes = list[0].DurationInMinutes + 21.03; 

            // Assert
            Assert.NotEqual(list[0].DurationInMinutes, Events.List()[0].DurationInMinutes);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Add()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Events Events = new Events();
            Events.ReadFromFile(dir + "\\" + testInputFile);
            int category = 57;
            double DurationInMinutes = 98.1;

            // Act
            Events.Add(DateTime.Now,category,DurationInMinutes,"new Event");
            List<Event> EventsList = Events.List();
            int sizeOfList = Events.List().Count;

            // Assert
            Assert.Equal(numberOfEventsInFile+1, sizeOfList);
            Assert.Equal(maxIDInEventFile + 1, EventsList[sizeOfList - 1].Id);
            Assert.Equal(DurationInMinutes, EventsList[sizeOfList - 1].DurationInMinutes);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Delete()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Events Events = new Events();
            Events.ReadFromFile(dir + "\\" + testInputFile);
            int IdToDelete = 3;

            // Act
            Events.Delete(IdToDelete);
            List<Event> EventsList = Events.List();
            int sizeOfList = EventsList.Count;

            // Assert
            Assert.Equal(numberOfEventsInFile - 1, sizeOfList);
            Assert.False(EventsList.Exists(e => e.Id == IdToDelete), "correct Event item deleted");

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Events Events = new Events();
            Events.ReadFromFile(dir + "\\" + testInputFile);
            int IdToDelete = 1006;
            int sizeOfList = Events.List().Count;

            // Act
            try
            {
                Events.Delete(IdToDelete);
                Assert.Equal(sizeOfList, Events.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }


        // ========================================================================

        [Fact]
        public void EventMethod_WriteToFile()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Events Events = new Events();
            Events.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.EventOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);

            // Act
            Events.SaveToFile(outputFile);

            // Assert
            Assert.True(File.Exists(outputFile), "output file created");
            Assert.True(TestConstants.FileEquals(dir + "\\" + testInputFile, outputFile), "Input /output files are the same");
            String fileDir = Path.GetFullPath(Path.Combine(Events.DirName, ".\\"));
            Assert.Equal(dir, fileDir);
            Assert.Equal(fileName, Events.FileName);

            // Cleanup
            if (TestConstants.FileEquals(dir + "\\" + testInputFile, outputFile))
            {
                File.Delete(outputFile);
            }

        }

        // ========================================================================

        [Fact]
        public void EventMethod_WriteToFile_VerifyNewEventWrittenCorrectly()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Events Events = new Events();
            Events.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.EventOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);

            // Act
            Events.Add(DateTime.Now, 14, 35.27, "McDonalds");
            List<Event> listBeforeSaving = Events.List();
            Events.SaveToFile(outputFile);
            Events.ReadFromFile(outputFile);
            List<Event> listAfterSaving = Events.List();

            Event beforeSaving = listBeforeSaving[listBeforeSaving.Count - 1];
            Event afterSaving = listAfterSaving.Find(e => e.Id == beforeSaving.Id);

            // Assert
            Assert.Equal(beforeSaving.Id, afterSaving.Id);
            Assert.Equal(beforeSaving.Category, afterSaving.Category);
            Assert.Equal(beforeSaving.Details, afterSaving.Details);
            Assert.Equal(beforeSaving.DurationInMinutes, afterSaving.DurationInMinutes);

        }

        // ========================================================================

        [Fact]
        public void EventMethod_WriteToFile_WriteToLastFileWrittenToByDefault()
        {
            // Arrange
            String dir = TestConstants.GetSolutionDir();
            Events Events = new Events();
            Events.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.EventOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);
            Events.SaveToFile(outputFile); // output file is now last file that was written to.
            File.Delete(outputFile);  // Delete the file

            // Act
            Events.SaveToFile(); // should write to same file as before

            // Assert
            Assert.True(File.Exists(outputFile), "output file created");
            String fileDir = Path.GetFullPath(Path.Combine(Events.DirName, ".\\"));
            Assert.Equal(dir, fileDir);
            Assert.Equal(fileName, Events.FileName);

            // Cleanup
            if (TestConstants.FileEquals(dir + "\\" + testInputFile, outputFile))
            {
                File.Delete(outputFile);
            }

        }
    }
}

