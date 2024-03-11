using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Events
    //        - A collection of Event items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    /// Defines the Events to classify Calendar items.
    /// </summary>
    public class Events
    {
        private SQLiteConnection _Connection;

        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Get the database connection.
        /// </summary>
        /// <value>A database connection. Cannot be null and needs to be valid.</value>
        public SQLiteConnection Connection { get { return _Connection; } }

        // ====================================================================
        // Add Event
        // ====================================================================
        private void Add(Event exp)
        {
            var con = Connection;
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = "INSERT INTO events(Id, StartDateTime, Details, DurationInMinutes, CategoryId) VALUES(@id, @startdatetime, @details, @durationminutes, @categoryid)";
            cmd.Parameters.AddWithValue("@id", exp.Id);
            cmd.Parameters.AddWithValue("@startdatetime", exp.StartDateTime);
            cmd.Parameters.AddWithValue("@details", exp.Details);
            cmd.Parameters.AddWithValue("@durationminutes", exp.DurationInMinutes);
            cmd.Parameters.AddWithValue("@categoryid", exp.Category);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds a Event to the Events List.
        /// </summary>
        /// <param name="date">A start date and time of the evemt.</param>
        /// <param name="category">The id of the category of the event.</param>
        /// <param name="duration">The duration of the event in minutes.</param>
        /// <param name="details">The details of the event.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Events events = new Events();
        /// int eventCategoryId = 5;
        /// double eventDuration = 60;
        /// events.Add(DateTime.Now, eventCategoryId, eventDuration, "Homework");
        /// ]]></code>
        /// </example>
        public void Add(DateTime date, int category, Double duration, String details)
        {
            try
            {
                //Opening connection
                var con = Connection;
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = "INSERT INTO events(StartDateTime, Details, DurationInMinutes, CategoryId) VALUES(@date, @details, @duration, @categoryid)";
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@details", details);
                cmd.Parameters.AddWithValue("@duration", duration);
                cmd.Parameters.AddWithValue("@categoryid", category);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // ====================================================================
        // Delete Event
        // ====================================================================
        /// <summary>
        /// Removes an Event from the Events list at a given Id.
        /// </summary>
        /// <param name="Id">The id of the event to remove.</param>
        /// <exception cref="Exception">Thrown if there was no event at the given id or if the id was out of range.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try 
        /// {
        ///     Events events = new Events();
        ///     int eventIdToDelete = 5;
        ///     events.Delete(eventIdToDelete);
        /// }
        /// catch (Exception ex)
        ///     Console.WriteLine(ex.Message)
        /// ]]></code></example>
        public void Delete(int Id)
        {
            try
            {
                //connect to category
                var con = Database.dbConnection;
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = $"DELETE FROM events WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // ====================================================================
        // Return list of Events
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Creates a new copy of an existing list of Events. User cannot modify this instance.
        /// </summary>
        /// <returns>A new list of Events.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///  Events events = new Events();
        ///  events.Add(DateTime.Now, (int) Category.CategoryType.Event, 30, "documentation assignment");
        ///  
        ///  List<Event> list = events.List();
        ///  foreach (Event anEvent in list)
        ///     Console.WriteLine(anEvent.Details);
        /// ]]>
        /// </code>
        /// </example>
        public List<Event> List()
        {
            List<Event> newList = new List<Event>();

            // Retrieve events from db file 
            string query = "SELECT Id, StartDateTime, Details, DurationInMinutes, CategoryId FROM events";

            using var cmd = new SQLiteCommand(query, Connection);
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                DateTime StartDateTime = reader.GetDateTime(1);
                string details = reader.GetString(2);
                double duration = reader.GetDouble(3);
                int category = reader.GetInt32(4);

                Event newEvent = new Event(id, StartDateTime, category, duration, details);
                newList.Add(newEvent);
            }
            return newList;
        }
    }
}
