using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using HomeCalendarWPF;

namespace HomeCalendarWPF
{
    internal interface UpdateEventsWindowInterface
    {
        void ShowDefaultCategories(List<Category> categoriesList);
        void ShowDefaultDateTime();
        void PopulateFields();
        void ShowError(string message);
    }
}
