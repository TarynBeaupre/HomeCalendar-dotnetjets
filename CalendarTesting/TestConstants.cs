using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace CalendarCodeTests
{
    public class TestConstants
    {

        private static Event Event1 = new Event(1, new DateTime(2018, 1, 10), 3, 40, "App Dev Homework");
        private static CalendarItem CalendarItem1 = new CalendarItem
        {
            CategoryID = Event1.Category,
            EventID = Event1.Id,
            DurationInMinutes = Event1.DurationInMinutes
        };

        private static Event Event2 = new Event(2, new DateTime(2020, 1, 9), 9, 1440, "Honolulu");
        private static CalendarItem CalendarItem2 = new CalendarItem
        {
            CategoryID = Event2.Category,
            EventID = Event2.Id,
            DurationInMinutes = Event2.DurationInMinutes
        };


        private static CalendarItem CalendarItem3 = new CalendarItem
        {
            CategoryID = 9,
            EventID = 3,
            DurationInMinutes = 1440
        };

        private static Event Event4 = new Event(4, new DateTime(2020, 1, 20), 7, 180, "On call security");
        private static CalendarItem CalendarItem4 = new CalendarItem
        {
            CategoryID = Event4.Category,
            EventID = Event4.Id,
            DurationInMinutes = Event4.DurationInMinutes
        };


        private static Event Event5 = new Event(5, new DateTime(2020, 1, 11, 7, 30, 0), 2, 15, "staff meeting");
        private static CalendarItem CalendarItem5 = new CalendarItem
        {
            CategoryID = Event5.Category,
            EventID = Event5.Id,
            DurationInMinutes = Event5.DurationInMinutes
        };

        private static CalendarItem CalendarItem6 = new CalendarItem
        {
            CategoryID = 8,
            EventID = 6,
            DurationInMinutes = 1440
        };

        private static Event Event7 = new Event(7, new DateTime(2020, 1, 12), 11, 1440, "Wendy's birthday");
        private static CalendarItem CalendarItem7 = new CalendarItem
        {
            CategoryID = Event7.Category,
            EventID = Event7.Id,
            DurationInMinutes = Event7.DurationInMinutes
        };


        private static Event Event8 = new Event(8, new DateTime(2018, 1, 11, 10, 15, 0), 2, 60, "Sprint retrospective");
        private static CalendarItem CalendarItem8 = new CalendarItem
        {
            CategoryID = Event8.Category,
            EventID = Event8.Id,
            DurationInMinutes = Event8.DurationInMinutes
        };

        private static Event Event9 = new Event(9, new DateTime(2019, 1, 11, 9, 30, 0), 2, 60, "training");
        private static CalendarItem CalendarItem9 = new CalendarItem
        {
            CategoryID = 2,
            EventID = 9,
            DurationInMinutes = 60
        };
        public static int numberOfCategoriesInFile = 12;
        public static String testCategoriesInputFile = "test_categories.cats";
        public static String testDBInputFile = "testDBInput.db";
        public static int maxIDInCategoryInFile = 12;
        public static Category firstCategoryInFile = new Category(1, "School", Category.CategoryType.Event);
        public static int CategoryIDWithAllDayEventType = 9;
        public static int CategoryIDWithAvailabilityType = 6;
        public static string CategoriesOutputTestFile = "test_output.cats";

        public static int numberOfEventsInFile = 9;
        public static String testEventsInputFile = "test_events.evts";
        public static int maxIDInEventFile = 9;
        public static Event firstEventInFile { get { return Event1; } }
        public static string EventOutputTestFile = "test_output.evts";

        public static string testCalendarFile = "test.Calendar";
        public static string outputTestCalendarFile = "output_test.Calendar";

        public static List<Event> filteredbyCat3()
        {
            List<Event> filtered = new List<Event>();
            filtered.Add(Event1);
            return filtered;
        }
        public static double filteredbyCat2Total = Event5.DurationInMinutes + Event8.DurationInMinutes + Event9.DurationInMinutes;
        public static List<Event> filteredbyCat2()
        {
            List<Event> filtered = new List<Event>();
            filtered.Add(Event5);
            filtered.Add(Event8);
            filtered.Add(Event9);
            return filtered;
        }
        public static List<Event> filteredbyYear2018AndCategory3()
        {
            List<Event> filtered = new List<Event>();
            filtered.Add(Event1);
            return filtered;
        }

        public static List<Event> filteredbyYear2018()
        {
            List<Event> filtered = new List<Event>();
            filtered.Add(Event1);
            filtered.Add(Event5);
            filtered.Add(Event8);
            return filtered;
        }


        // LIST EventS BY MONTH
        public static int CalendarItemsByMonth_MaxRecords = 3;
        public static CalendarItemsByMonth CalendarItemsByMonth_FirstRecord = getCalendarItemsBy2018_01()[0];
        public static int CalendarItemsByMonth_FilteredByCat9_number = 1;
        public static CalendarItemsByMonth CalendarItemsByMonth_FirstRecord_FilteredCat9 = getCalendarItemsBy2020_01_filteredByCat9()[0];
        public static int CalendarItemsByMonth_2018_FilteredByCat2_number = 1;


        public static List<CalendarItemsByMonth> getCalendarItemsBy2018_01()
        {
            List<CalendarItemsByMonth> list = new List<CalendarItemsByMonth>();
            List<CalendarItem> CalendarItems = new List<CalendarItem>();

            CalendarItems.Add(CalendarItem1);
            CalendarItems.Add(CalendarItem8);
            CalendarItems.Add(CalendarItem5);


            list.Add(new CalendarItemsByMonth
            {
                // changed from 2018/01
                Month = "2018-01",
                Items = CalendarItems,
                TotalBusyTime = CalendarItem1.DurationInMinutes +
                                CalendarItem5.DurationInMinutes +
                                CalendarItem8.DurationInMinutes
            });
            return list;
        }

        public static List<CalendarItemsByMonth> getCalendarItemsBy2020_01_filteredByCat9()
        {
            List<CalendarItemsByMonth> list = new List<CalendarItemsByMonth>();
            List<CalendarItem> CalendarItems = new List<CalendarItem>();

            CalendarItems.Add(CalendarItem2);
            CalendarItems.Add(CalendarItem3);

            list.Add(new CalendarItemsByMonth
            {
                // changed from 2020/01
                Month = "2020-01",
                Items = CalendarItems,
                TotalBusyTime = CalendarItem2.DurationInMinutes + CalendarItem3.DurationInMinutes
            }); ; ;
            return list;
        }



        // LIST Events BY CATEGORY
        public static int CalendarItemsByCategory_MaxRecords = 6;
        public static CalendarItemsByCategory CalendarItemsByCategory_FirstRecord = getCalendarItemsByCategoryCat11()[0];
        public static int CalendarItemsByCategory11 = 1;
        public static int CalendarItemsByCategory20 = 0;


        public static List<CalendarItemsByCategory> getCalendarItemsByCategoryCat11()
        {
            List<CalendarItemsByCategory> list = new List<CalendarItemsByCategory>();
            List<CalendarItem> CalendarItems = new List<CalendarItem>();

            CalendarItems.Add(CalendarItem7);


            list.Add(new CalendarItemsByCategory
            {
                Category = "Birthdays",
                Items = CalendarItems,
                TotalBusyTime = CalendarItem7.DurationInMinutes
            });
            return list;
        }

        public static List<CalendarItemsByCategory> getCalendarItemsByCategory2018_Cat2()
        {
            List<CalendarItemsByCategory> list = new List<CalendarItemsByCategory>();
            List<CalendarItem> CalendarItems = new List<CalendarItem>();

            CalendarItems.Add(CalendarItem8);
            CalendarItems.Add(CalendarItem5);

            list.Add(new CalendarItemsByCategory
            {
                Category = "Work",
                Items = CalendarItems,
                TotalBusyTime = CalendarItem5.DurationInMinutes + CalendarItem8.DurationInMinutes
            }); ;
            return list;
        }

        public static List<CalendarItemsByCategory> getCalendarItemsByCategory2018()
        {
            List<CalendarItemsByCategory> list = new List<CalendarItemsByCategory>();
            List<CalendarItem> CalendarItems = new List<CalendarItem>();

            CalendarItems.Add(CalendarItem1);

            list.Add(new CalendarItemsByCategory
            {
                Category = "Fun",
                Items = CalendarItems,
                TotalBusyTime = CalendarItem1.DurationInMinutes
            });


            CalendarItems = new List<CalendarItem>();
            CalendarItems.Add(CalendarItem5);
            CalendarItems.Add(CalendarItem8);

            list.Add(new CalendarItemsByCategory
            {
                Category = "Work",
                Items = CalendarItems,
                TotalBusyTime = CalendarItem5.DurationInMinutes + CalendarItem8.DurationInMinutes
            }); ;
            return list;
        }




        // LIST EventS BY CATEGORY AND MONTH
        public static int CalendarItemsByCategoryAndMonth_MaxRecords = 3; // 3 months

        public static Dictionary<string, object> getCalendarItemsByCategoryAndMonthFirstRecord()
        {
            List<CalendarItem> CalendarItems;

            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "Month","2018/01" },{"TotalBusyTime", CalendarItem1.DurationInMinutes +
                                                        CalendarItem5.DurationInMinutes +
                                                        CalendarItem8.DurationInMinutes}  };


            CalendarItems = new List<CalendarItem>();
            CalendarItems.Add(CalendarItem1);

            dict.Add("items:Fun", CalendarItems);
            dict.Add("Fun", CalendarItem1.DurationInMinutes);


            CalendarItems = new List<CalendarItem>();

            CalendarItems.Add(CalendarItem8);
            CalendarItems.Add(CalendarItem5);

            dict.Add("items:Work", CalendarItems);
            dict.Add("Work", CalendarItem5.DurationInMinutes + CalendarItem8.DurationInMinutes);



            return dict;
        }

        public static Dictionary<string, object> getCalendarItemsByCategoryAndMonthTotalsRecord()
        {
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "Month","TOTALS" }  };
            dict.Add("Work", CalendarItem5.DurationInMinutes + CalendarItem8.DurationInMinutes 
                             + CalendarItem9.DurationInMinutes);
            dict.Add("Fun", CalendarItem1.DurationInMinutes);
            dict.Add("On call", CalendarItem4.DurationInMinutes);
            dict.Add("Canadian Holidays", CalendarItem6.DurationInMinutes);
            dict.Add("Vacation", CalendarItem2.DurationInMinutes + CalendarItem3.DurationInMinutes);
            dict.Add("Birthdays", CalendarItem7.DurationInMinutes);

            return dict;
        }

        public static List<Dictionary<string, object>> getCalendarItemsByCategoryAndMonthCat2()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            List<CalendarItem> CalendarItems = new List<CalendarItem>();

            CalendarItems.Add(CalendarItem5);
            CalendarItems.Add(CalendarItem8);

            list.Add(new Dictionary<string, object> {
                {"Month","2018/01" },
                { "TotalBusyTime", CalendarItem5.DurationInMinutes + CalendarItem8.DurationInMinutes },

                { "Work",CalendarItem5.DurationInMinutes + CalendarItem8.DurationInMinutes},

                }
            );

            CalendarItems = new List<CalendarItem>();

            CalendarItems.Add(CalendarItem9);

            list.Add(new Dictionary<string, object> {
                {"Month","2019/01" },
                { "TotalBusyTime", CalendarItem9.DurationInMinutes },
                {"items:Work",CalendarItems },
                { "Work",CalendarItem9.DurationInMinutes},
                }
            );

            list.Add(new Dictionary<string, object> {
                {"Month","TOTALS" },
                { "Work",CalendarItem5.DurationInMinutes + CalendarItem8.DurationInMinutes +
                            CalendarItem9.DurationInMinutes},
                }
            );

            return list;
        }


        public static List<Dictionary<string, object>> getCalendarItemsByCategoryAndMonth2020()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            list.Add(new Dictionary<string, object> {
                {"Month","2020/01"},
                { "TotalBusyTime", CalendarItem7.DurationInMinutes + CalendarItem6.DurationInMinutes +
                                   CalendarItem4.DurationInMinutes + CalendarItem3.DurationInMinutes +
                                   CalendarItem2.DurationInMinutes },
                {"items:Birthdays", new List<CalendarItem>{CalendarItem7} },
                { "Birthdays",CalendarItem7.DurationInMinutes },
                { "items:Canadian Holidays", new List<CalendarItem> { CalendarItem6 } },
                { "Canadian Holidays",CalendarItem6.DurationInMinutes },
                { "items:On call", new List<CalendarItem> { CalendarItem4 } },
                { "On call",CalendarItem4.DurationInMinutes },
                { "items:Vacation",new List<CalendarItem> { CalendarItem2, CalendarItem3 } },
                { "Vacation",CalendarItem2.DurationInMinutes + CalendarItem3.DurationInMinutes}
                }
            );

            list.Add(new Dictionary<string, object> {
                {"Month","TOTALS" },
                { "On call",CalendarItem4.DurationInMinutes},
                {"Canadian Holidays",CalendarItem6.DurationInMinutes },
                { "Vacation",CalendarItem2.DurationInMinutes+ CalendarItem3.DurationInMinutes},
                {"Birthdays",CalendarItem7.DurationInMinutes },
                }
             );



            return list;
        }

        static public String GetSolutionDir()
        {

            // this is valid for C# .Net Foundation (not for C# .Net Core)
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
        }

        // source taken from: https://www.dotnetperls.com/file-equals

        static public bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        static public bool FileSameSize(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            return (file1.Length == file2.Length);
        }
    }
}




