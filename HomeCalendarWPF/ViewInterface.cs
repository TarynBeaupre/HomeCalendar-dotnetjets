using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF
{
    internal interface ViewInterface
    {
        void NewCalendar(); // Creates a new calendar for user
        void OpenExistingCalendar(); // Opens an existing Calendar File
        void AddNewEvent(); // Add New Event to Calendar
        void AddNewCategory(); // Add New Category to Calendar
        void ShowError(string msg); // Show errors like missing infos to add
    }
}
