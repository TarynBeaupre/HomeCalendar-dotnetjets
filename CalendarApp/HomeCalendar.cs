// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================
using System;
using System.Data.SQLite;
using System.Globalization;
using System.Security.Cryptography;

namespace Calendar
{
    // ====================================================================
    // CLASS: HomeCalendar
    //        - Combines a Categories Class and an Events Class
    //        - One File defines Category and Events File
    //        - etc
    // ====================================================================
    /// <summary>
    /// Manages the files containing <see cref="Category"/> and <see cref="Event"/> data, and combines the <see cref="Categories"/> and <see cref="Events"/> classes to be used by Calendar Items.
    /// </summary>
    public class HomeCalendar
    {
        private Categories _categories;
        private Events _events;
        private SQLiteConnection _Connection;



        // ====================================================================
        // Properties
        // ===================================================================
        // Properties (categories and events object)
        /// <summary>
        /// Gets the possible categories for Calendar Items.
        /// </summary>
        /// <value>The categories coming from the Categories class.</value>
        /// <returns>Returns a categories backing field.</returns>
        public Categories categories { get { return _categories; } }
        /// <summary>
        /// Gets and sets the database connection used to run sql queries.
        /// </summary>
        public SQLiteConnection Connection
        {
            get { return _Connection; }
            set
            {
                if (value != null)
                    _Connection = value;
                else
                    throw new ArgumentException("Connection to database cannot be null.");
            }
        }

        /// <summary>
        /// Gets the possible events for Calendar Items.
        /// </summary>
        /// <value>The events coming from the Events class.</value>
        /// <returns>Returns an events backing field.</returns>
        public Events events { get { return _events; } }

        // -------------------------------------------------------------------
        // Constructor (new... default categories, no events)
        // -------------------------------------------------------------------
        /// <summary>
        /// Creates a Home Calendar object with the default properties.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// HomeCalendar homeCalendar = new HomeCalendar();
        /// ]]>
        /// </code>
        /// </example>


        public HomeCalendar(String databaseFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }

            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }
            // Connecting home calendar
            Connection = Database.dbConnection;

