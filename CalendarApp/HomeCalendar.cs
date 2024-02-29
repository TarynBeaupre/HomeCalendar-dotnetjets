// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================
using System.Data.SQLite;

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
        private string? _FileName;
        private string? _DirName;
        private Categories _categories;
        private Events _events;

        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (location of files etc)
        /// <summary>
        /// Gets the file name of the file containing the location of Category and Event files. 
        /// </summary>
        /// <value>The filename of the file containing the location of Category and Event files. If null, uses the default file name.</value>
        /// <remarks>Returns the backing field containing the file name.</remarks>
        public String? FileName { get { return _FileName; } }
        /// <summary>
        /// Gets the directory name of the directory containing the file with Category and Event file locations. 
        /// </summary>
        /// <value>The directory name of the file containing the location of Category and Event files. If null, uses the default directory name.</value>
        /// <remarks>Returns the backing field containing the file name.</remarks>
        public String? DirName { get { return _DirName; } }
        /// <summary>
        /// Gets the path name of the directory containing the file with Category and Event file locations.
        /// </summary>
        /// <value>The full path name of the file containing the location of Category and Event files. Can be null if either FileName or DirName is null.</value>
        /// <remarks>Returns the result of the <see cref="Path.GetFullPath(string)"/> function, it is a calculated property.</remarks>
        public String? PathName
        {
            get
            {
                if (_FileName != null && _DirName != null)
                {
                    return Path.GetFullPath(_DirName + "\\" + _FileName);
                }
                else
                {
                    return null;
                }
            }
        }

        // Properties (categories and events object)
        /// <summary>
        /// Gets the possible categories for Calendar Items.
        /// </summary>
        /// <value>The categories coming from the Categories class.</value>
        /// <returns>Returns a categories backing field.</returns>
        public Categories categories { get { return _categories; } }

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
        public HomeCalendar()
        {
            _categories = new Categories();
            _events = new Events();
        }

        public HomeCalendar(String databaseFile, String eventsXMLFile, bool newDB = false)
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

            // create the category object
            _categories = new Categories(Database.dbConnection, newDB);

            // create the _events course
            _events = new Events();
            _events.ReadFromFile(eventsXMLFile);
        }

        // -------------------------------------------------------------------
        // Constructor (existing calendar ... must specify file)
        // -------------------------------------------------------------------
        /// <summary>
        /// Creates a Home Calendar object containing a list of categories and events read from a file.
        /// </summary>
        /// <param name="calendarFileName">The file containing the paths of the files containing the event and category data.</param>
        /// <exception cref="Exception">Thrown if unable to read from the file.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     HomeCalendar homeCalendar = new HomeCalendar("../../../newcalendar.calendar");
        /// }
        /// catch (Exception ex){
        ///     Console.WriteLine(ex.Message);
        /// }
        /// ]]>
        /// </code></example>
        public HomeCalendar(String calendarFileName)
        {
            _categories = new Categories();
            _events = new Events();
            ReadFromFile(calendarFileName);
        }

        #region OpenNewAndSave
        // ---------------------------------------------------------------
        // Read
        // Throws Exception if any problem reading this file
        // ---------------------------------------------------------------
        /// <summary>
        /// Reads calendar data from the specified file name and populates the event and category properties with the file data.
        /// </summary>
        /// <param name="calendarFileName">The file name containing calendar data. If null, it will use the default file name.</param>
        /// <exception cref="Exception">Throws an Exception if the calendar data could not be read from the file name given.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// HomeCalendar homeCalendar = new HomeCalendar();
        /// homeCalendar.ReadFromFile("filename.txt");
        /// ]]>
        /// </code>
        /// </example>
        public void ReadFromFile(String? calendarFileName)
        {
            // ---------------------------------------------------------------
            // read the calendar file and process
            // ---------------------------------------------------------------
            try
            {
                // get filepath name (throws exception if it doesn't exist)
                calendarFileName = CalendarFiles.VerifyReadFromFileName(calendarFileName, "");

                // If file exists, read it
                string[] filenames = System.IO.File.ReadAllLines(calendarFileName);

                // ----------------------------------------------------------------
                // Save information about the calendar file
                // ----------------------------------------------------------------
                string? folder = Path.GetDirectoryName(calendarFileName);
                _FileName = Path.GetFileName(calendarFileName);

                // read the events and categories from their respective files
                _events.ReadFromFile(folder + "\\" + filenames[1]);

                // Save information about calendar file
                _DirName = Path.GetDirectoryName(calendarFileName);
                _FileName = Path.GetFileName(calendarFileName);

            }

            // ----------------------------------------------------------------
            // throw new exception if we cannot get the info that we need
            // ----------------------------------------------------------------
            catch (Exception e)
            {
                throw new Exception("Could not read calendar info: \n" + e.Message);
            }

        }

        // ====================================================================
        // save to a file
        // saves the following files:
        //  filepath_events.evts  # events file
        //  filepath_categories.cats # categories files
        //  filepath # a file containing the names of the events and categories files.
        //  Throws exception if we cannot write to that file (ex: invalid dir, wrong permissions)
        // ====================================================================
        /// <summary>
        /// Saves the events file and categories file at the given file path.
        /// </summary>
        /// <param name="filepath">The file path to write the data to.</param>
        /// <exception cref="Exception">Throws an exception if verification of the file path failed or could not write to path.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// HomeCalendar homeCalendar = new HomeCalendar();
        /// homeCalendar.SaveToFile("my_calendar.txt");
        /// ]]></code></example>
        public void SaveToFile(String filepath)
        {

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if we can't write to the file)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyWriteToFileName(filepath, "");

            String? path = Path.GetDirectoryName(Path.GetFullPath(filepath));
            String file = Path.GetFileNameWithoutExtension(filepath);
            String ext = Path.GetExtension(filepath);

            // ---------------------------------------------------------------
            // construct file names for events and categories
            // ---------------------------------------------------------------
            String eventpath = path + "\\" + file + "_events" + ".evts";
            String categorypath = path + "\\" + file + "_categories" + ".cats";

            // ---------------------------------------------------------------
            // save the events and categories into their own files
            // ---------------------------------------------------------------
            _events.SaveToFile(eventpath);

            // ---------------------------------------------------------------
            // save filenames of events and categories to calendar file
            // ---------------------------------------------------------------
            string[] files = { Path.GetFileName(categorypath), Path.GetFileName(eventpath) };
            System.IO.File.WriteAllLines(filepath, files);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = path;
            _FileName = Path.GetFileName(filepath);
        }
        #endregion OpenNewAndSave

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
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            var query = from c in _categories.List()
                        join e in _events.List() on c.Id equals e.Category
                        where e.StartDateTime >= Start && e.StartDateTime <= End
                        select new { CatId = c.Id, EventId = e.Id, e.StartDateTime, Category = c.Description, e.Details, e.DurationInMinutes };

            // ------------------------------------------------------------------------
            // create a CalendarItem list with totals,
            // ------------------------------------------------------------------------

            List<CalendarItem> items = new List<CalendarItem>();
            Double totalBusyTime = 0;

            foreach (var e in query.OrderBy(q => q.StartDateTime))
            {
                // filter out unwanted categories if filter flag is on
                if (FilterFlag && CategoryID != e.CatId)
                {
                    continue;
                }

                // keep track of running totals
                totalBusyTime = totalBusyTime + e.DurationInMinutes;
                items.Add(new CalendarItem
                {
                    CategoryID = e.CatId,
                    EventID = e.EventId,
                    ShortDescription = e.Details,
                    StartDateTime = e.StartDateTime,
                    DurationInMinutes = e.DurationInMinutes,
                    Category = e.Category,
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
            List<CalendarItem> items = GetCalendarItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by year/month
            // -----------------------------------------------------------------------
            var GroupedByMonth = items.GroupBy(c => c.StartDateTime.Year.ToString("D4") + "/" + c.StartDateTime.Month.ToString("D2"));

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<CalendarItemsByMonth>();
            foreach (var MonthGroup in GroupedByMonth)
            {
                // calculate totalBusyTime for this month, and create list of items
                double total = 0;
                var itemsList = new List<CalendarItem>();
                foreach (var item in MonthGroup)
                {
                    total = total + item.DurationInMinutes;
                    itemsList.Add(item);
                }

                // Add new CalendarItemsByMonth to our list
                summary.Add(new CalendarItemsByMonth
                {
                    Month = MonthGroup.Key,
                    Items = itemsList,
                    TotalBusyTime = total
                });
            }

            return summary;
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
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<CalendarItem> filteredItems = GetCalendarItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by Category
            // -----------------------------------------------------------------------
            var GroupedByCategory = filteredItems.GroupBy(c => c.Category);

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<CalendarItemsByCategory>();
            foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
            {
                // calculate totalBusyTime for this category, and create list of items
                double total = 0;
                var items = new List<CalendarItem>();
                foreach (var item in CategoryGroup)
                {
                    total = total + item.DurationInMinutes;
                    items.Add(item);
                }

                // Add new CalendarItemsByCategory to our list
                summary.Add(new CalendarItemsByCategory
                {
                    Category = CategoryGroup.Key,
                    Items = items,
                    TotalBusyTime = total
                });
            }

            return summary;
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
