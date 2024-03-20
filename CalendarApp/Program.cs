namespace Calendar
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to your home calendar!");
            HomeCalendar homeCalendar = new HomeCalendar("../../../test.calendar");
            DateTime startDateTime = new DateTime(2010, 01, 01);
            DateTime endDateTime = DateTime.Now;
            bool run = true;
            while (run)
            {
                Console.WriteLine("1: All calendar items");
                Console.WriteLine("2: All calendar items by month");
                Console.WriteLine("3: All calendar items by category");
                Console.WriteLine("4: All calendar items by category and month (Dictionary)");
                Console.WriteLine("5: Quit");
                Console.Write("Enter an option:");
                string optionInput = Console.ReadLine();
                int option;
                if (!int.TryParse(optionInput, out option) || option < 1 || option > 5)
                {
                    Console.WriteLine("Invalid option. Please enter a number between 1 and 5.");
                    continue; // restart the loop to prompt the user again
                }
                Console.Write("Apply category filter (category id of 9)? Enter 'y' to filter: ");
                string filter = Console.ReadLine();
                bool filterBool = filter.ToLower() == "y";
                Console.Clear();
                if (filterBool ) { Console.WriteLine("Showing the FILTERED results for category id 9"); }
                switch (Convert.ToInt32(option))
                {
                    case 1:
                        Console.WriteLine($"\nAll calendar items from {startDateTime} to {endDateTime}:\n");
                        FormatPrintCalendarItems(homeCalendar.GetCalendarItems(startDateTime, endDateTime, filterBool, 9));
                        break;
                    case 2:
                        Console.WriteLine($"\nAll calendar items for each month:");
                        FormatPrintCalendarItemsMonth(homeCalendar.GetCalendarItemsByMonth(startDateTime, endDateTime, filterBool, 9));
                        break;
                    case 3:
                        Console.WriteLine($"\nAll calendar items for each category.");
                        FormatPrintCalendarItemsCategory(homeCalendar.GetCalendarItemsByCategory(startDateTime, endDateTime, filterBool, 9));
                        break;
                    case 4:
                        Console.WriteLine($"\nAll calendar items for each category and month.");
                        FormatPrintCalendarItemsDict(homeCalendar.GetCalendarDictionaryByCategoryAndMonth(startDateTime, endDateTime, filterBool, 9));
                        break;
                    case 5:
                        run = false;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public static void FormatPrintCalendarItems(List<CalendarItem> items)
        {
            string bar = ("-----------------------------------------------------------------------------------------------------------------------");
            string s = string.Format("|{0,-20} |{1,-25} |{2,-16} |{3,-20} |{4,-10} |{5,-15} |", "Details", "Start Time", "Duration (mins)", "Category", "Event ID", "BusyTime");
            Console.WriteLine(s);
            Console.WriteLine(bar);
            for (int i = 0; i < items.Count; i++)
            {
                string itemString = string.Format("|\x1b[94m{0,-20}\x1b[0m |\x1b[94m{1,-25}\x1b[0m |\u001b[94m{2,-16}\u001b[0m |\u001b[94m{3,-20}\u001b[0m |\u001b[94m{4,-10}\u001b[0m |\u001b[94m{5,-15}\u001b[0m |", items[i].ShortDescription, items[i].StartDateTime, items[i].DurationInMinutes, items[i].Category, items[i].EventID, items[i].BusyTime);
                Console.WriteLine(itemString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public static void FormatPrintCalendarItemsMonth(List<CalendarItemsByMonth> items)
        {
            for (int j = 0; j < items.Count; j++) //looping all months
            {
                string bar = ("-----------------------------------------------------------------------------------------------------------------");
                string s = string.Format("\n|{0,-10} |{1,-25} |{2,-20} |{3,-20} |{4,-10} |{5,-15} |", "Month", "Details", "Duration (mins)", "Category", "Event ID", "Busy Time");
                Console.WriteLine(s);
                Console.WriteLine(bar);
                for (int i = 0; i < items[j].Items.Count; i++) //looping all items in month
                {
                    string itemString = string.Format("|\x1b[94m{0,-10}\x1b[0m |\x1b[94m{1,-25}\x1b[0m |\u001b[94m{2,-20}\u001b[0m |\u001b[94m{3,-20}\u001b[0m |\u001b[94m{4,-10}\u001b[0m |\u001b[94m{5,-15}\u001b[0m |",  items[j].Month, items[j].Items[i].ShortDescription, items[j].Items[i].DurationInMinutes, items[j].Items[i].Category, items[j].Items[i].EventID, items[j].Items[i].BusyTime);
                    Console.WriteLine(itemString);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public static void FormatPrintCalendarItemsCategory(List<CalendarItemsByCategory> items)
        {
            for (int j = 0; j < items.Count; j++) //looping all categories
            {
                string bar = ("---------------------------------------------------------------------------------------------------------------");
                string s = string.Format("\n|{0,-20} |{1,-25} |{2,-20} |{3,-20} |{4,-15} |", "Category", "Details",  "Duration (mins)", "Event ID", "Busy Time");
                Console.WriteLine(s);
                Console.WriteLine(bar);
                for (int i = 0; i < items[j].Items.Count; i++) //looping all items in category
                {
                    string itemString = string.Format("|\x1b[94m{0,-20}\x1b[0m |\x1b[94m{1,-25}\x1b[0m |\u001b[94m{2,-20}\u001b[0m |\u001b[94m{3,-20}\u001b[0m |\u001b[94m{4,-15}\u001b[0m |", items[j].Category, items[j].Items[i].ShortDescription, items[j].Items[i].DurationInMinutes, items[j].Items[i].EventID, items[j].Items[i].BusyTime);
                    Console.WriteLine(itemString);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calendarDictionary"></param>
        public static void FormatPrintCalendarItemsDict(List<Dictionary<string, object>> calendarDictionary)
        {
            foreach (var monthRecord in calendarDictionary)
            {
                Console.WriteLine($"\nMonth: {monthRecord["Month"]}");
                //checking if the record contains a total busy time
                if (monthRecord.ContainsKey("TotalBusyTime"))
                Console.WriteLine($"Total Busy Time: {monthRecord["TotalBusyTime"]} minutes\n");

                //looping over each category and their events and printing their details
                foreach (var categoryRecord in monthRecord.Keys)
                {
                    //check if the key starts with "items:" since this means it corresponds to a category
                    if (categoryRecord.StartsWith("items:"))
                    {
                        string categoryName = categoryRecord.Substring(6); //taking the category name for printing
                        List<CalendarItem> eventRecords = (List<CalendarItem>)monthRecord[categoryRecord];
                        Console.WriteLine($"\u001b[94mCategory: {categoryName} \u001b[0m, Total Busy Time: {monthRecord[categoryName]} minutes");

                        //looping over each calendar item and printing its details
                        foreach (var eventItem in eventRecords)
                            Console.WriteLine($"Event: {eventItem.ShortDescription}, Start Time: {eventItem.StartDateTime}, Duration: {eventItem.DurationInMinutes} minutes");
                    }
                }
            }
        }
    }
}