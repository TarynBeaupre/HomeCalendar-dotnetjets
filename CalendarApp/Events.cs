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
        private static String DefaultFileName = "calendar.txt";
        private List<Event> _Events = new List<Event>();
        private string _FileName;
        private string _DirName; private SQLiteConnection _Connection;

        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Get the database connection.
        /// </summary>
        /// <value>A database connection. Cannot be null and needs to be valid.</value>
        //TODO: verify the connection
        public SQLiteConnection Connection { get { return _Connection; } }
        /// <summary>
        /// Gets the returned file name field value.
        /// </summary>
        /// <value>The file name associated with the events.</value>
        public String FileName { get { return _FileName; } }
        /// <summary>
        /// Gets the returned directory name field value.
        /// </summary>
        /// <value>The directory name containing the associated files.</value>
        public String DirName { get { return _DirName; } }


        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================
        /// <summary>
        /// Reads data from a file.
        /// </summary>
        /// <param name="filepath">The file path to read data from. If null, uses the default file path.</param>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <exception cref="Exception">Thrown if unable to read the files correctly.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     Events events = new Events();
        ///     events.ReadFromFile("events.txt");
        /// }
        /// catch (Exception ex){
        ///     Console.WriteLine(ex.Message);
        /// }
        /// ]]></code></example>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current Events,
            // so clear out any old definitions
            // ---------------------------------------------------------------
            _Events.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyReadFromFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // read the Events from the xml file
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use?
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        /// <summary>
        /// Writes data to a file.
        /// </summary>
        /// <param name="filepath">The file path of the file to write data to. If null, uses the default file path.</param>
        /// <exception cref="Exception">Thrown if the file doesn't exist or if unable to write data to the file.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     Events events = new Events();
        ///     events.SaveToFile("savedEvents.txt")
        /// }
        /// catch (Exception ex){
        ///     Console.WriteLine(ex.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>

        public void SaveToFile(String filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyWriteToFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }



        // ====================================================================
        // Add Event
        // ====================================================================
        private void Add(Event exp)
        {
            var con = _Connection;
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = "INSERT INTO events(Id, StartDateTime, Details, DurationInMinutes, CategoryId) VALUES(@id, @startdatetime, @details, @durationminutes, @categoryid) RETURNING ID";
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
                var con = _Connection;
                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = "INSERT INTO events(StartDateTime, Details, DurationInMinutes, CategoryId) VALUES(@date, @details, @duration, @categoryid) RETURNING ID";
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
        /// Events events = new Events();
        /// int eventIdToDelete = 5;
        /// events.Delete(eventIdToDelete);
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

            using var cmd = new SQLiteCommand(query, _Connection);
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


        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                // Loop over each Event
                foreach (XmlNode Event in doc.DocumentElement.ChildNodes)
                {
                    // set default Event parameters
                    int id = int.Parse((((XmlElement)Event).GetAttributeNode("ID")).InnerText);
                    String description = "";
                    DateTime date = DateTime.Parse("2000-01-01");
                    int category = 0;
                    Double DurationInMinutes = 0.0;

                    // get Event parameters
                    foreach (XmlNode info in Event.ChildNodes)
                    {
                        switch (info.Name)
                        {
                            case "StartDateTime":
                                date = DateTime.Parse(info.InnerText);
                                break;
                            case "DurationInMinutes":
                                DurationInMinutes = Double.Parse(info.InnerText);
                                break;
                            case "Details":
                                description = info.InnerText;
                                break;
                            case "Category":
                                category = int.Parse(info.InnerText);
                                break;
                        }
                    }

                    // have all info for Event, so create new one
                    this.Add(new Event(id, date, category, DurationInMinutes, description));

                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadFromFileException: Reading XML " + e.Message);
            }
        }


        // ====================================================================
        // write to an XML file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            // ---------------------------------------------------------------
            // loop over all categories and write them out as XML
            // ---------------------------------------------------------------
            try
            {
                // create top level element of Events
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Events></Events>");

                // foreach Category, create an new xml element
                foreach (Event exp in _Events)
                {
                    // main element 'Event' with attribute ID
                    XmlElement ele = doc.CreateElement("Event");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = exp.Id.ToString();
                    ele.SetAttributeNode(attr);
                    doc.DocumentElement.AppendChild(ele);

                    // child attributes (date, description, DurationInMinutes, category)
                    XmlElement d = doc.CreateElement("StartDateTime");
                    XmlText dText = doc.CreateTextNode(exp.StartDateTime.ToString("M\\/d\\/yyyy h:mm:ss tt"));
                    ele.AppendChild(d);
                    d.AppendChild(dText);

                    XmlElement de = doc.CreateElement("Details");
                    XmlText deText = doc.CreateTextNode(exp.Details);
                    ele.AppendChild(de);
                    de.AppendChild(deText);

                    XmlElement a = doc.CreateElement("DurationInMinutes");
                    XmlText aText = doc.CreateTextNode(exp.DurationInMinutes.ToString());
                    ele.AppendChild(a);
                    a.AppendChild(aText);

                    XmlElement c = doc.CreateElement("Category");
                    XmlText cText = doc.CreateTextNode(exp.Category.ToString());
                    ele.AppendChild(c);
                    c.AppendChild(cText);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("SaveToFileException: Reading XML " + e.Message);
            }
        }

    }
}

