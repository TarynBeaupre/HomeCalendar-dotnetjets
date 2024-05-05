using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.Presenters
{
    internal class UpdateEventsWindowPresenter
    {
        readonly private UpdateEventsWindowInterface view;
        readonly public HomeCalendar model;

        public UpdateEventsWindowPresenter(UpdateEventsWindowInterface view, HomeCalendar model)
        {
            this.view = view;
            this.model = model;

            this.view.ShowDefaultCategories(model.categories.List());
            this.view.ShowDefaultDateTime();
            this.view.PopulateFields();
        }
        public void GetDefaultCategories()
        {
            view.ShowDefaultCategories(model.categories.List());
        }
        public void UpdateEvent(int eventId, DateTime startDateTime, int categoryId, double durationInMinutes, string details)
        {
            model.events.UpdateProperties(eventId, startDateTime, categoryId + 1, durationInMinutes, details);
        }
    }
}
