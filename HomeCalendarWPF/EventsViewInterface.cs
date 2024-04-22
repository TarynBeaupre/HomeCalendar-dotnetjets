using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace HomeCalendarWPF
{
    public interface EventsViewInterface
    {
        void ShowError(string message);
        void ShowMessage(string message);
        void ShowDefaultCategories(List<Category> categoriesList);
        void ShowDefaultDateTime();
        void ResetEventForm();
    };
}
