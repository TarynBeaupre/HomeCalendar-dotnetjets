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
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Events Events = new Events(conn);

            // Assert 
            Assert.IsType<Events>(Events);

            Assert.True(typeof(Events).GetProperty("Connection")!.CanWrite == false);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String goodDBCopy = $"{folder}\\testDBCopy.db";
            System.IO.File.Copy(goodDB, goodDBCopy, true);
            Database.existingDatabase(goodDBCopy);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Events events = new Events(conn);
            List<Event> list = events.List();
            Event firstEvent = list[0];

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);
            Assert.Equal(firstEventInFile.Id, firstEvent.Id);
            Assert.Equal(firstEventInFile.DurationInMinutes, firstEvent.DurationInMinutes);
            Assert.Equal(firstEventInFile.Details, firstEvent.Details);
            Assert.Equal(firstEventInFile.Category, firstEvent.Category);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_List_ReturnsListOfEvents()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String goodDBCopy = $"{folder}\\testDBCopy.db";
            System.IO.File.Copy(goodDB, goodDBCopy, true);
            Database.existingDatabase(goodDBCopy);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Events events = new Events(conn);
            List<Event> list = events.List();

            // Assert
            Assert.Equal(numberOfEventsInFile, list.Count);

        }

        // ========================================================================

        [Fact]
        public void EventsMethod_List_ModifyListDoesNotModifyEventsInstance()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String goodDBCopy = $"{folder}\\testDBCopy.db";
            System.IO.File.Copy(goodDB, goodDBCopy, true);
            Database.existingDatabase(goodDBCopy);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);

            // Act
            List<Event> list = events.List();
            list[0].DurationInMinutes = list[0].DurationInMinutes + 21.03; 

            // Assert
            Assert.NotEqual(list[0].DurationInMinutes, events.List()[0].DurationInMinutes);

        }

        [Fact]
        public void EventsMethod_UpdateCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            string details = "Shopping with friends";
            int id = 9;
            DateTime date = new DateTime(2019, 1, 11, 9, 30, 0);

            // Act
            events.Update(id, date, 3, 30.0, details);
            Event chosenEvent = Events.GetEventFromId(id);

            // Assert 
            Assert.Equal(details, chosenEvent.Details);
            Assert.Equal(date, chosenEvent.StartDateTime);
            Assert.Equal(3, chosenEvent.Category);
            Assert.Equal(30.0, chosenEvent.DurationInMinutes);

        }


        // ========================================================================

        [Fact]
        public void EventsMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String goodDBCopy = $"{folder}\\testDBCopy.db";
            System.IO.File.Copy(goodDB, goodDBCopy, true);
            Database.existingDatabase(goodDBCopy);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int category = 1;
            double DurationInMinutes = 98.1;

            // Act
            events.Add(DateTime.Now,category,DurationInMinutes,"new Event");
            List<Event> EventsList = events.List();
            int sizeOfList = events.List().Count;

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
            //String dir = TestConstants.GetSolutionDir();
            //Events Events = new Events();
            //Events.ReadFromFile(dir + "\\" + testInputFile);
            //int IdToDelete = 3;

            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String goodDBCopy = $"{folder}\\testDBCopy.db";
            System.IO.File.Copy(goodDB, goodDBCopy, true);
            Database.existingDatabase(goodDBCopy);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int IdToDelete = 3;

            // Act
            events.Delete(IdToDelete);
            List<Event> EventsList = events.List();
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
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String goodDBCopy = $"{folder}\\testDBCopy.db";
            System.IO.File.Copy(goodDB, goodDBCopy, true);
            Database.existingDatabase(goodDBCopy);
            SQLiteConnection conn = Database.dbConnection;
            Events events = new Events(conn);
            int IdToDelete = 1006;
            int sizeOfList = events.List().Count;

            // Act
            try
            {
                events.Delete(IdToDelete);
                Assert.Equal(sizeOfList, events.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }
    }
}