            // create the categories object
            _categories = new Categories(Connection, newDB);
            // create the events object
            _events = new Events(Connection);
        }

        #region GetList

        // ============================================================================
        // Get all events list
        // ============================================================================
        /// <summary>
        /// Finds all items with a category and event. Ordering by start date and time, and optionally filtering by Category.
        /// </summary>
        /// <param name="Start">The start date and time. Inclusive</param>
        /// <param name="End">The end date and time. Inclusive</param>
        /// <param name="FilterFlag">If true, calendar items returned will need to be in the specified Category Id. If false, includes all Categories.</param>
        /// <param name="CategoryID">Specifies a Category Id to filter the Calendar Items. Will be ignored if FilterFlag is false.</param>
        /// <returns>A list of the Calendar Items as specified by the parameters.</returns>
        /// <remarks>
        /// The date range of returned events is inclusive and the returned list will be sorted by start date and time (ascending).
        /// The category ID is used to retrieve the associated Category in the <see cref="Categories"/> class.
        /// Likewise with the event ID and the <see cref="Events"/> class.
        /// </remarks>
        /// <example>
        /// For all examples below, assume the calendar file contains the
        /// following elements:
        /// 
        /// <code>
        ///|Details              |Start Time                |Duration  |Category           |Event ID  |
        ///|App Dev Homework     |2018-01-10 10:00:00 AM    |40        |Fun                |1         |
        ///|Honolulu             |2020-01-09 12:00:00 AM    |1440      |Vacation           |2         |
        ///|Honolulu             |2020-01-10 12:00:00 AM    |1440      |Vacation           |3         |
        ///|On call security     |2020-01-20 11:00:00 AM    |180       |On call            |4         |
        ///|staff meeting        |2018-01-11 7:30:00 PM     |15        |Work               |5         |
        ///|New Year's           |2020-01-01 12:00:00 AM    |1440      |Canadian Holidays  |6         |
        ///|Wendy's birthday     |2020-01-12 12:00:00 AM    |1440      |Birthdays          |7         |
        ///|Sprint retrospective |2018-01-11 10:15:00 AM    |60        |Work               |8         |
        /// </code>
        /// <b>Example 1: Getting a list of ALL calendar items.</b>
        /// If the filter flag is false, the method returns all events. Busy Time is a variable that represents the total duration of all calendar events in minutes.
        /// <code>
        /// <![CDATA[
        ///  HomeCalendar calendar = new HomeCalendar();
        ///  calendar.ReadFromFile(filename);
        /// 
        ///  List <CalendarItem> items = calendar.GetCalendarItems(null, null, false, 0);
        ///             
        ///  // print important information
        ///  string bar = ("-----------------------------------------------------------------------------------------------------------------------");
        ///  string s = string.Format("|{0,-20} |{1,-25} |{2,-16} |{3,-20} |{4,-10} |{5,-15} |", "Details", "Start Time", "Duration (mins)", "Category", "Event ID", "BusyTime");
        ///  Console.WriteLine(s);
        ///  Console.WriteLine(bar);
        ///  for (int i = 0; i<items.Count; i++)
        ///  {
        ///     string itemString = string.Format("|\x1b[94m{0,-20}\x1b[0m |\x1b[94m{1,-25}\x1b[0m |\u001b[94m{2,-16}\u001b[0m |\u001b[94m{3,-20}\u001b[0m |\u001b[94m{4,-10}\u001b[0m |\u001b[94m{5,-15}\u001b[0m |", items[i].ShortDescription, items[i].StartDateTime, items[i].DurationInMinutes, items[i].Category, items[i].EventID, items[i].BusyTime);
        ///     Console.WriteLine(itemString);
        ///  }
        ///
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output data:
        /// <code>
        ///|Details              |Start Time              |Duration |Category          |Event ID | Busy Time |
        ///|App Dev Homework     |2018-01-10 10:00:00 AM  |40       |Fun               |1        |40         |
        ///|Sprint retrospective |2018-01-11 10:15:00 AM  |60       |Work              |8        |100        |
        ///|staff meeting        |2018-01-11 7:30:00 PM   |15       |Work              |5        |115        |
        ///|New Year's           |2020-01-01 12:00:00 AM  |1440     |Canadian Holidays |6        |1555       |
        ///|Honolulu             |2020-01-09 12:00:00 AM  |1440     |Vacation          |2        |2995       |
        ///|Honolulu             |2020-01-10 12:00:00 AM  |1440     |Vacation          |3        |4435       |
        ///|Wendy's birthday     |2020-01-12 12:00:00 AM  |1440     |Birthdays         |7        |5875       |
        ///|On call security     |2020-01-20 11:00:00 AM  |180      |On call           |4        |6055       |
        /// </code>
        /// 
        /// <b>Example 2: FILTERED list of all calendar items</b>
        /// 
        /// If we set the <paramref name="FilterFlag"/> to true, the method will filter the Events and return only the Events with the same category Id.
        /// In this example, it will return all events of category id '9'.
        /// <code>
        /// <![CDATA[
        /// HomeCalendar calendar = new HomeCalendar();
        ///  calendar.ReadFromFile(filename);
        ///  List <CalendarItem> calendarItems = calendar.GetCalendarItems(null, null, true, 9);
        ///             
        ///  // print important information, see sample code from Example 1
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// |Details      |Start Time                |Duration  |Category           |Event ID   |Busy Time  |
        /// |Honolulu     |2020-01-09 12:00:00 AM    |1440      |Vacation           |2          |1440       |
        /// |Honolulu     |2020-01-10 12:00:00 AM    |1440      |Vacation           |3          |2880       |
        /// </code>
        /// </example>
        public List<CalendarItem> GetCalendarItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            DateTime notNullStart = Start ?? new DateTime(1900, 1, 1);
            DateTime notNullEnd = End ?? new DateTime(2500, 1, 1);

            bool isStartNull = Start is null;
            bool isEndNull = End is null;

            using var cmd = new SQLiteCommand(_Connection);
            
            // I am so sorry for writing this - Eric
            cmd.CommandText = $"SELECT c.Id, c.Description, e.Id, e.StartDateTime, e.Details, e.DurationInMinutes, e.CategoryId\n" +
                               "FROM events e\n" +
                               "LEFT JOIN categories c\n" +
                               "ON e.CategoryId = c.Id\n" +
                               $"{(!isStartNull || !isEndNull ? "WHERE " : "")}" + 
                               $"{(!isStartNull ? $"e.StartDateTime >= @start {(!isEndNull ? "AND " : "")}" : "")}" + 
                               $"{(!isEndNull ? "e.StartDateTime <= @end" : "")}\n" +
                               "ORDER BY e.StartDateTime";
            if (Start is not null)
                cmd.Parameters.AddWithValue("@start", notNullStart.ToString(@"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            if (End is not null)
                cmd.Parameters.AddWithValue("@end", notNullEnd.ToString(@"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));



            //from c in _categories.List()
            //join e in _events.List() on c.Id equals e.Category
            //where e.StartDateTime >= Start && e.StartDateTime <= End
            //select new { CatId = c.Id, EventId = e.Id, e.StartDateTime, Category = c.Description, e.Details, e.DurationInMinutes };

            // ------------------------------------------------------------------------
            // create a CalendarItem list with totals,
            // ------------------------------------------------------------------------
            using SQLiteDataReader reader = cmd.ExecuteReader();

            List<CalendarItem> items = new List<CalendarItem>();
            Double totalBusyTime = 0;
            while (reader.Read())
            {

                int categoryId = reader.GetInt32(0);
                if (FilterFlag && CategoryID != categoryId)
                {
                    continue;
                }
                string categoryDescription = reader.GetString(1);
                int eventId = reader.GetInt32(2);
                DateTime eventsStartDateTime = DateTime.Parse(reader.GetString(3));
                string eventsDetails = reader.GetString(4);
                double eventsDurationTime = reader.GetDouble(5);
                int eventsCategoryId = reader.GetInt32(6);
                totalBusyTime += eventsDurationTime;

                items.Add(new CalendarItem
                {
                    CategoryID = categoryId,
                    EventID = eventId,
                    ShortDescription = eventsDetails,
                    StartDateTime = eventsStartDateTime,
                    DurationInMinutes = eventsDurationTime,
                    Category = categoryDescription,
                    BusyTime = totalBusyTime
                });
            }

            return items;
        }

        // ============================================================================
        // Group all events month by month (sorted by year/month)
        // returns a list of CalendarItemsByMonth which is 
        // "year/month", list of calendar items, and totalBusyTime for that month
        // ============================================================================
        /// <summary>
        /// Groups calendar events by year and month (sorted by year and by month).
        /// </summary>
        /// <param name="Start">The start date and time of the range of events. Inclusive.</param>
        /// <param name="End">The end date and time of the range of events. Inclusive.</param>
        /// <param name="FilterFlag">If true, will only return events of a specified category.</param>
        /// <param name="CategoryID">Specifies a Category Id to filter the Calendar Items. Will be ignored if FilterFlag is false.</param>
        /// <exception cref="ArgumentNullException">Thrown if the items could not be grouped correctly.</exception>
        /// <returns>A list of items containing a month, a list of the calendar items occuring in the month and a total busy time for the month.</returns>
        /// <remarks>
        /// This method groups calendar events by year and month, sorted by year and month.
        /// It returns a list of CalendarItemsByMonth objects, each representing a month with a list of calendar items occurring in that month and the total busy time for the month.
        /// </remarks>
        /// <example>
        /// 
        /// For all examples below, assume the calendar file contains the following elements:
        /// <code>
        /// |Details              |Start Time              |Duration |Category           |Event ID  |
        /// |App Dev Homework     |2018-01-10 10:00:00 AM  |40       |Fun                |1         |
        /// |Honolulu             |2020-01-09 12:00:00 AM  |1440     |Vacation           |2         |
        /// |Honolulu             |2020-01-10 12:00:00 AM  |1440     |Vacation           |3         |
        /// |On call security     |2020-01-20 11:00:00 AM  |180      |On call            |4         |
        /// |staff meeting        |2018-01-11 7:30:00 PM   |15       |Work               |5         |
        /// |New Year's           |2020-01-01 12:00:00 AM  |1440     |Canadian Holidays  |6         |
        /// |Wendy's birthday     |2020-01-12 12:00:00 AM  |1440     |Birthdays          |7         |
        /// |Sprint retrospective |2018-01-11 10:15:00 AM  |60       |Work               |8         |
        /// </code>
        /// The date range of returned events is inclusive, and the returned list will be sorted by start date and time (ascending).
        /// The category ID is used to retrieve the associated Category in the <see cref="Categories"/> class.
        /// Likewise with the event ID and the <see cref="Events"/> class.
        /// Busy Time is a variable that represents the total duration of all calendar events in minutes.
        /// 
        /// <b>Example 1: Getting a list of calendar items grouped by month:</b>
        /// <code>
        /// <![CDATA[
        /// HomeCalendar calendar = new HomeCalendar();
        /// calendar.ReadFromFile(filename);
        /// 
        /// List<CalendarItemsByMonth> items = calendar.GetCalendarItemsByMonth(null, null, false, 0);
        /// 
        /// //print the output via a loop
        /// for (int j = 0; j < items.Count; j++) //looping all months
        ///      {
        ///         string bar = ("-----------------------------------------------------------------------------------------------------------------");
        ///         string s = string.Format("\n|{0,-10} |{1,-25} |{2,-20} |{3,-20} |{4,-10} |{5,-15} |", "Month", "Details", "Duration (mins)", "Category", "Event ID", "Busy Time");
        ///         Console.WriteLine(s);
        ///         Console.WriteLine(bar);
        ///             for (int i = 0; i<items[j].Items.Count; i++) //looping all items in month
        ///             {
        ///                 string itemString = string.Format("|\x1b[94m{0,-10}\x1b[0m |\x1b[94m{1,-25}\x1b[0m |\u001b[94m{2,-20}\u001b[0m |\u001b[94m{3,-20}\u001b[0m |\u001b[94m{4,-10}\u001b[0m |\u001b[94m{5,-15}\u001b[0m |", items[j].Month, items[j].Items[i].ShortDescription, items[j].Items[i].DurationInMinutes, items[j].Items[i].Category, items[j].Items[i].EventID, items[j].Items[i].BusyTime);
        ///                 Console.WriteLine(itemString);
        ///             }
        ///     }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// |Month      |Details                 |Duration  |Category           |Event ID   |Busy Time  |
        /// ---------------------------------------------------------------------------------------------
        /// |2018/01    |App Dev Homework        |40        |Fun                |1          |40         |
        /// |2018/01    |Sprint retrospective    |60        |Work               |8          |100        |
        /// |2018/01    |staff meeting           |15        |Work               |5          |115        |
        ///
        /// |Month      |Details                 |Duration  |Category           |Event ID   |Busy Time  |
        /// ---------------------------------------------------------------------------------------------
        /// |2020/01    |New Year's              |1440      |Canadian Holidays  |6          |1555       |
        /// |2020/01    |Honolulu                |1440      |Vacation           |2          |2995       |
        /// |2020/01    |Honolulu                |1440      |Vacation           |3          |4435       |
        /// |2020/01    |Wendy's birthday        |1440      |Birthdays          |7          |5875       |
        /// |2020/01    |On call security        |180       |On call            |4          |6055       |
        /// </code>
        /// 
        /// <b>Example 2: Filter flag is true</b>
        /// 
        /// If we set the <paramref name="FilterFlag"/> to true, the method will filter the Events and return only the Events with the same category Id.
        /// In this example, it will return all events of category id '2'.
        /// <code>
        /// <![CDATA[
        /// HomeCalendar calendar = new HomeCalendar();
        /// calendar.ReadFromFile(filename);
        /// List<CalendarItemsByMonth> calendarItemsByMonth = calendar.GetCalendarItemsByMonth(null, null, true, 2);
        /// 
        /// //print information retrieved, see above example for code.
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// |Month      |Details     |Duration (mins)      |Category    |Event ID   |Busy Time       |
        /// ------------------------------------------------------------------------------------------
        /// |2020/01    |Honolulu    |1440                 |Vacation    |2          |1440            |
        /// |2020/01    |Honolulu    |1440                 |Vacation    |3          |2880            |
        /// </code>
        /// </example>
        public List<CalendarItemsByMonth> GetCalendarItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            DateTime realStart = Start ?? new DateTime(1900, 1, 1);
            DateTime realEnd = End ?? new DateTime(2500, 1, 1);

            bool isStartNull = Start is null;
            bool isEndNull = End is null;

            var startDate = new DateTime(realStart.Year, realStart.Month, 1);
            var lastDay = DateTime.DaysInMonth(realStart.Year, realStart.Month);
            var endDate = new DateTime(realEnd.Year, realEnd.Month, lastDay);
            using var cmd = new SQLiteCommand(_Connection);
            cmd.CommandText = $"SELECT e.CategoryId, substr(StartDateTime, 1, 7) as Month, e.StartDateTime\n" +
                               "FROM events e\n" +
                               $"{(!isStartNull || !isEndNull ? "WHERE " : "")}" +
                               $"{(!isStartNull ? $"e.StartDateTime >= @start {(!isEndNull ? "AND " : "")}" : "")}" +
                               $"{(!isEndNull ? "e.StartDateTime <= @end" : "")}\n" +
                               "GROUP BY Month";
            if (Start is not null)
                cmd.Parameters.AddWithValue("@start", startDate.ToString(@"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            if (End is not null)
                cmd.Parameters.AddWithValue("@end", endDate.ToString(@"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            if (FilterFlag)
            {
                cmd.CommandText = $"SELECT e.CategoryId, substr(StartDateTime, 1, 7) as Month, e.StartDateTime\n" +
                              "FROM events e WHERE e.StartDateTime >= @start AND e.StartDateTime <= @end AND e.CategoryId = @catId\n" +
                              "GROUP BY Month";
                cmd.Parameters.AddWithValue("@start", startDate.ToString(@"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@end", endDate.ToString(@"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@catId", CategoryID);
            }
            
           
            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            using SQLiteDataReader reader = cmd.ExecuteReader();

            List<CalendarItemsByMonth> itemsByMonth = new List<CalendarItemsByMonth>();

            while (reader.Read())
            {
                // Creating necessary variables for a new CalendarItemByMonth object
                int categoryId = reader.GetInt32(0);

                //if (FilterFlag && CategoryID != categoryId)
                //    continue;
                string month = reader.GetString(1);
                string stringMonth = month.ToString();
                DateTime startDateMonth = reader.GetDateTime(2);

                // Getting the start and end date time for the month
                var startDateItems = new DateTime(startDateMonth.Year, startDateMonth.Month, 1);
                var lastDayItems = DateTime.DaysInMonth(startDateMonth.Year, startDateMonth.Month);
                var endDateItems = startDateItems.AddDays(lastDayItems);

                // Getting all items for that month
                List<CalendarItem> items = GetCalendarItems(startDateItems, endDateItems, FilterFlag, CategoryID);
                // Adding up the busytime NOTE: change this eventually to not loop, just for now this works
                double totalItemBusyTime = 0;
                foreach ( CalendarItem item in items )
                {
                    totalItemBusyTime += item.DurationInMinutes;
                }
                // Adding the items to the List
                itemsByMonth.Add(new CalendarItemsByMonth
                {
                    Month = stringMonth,
                    Items = items,
                    TotalBusyTime = totalItemBusyTime,
                });
           }

            return itemsByMonth;

        }


        // ============================================================================
        // Group all events by category (ordered by category name)
        // ============================================================================
        /// <summary>
        /// Group all events by category (ordered by category name).
        /// </summary>
        /// <param name="Start">The start date and time of the range of events. Inclusive.</param>
        /// <param name="End">The end date and time of the range of events. Inclusive.</param>
        /// <param name="FilterFlag">If true, will only return events of a specified category.</param>
        /// <param name="CategoryID">Specifies a Category Id to filter the Calendar Items. Will be ignored if FilterFlag is false.</param>
        /// <returns>A list of items containing a category, a list of calendar items of the category and a total busy time for the category.</returns>
        /// <example>
        /// For all examples below, assume the calendar file contains the following elements:
        /// <code>
        /// |Details              |Start Time              |Duration |Category           |Event ID  |
        /// |App Dev Homework     |2018-01-10 10:00:00 AM  |40       |Fun                |1         |
        /// |Honolulu             |2020-01-09 12:00:00 AM  |1440     |Vacation           |2         |
        /// |Honolulu             |2020-01-10 12:00:00 AM  |1440     |Vacation           |3         |
        /// |On call security     |2020-01-20 11:00:00 AM  |180      |On call            |4         |
        /// |staff meeting        |2018-01-11 7:30:00 PM   |15       |Work               |5         |
        /// |New Year's           |2020-01-01 12:00:00 AM  |1440     |Canadian Holidays  |6         |
        /// |Wendy's birthday     |2020-01-12 12:00:00 AM  |1440     |Birthdays          |7         |
        /// |Sprint retrospective |2018-01-11 10:15:00 AM  |60       |Work               |8         |
        /// </code>
        /// The date range of returned events is inclusive, and the returned list will be sorted by start date and time (ascending).
        /// The category ID is used to retrieve the associated Category in the <see cref="Categories"/> class.
        /// Likewise with the event ID and the <see cref="Events"/> class.
        /// Busy Time is a variable that represents the total duration of all calendar events in minutes.
        /// <b>Example 1: Getting a list of calendar items grouped by category:</b>
        /// <code>
        /// <![CDATA[
        /// HomeCalendar calendar = new HomeCalendar();
        /// calendar.ReadFromFile(filename);
        /// 
        /// List<CalendarItemsByCategory> items = calendar.GetCalendarItemsByCategory(null, null, false, 0);
        /// 
        /// //print the output via a loop
        /// for (int j = 0; j < items.Count; j++) //looping all categories
        ///    {
        ///        string bar = ("---------------------------------------------------------------------------------------------------------------");
        ///string s = string.Format("\n|{0,-20} |{1,-25} |{2,-20} |{3,-20} |{4,-15} |", "Category", "Details", "Duration (mins)", "Event ID", "Busy Time");
        ///Console.WriteLine(s);
        ///        Console.WriteLine(bar);
        ///        for (int i = 0; i<items[j].Items.Count; i++) //looping all items in category
        ///        {
        ///            string itemString = string.Format("|\x1b[94m{0,-20}\x1b[0m |\x1b[94m{1,-25}\x1b[0m |\u001b[94m{2,-20}\u001b[0m |\u001b[94m{3,-20}\u001b[0m |\u001b[94m{4,-15}\u001b[0m |", items[j].Category, items[j].Items[i].ShortDescription, items[j].Items[i].DurationInMinutes, items[j].Items[i].EventID, items[j].Items[i].BusyTime);
        ///Console.WriteLine(itemString);
        ///        }
        ///}
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// 
        ///|Category             |Details            |Duration(mins) |Event ID   |Busy Time    |
        ///-------------------------------------------------------------------------------------
        ///|Birthdays            |Wendy's birthday   |1440           |7          |5875         |
        ///
        ///|Category             |Details            |Duration(mins) |Event ID   |Busy Time    |
        ///-------------------------------------------------------------------------------------
        ///|Canadian Holidays    |New Year's         |1440           |6          |1555         |
        ///
        ///|Category             |Details            |Duration(mins) |Event ID   |Busy Time    |
        ///-------------------------------------------------------------------------------------
        ///|Fun                  |App Dev Homework   |40             |1          |40           |
        ///
        ///|Category             |Details            |Duration(mins) |Event ID   |Busy Time    |
        ///-------------------------------------------------------------------------------------
        ///|On call              |On call security   |180            |4          |6055         |
        ///
        ///|Category             |Details            |Duration(mins) |Event ID   |Busy Time    |
        ///-------------------------------------------------------------------------------------
        ///|Vacation             |Honolulu           |1440           |2          |2995         |
        ///|Vacation             |Honolulu           |1440           |3          |4435         |
        ///
        ///|Category             |Details            |Duration(mins) |Event ID   |Busy Time    |
        ///-------------------------------------------------------------------------------------
        ///|Work                 |Sprint retrospective      |60      |8          |100          |
        ///|Work                 |staff meeting             |15      |5          |115          |
        /// </code>
        /// <b>Example 2: Filter flag is true</b>
        /// If we set the <paramref name="FilterFlag"/> to true, the method will filter the Events and return only the Events with the same category Id.
        /// In this example, it will return all events of category id '9'.
        /// <code>
        /// <![CDATA[
        /// HomeCalendar calendar = new HomeCalendar();
        /// calendar.ReadFromFile(filename);
        /// List<CalendarItemsByCategory> calendarItemsByCategory = calendar.GetCalendarItemsByMonth(null, null, true, 9);
        /// 
        /// //print information retrieved, see above example for code.
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// |Category    |Details          |Duration (mins)      |Event ID   |Busy Time       |
        /// -----------------------------------------------------------------------------------
        /// |Vacation    |Honolulu         |1440                 |2          |1440            |
        /// |Vacation    |Honolulu         |1440                 |3          |2880            |
        /// </code>
        /// </example>
        public List<CalendarItemsByCategory> GetCalendarItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // Get all unique categories used in Categories table
            DateTime notNullStart = Start ?? new DateTime(1900, 1, 1);
            DateTime notNullEnd = End ?? new DateTime(2500, 1, 1);

            //! Might need to change to *all unique categoriesId in categories table*, because some Categories in CategoryTypes table might not appear in Categories table (IF USING DEFAULT)
            using var cmd = new SQLiteCommand(_Connection);
            cmd.CommandText = "SELECT e.Id, e.StartDateTime, e.Details, e.DurationInMinutes, e.CategoryId, c.Description\n" +
                            "FROM events e LEFT JOIN categories c\n" +
                            "WHERE e.CategoryId = c.Id AND e.StartDateTime > @start AND e.StartDateTime < @end" +
                            $"{(FilterFlag ? " AND e.CategoryId = @categoryId" : "")}\n" +
                            /* -------------------------------------------------------------------------------------------------------------------
                             *  IMPORTANT: IDK if it's supposed to be ordered by e.Details, or e.DurationInMinutes DESC, because both work - Eric
                             * -------------------------------------------------------------------------------------------------------------------
                             */
                            "ORDER BY c.Description, e.Details";

            cmd.Parameters.AddWithValue("@start", notNullStart.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@end", notNullEnd.ToString("yyyy-MM-dd HH:mm:ss"));
            if (FilterFlag)
                cmd.Parameters.AddWithValue("@categoryId", CategoryID);

            // Create a list with all unique CategoriesId
            using SQLiteDataReader reader = cmd.ExecuteReader();
            string previousCategory = "";
            int index = -1;

            List<CalendarItemsByCategory> items = new List<CalendarItemsByCategory>();
            while (reader.Read())
            {
                int eventId = reader.GetInt32(0);
                DateTime eventStartDateTime = DateTime.ParseExact(reader.GetString(1), @"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                string eventDetails = reader.GetString(2);
                double eventDurationInMinutes = reader.GetDouble(3);
                int eventCategoryID = reader.GetInt32(4);
                string categoryDescription = reader.GetString(5);

                if (previousCategory != categoryDescription)
                {
                    CalendarItem calendarItem = new CalendarItem { EventID = eventId, 
                        StartDateTime = eventStartDateTime, 
                        ShortDescription = eventDetails, 
                        DurationInMinutes = eventDurationInMinutes,
                        CategoryID = eventCategoryID,
                        Category = categoryDescription,
                        BusyTime = eventDurationInMinutes
                    };

                    items.Add(new CalendarItemsByCategory
                    {
                        Category = calendarItem.Category,
                        Items = new List<CalendarItem>() { calendarItem },
                        TotalBusyTime = calendarItem.BusyTime,
                    });

                    index++;
                }
                else
                {
                    CalendarItem calendarItem = new CalendarItem
                    {
                        EventID = eventId,
                        StartDateTime = eventStartDateTime,
                        ShortDescription = eventDetails,
                        DurationInMinutes = eventDurationInMinutes,
                        CategoryID = eventCategoryID,
                        Category = categoryDescription,
                        BusyTime = eventDurationInMinutes
                    };

                    items[index].Items!.Add(calendarItem);
                    items[index].TotalBusyTime += calendarItem.BusyTime;
                }

                previousCategory = categoryDescription;
            }
            return items;
        }

        // ============================================================================
        // Group all events by category and Month
        // creates a list of Dictionary objects with:
        //          one dictionary object per month,
        //          and one dictionary object for the category total busy times
        // 
        // Each per month dictionary object has the following key value pairs:
        //           "Month", <name of month>
        //           "TotalBusyTime", <the total durations for the month>
        //             for each category for which there is an event in the month:
        //             "items:category", a List<CalendarItem>
        //             "category", the total busy time for that category for this month
        // The one dictionary for the category total busy times has the following key value pairs:
        //             for each category for which there is an event in ANY month:
        //             "category", the total busy time for that category for all the months
        // <param name="Start">Start time of the calendar item. Can be null.</param>
        // <param name="End">End time of the calendar item. Can be null.</param>
        // <param name="FilterFlag">A filter flag for sorting calendar items by category. If true, will only display items of the category type.</param>
        // <param name="CategoryID">A category ID that will be filtered for if the flag is true.</param>
        // <remarks>The Month dictionary object has the following key value pairs: "Month", name of the month</remarks>
        // <returns>A List of Dictionaries containing the category and object.</returns>
        // ============================================================================
        /// <summary>
        /// Groups all Events by Category and Month using a dictionary for each month and another for the category total busy times.
        /// </summary>
        /// <param name="Start">Start time of the calendar item. Can be null. Inclusive. </param>
        /// <param name="End">End time of the calendar item. Can be null. Inclusive.</param>
        /// <param name="FilterFlag">A filter flag for sorting calendar items by category. If true, will only display items of the category type.</param>
        /// <param name="CategoryID">A category ID that will be filtered for if the flag is true.</param>
        /// <returns>A List of Dictionaries containing the month, total busy time, and events grouped by category and month.</returns>
        /// <example>
        /// For all examples below, assume the calendar file contains the following elements:
        /// <code>
        /// |Details              |Start Time              |Duration |Category           |Event ID  |
        /// |App Dev Homework     |2018-01-10 10:00:00 AM  |40       |Fun                |1         |
        /// |Honolulu             |2020-01-09 12:00:00 AM  |1440     |Vacation           |2         |
        /// |Honolulu             |2020-01-10 12:00:00 AM  |1440     |Vacation           |3         |
        /// |On call security     |2020-01-20 11:00:00 AM  |180      |On call            |4         |
        /// |staff meeting        |2018-01-11 7:30:00 PM   |15       |Work               |5         |
        /// |New Year's           |2020-01-01 12:00:00 AM  |1440     |Canadian Holidays  |6         |
        /// |Wendy's birthday     |2020-01-12 12:00:00 AM  |1440     |Birthdays          |7         |
        /// |Sprint retrospective |2018-01-11 10:15:00 AM  |60       |Work               |8         |
        /// </code>
        /// 
        /// <b>Example 1: Getting a list of calendar items grouped by month:</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeCalendar homeCalendar = new HomeCalendar();
        /// List<Dictionary<string, object>> calendarDictionary = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(startDateTime, endDateTime, false, 9);
        /// 
        /// foreach (var monthRecord in calendarDictionary)
        ///    {
        ///       Console.WriteLine($"\nMonth: {monthRecord["Month"]}");
        ///       //checking if the record contains a total busy time
        ///       if (monthRecord.ContainsKey("TotalBusyTime"))
        ///         Console.WriteLine($"Total Busy Time: {monthRecord["TotalBusyTime"]} minutes");
        ///
        ///       //looping over each category and their events and printing their details
        ///       foreach (var categoryRecord in monthRecord.Keys)
        ///       {
        ///           //check if the key starts with "items:" since this means it corresponds to a category
        ///           if (categoryRecord.StartsWith("items:"))
        ///           {
        ///               string categoryName = categoryRecord.Substring(6); //taking the category name for printing
        ///               List<CalendarItem> eventRecords = (List<CalendarItem>)monthRecord[categoryRecord];
        ///               Console.WriteLine($"Category: {categoryName}, Total Busy Time: {monthRecord[categoryName]} minutes");
        ///
        ///               //looping over each calendar item and printing its details
        ///               foreach (var eventItem in eventRecords)
        ///                   Console.WriteLine($"Event: {eventItem.ShortDescription}, Start Time: {eventItem.StartDateTime}, Duration: {eventItem.DurationInMinutes} minutes");
        ///           }
        ///       }
        ///   }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month: 2018/01
        /// Total Busy Time: 115 minutes
        ///
        /// Category: Fun , Total Busy Time: 40 minutes
        /// Event: App Dev Homework, Start Time: 2018-01-10 10:00:00 AM, Duration: 40 minutes
        /// Category: Work , Total Busy Time: 75 minutes
        /// Event: Sprint retrospective, Start Time: 2018-01-11 10:15:00 AM, Duration: 60 minutes
        /// Event: staff meeting, Start Time: 2018-01-11 7:30:00 PM, Duration: 15 minutes
        ///
        ///  Month: 2020/01
        /// Total Busy Time: 5940 minutes
        ///
        /// Category: Birthdays , Total Busy Time: 1440 minutes
        /// Event: Wendy's birthday, Start Time: 2020-01-12 12:00:00 AM, Duration: 1440 minutes
        /// Category: Canadian Holidays, Total Busy Time: 1440 minutes
        /// Event: New Year's, Start Time: 2020-01-01 12:00:00 AM, Duration: 1440 minutes
        /// Category: On call, Total Busy Time: 180 minutes
        /// Event: On call security, Start Time: 2020-01-20 11:00:00 AM, Duration: 180 minutes
        /// Category: Vacation , Total Busy Time: 2880 minutes
        /// Event: Honolulu, Start Time: 2020-01-09 12:00:00 AM, Duration: 1440 minutes
        /// Event: Honolulu, Start Time: 2020-01-10 12:00:00 AM, Duration: 1440 minutes
        /// </code>
        /// 
        /// <b>Example 2: Filter Flag is true</b>
        /// 
        /// If we set the <paramref name="FilterFlag"/> to true, the method will filter the Events and return only the Events with the same category Id.
        /// In this example, it will return all events of category id '9'.
        /// <code>
        /// <![CDATA[
        /// HomeCalendar homeCalendar = new HomeCalendar();
        /// List<Dictionary<string, object>> calendarDictionary = homeCalendar.GetCalendarDictionaryByCategoryAndMonth(startDateTime, endDateTime, true, 9);
        /// 
        /// // print information about the dictionary (see the nested foreach from example 1)
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// Month: 2020/01
        /// Total Busy Time: 2880 minutes
        ///
        /// Category: Vacation , Total Busy Time: 2880 minutes
        /// Event: Honolulu, Start Time: 2020-01-09 12:00:00 AM, Duration: 1440 minutes
        /// Event: Honolulu, Start Time: 2020-01-10 12:00:00 AM, Duration: 1440 minutes
        /// </code>
        /// </example>
        public List<Dictionary<string, object>> GetCalendarDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<CalendarItemsByMonth> GroupedByMonth = GetCalendarItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalBusyTimePerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["TotalBusyTime"] = MonthGroup.TotalBusyTime;

                // break up the month items into categories
                var GroupedByCategory = MonthGroup.Items.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of items
                    double totalCategoryBusyTimeForThisMonth = 0;
                    var details = new List<CalendarItem>();

                    foreach (var item in CategoryGroup)
                    {
                        totalCategoryBusyTimeForThisMonth = totalCategoryBusyTimeForThisMonth + item.DurationInMinutes;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["items:" + CategoryGroup.Key] = details;
                    record[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;

                    // keep track of totals for each category
                    if (totalBusyTimePerCategory.TryGetValue(CategoryGroup.Key, out Double currentTotalBusyTimeForCategory))
                    {
                        totalBusyTimePerCategory[CategoryGroup.Key] = currentTotalBusyTimeForCategory + totalCategoryBusyTimeForThisMonth;
                    }
                    else
                    {
                        totalBusyTimePerCategory[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalBusyTimePerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }
        #endregion GetList
    }
}
