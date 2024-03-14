using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using static Calendar.Category;
using System.Net.NetworkInformation;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Events
    //        - A collection of Event items,
    //        - Read / write to database
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
        // Constructor
        // ====================================================================
        /// <summary>
        /// Creates a new Events instance and sets up the database connection to use the events table.
        /// </summary>
        /// <param name="eventsConnection">A valid database connection</param>
        public Events(SQLiteConnection eventsConnection) 
        {
            _Connection = eventsConnection;
        }

        /// <summary>
        /// Adds an Event to the events table in the database.
        /// </summary>
        /// <param name="date">A start date and time of the evemt.</param>
        /// <param name="category">The id of the category of the event.</param>
        /// <param name="duration">The duration of the event in minutes.</param>
        /// <param name="details">The details of the event.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Event event = new Event(existingEvent);
        /// updateEvent = event.Add(2022-01-31, 2, 320, "Doing cool programming stuff")
        /// ]]></code></example>
        public void Add(DateTime date, int category, double duration, string details)
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
        // Update Event
        // ====================================================================
        /// <summary>
        /// Updates an Event in the events table in the database.
        /// </summary>
        /// <param name="id">Unique event identifier.</param>
        /// <param name="date">Start date and time for event.</param>
        /// <param name="category">Category Id of the event.</param>
        /// <param name="duration">Duration in minute of event.</param>
        /// <param name="details">Details of the event.</param>
        /// <example>
        /// <code>
        /// For the example below, assume we have an already existing events in the database table
        /// <![CDATA[
        /// Event event = new Event(existingEvent);
        /// updateEvent = event.Update(1, 2022-01-31, 2, 320, "Doing cool programming stuff")
        /// ]]></code></example>
        public void Update(int id, DateTime date, int category, double duration, string details)
        {
            try
            {
                var con = _Connection;
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = $"UPDATE events SET Date = @date, Details = @details, Duration = @duration, CategoryId = @category WHERE Id = @id";
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@details", details);
                cmd.Parameters.AddWithValue("@duration", duration);
                cmd.Parameters.AddWithValue("@categoryid", category);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // ====================================================================
        // Delete Event
        // ====================================================================
        /// <summary>
        /// Removes a specific Event, given an id, from the events table in the database.
        /// </summary>
        /// <param name="Id">The id of the event to remove.</param>
        /// <exception cref="Exception">Thrown if there was no event at the given id or if the id was out of range.</exception>
        /// <example>
        /// <code>
        /// For the example below, assume we have an already existing event object
        /// <![CDATA[
        /// Event event = new Event(existingEvent)
        /// int specificId = 2
        /// specificEvent = event.Delete(specificId)
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

        /// <summary>
        /// Returns a specific Event, given an id, from the events table in the database. 
        /// </summary>
        /// <param name="Id">The Event Id.</param>
        /// <returns>The retrieved Event object.</returns>
        /// <example>
        /// <code>
        /// For the example below, assume we have an already existing event object
        /// <![CDATA[
        /// Event event = new Event(existingEvent)
        /// int specificId = 2
        /// specificEvent = event.GetEventFromId(specificId)
        /// ]]></code></example>
        public Event GetEventFromId(int id)
        {
            int foundId = 0;
            DateTime date = DateTime.Now;
            int category = 0;
            double duration = 0.0;
            string details = "";
            var con = Connection;

            using var cmd = new SQLiteCommand(con);

            //making a reader to retrieve the categories
            cmd.CommandText = $"SELECT Id, Date, Category, Duration, Details FROM events WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                foundId = reader.GetInt32(0);
                date = reader.GetDateTime(1);
                category = reader.GetInt32(2);
                duration = reader.GetDouble(3);
                details = reader.GetString(4);
            }
            Event foundEvent = new Event(foundId, date, category, duration, details);

            return foundEvent;
        }


        // ====================================================================
        // Return list of Events
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Creates a list of events from the events table in the database.
        /// </summary>
        /// <returns>A new list of Events.</returns>
        /// <example>
        /// For the example below, assume we have an already existing event object
        /// <code>
        /// <![CDATA[
        /// Event event = new Event(existingEvent)
        /// eventList = event.List()
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
