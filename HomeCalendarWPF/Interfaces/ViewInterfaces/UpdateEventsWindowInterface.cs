using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using HomeCalendarWPF.Interfaces.ViewInterfaces;

namespace HomeCalendarWPF.Interfaces.Views
{
    internal interface UpdateEventsWindowInterface : ViewInterface
    {
        void ShowDefaultCategories(List<Category> categoriesList);
        void ShowDefaultDateTime();
        void PopulateFields();
    }
}
