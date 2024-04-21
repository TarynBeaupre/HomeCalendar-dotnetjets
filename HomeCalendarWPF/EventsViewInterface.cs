using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF
{
    public interface EventsViewInterface
    {
        void AddNewEvent();
        void AddNewCategory();
        void ShowError(string message);
        void ShowMessage(string message);
        void SetDefaults();
    };
}
