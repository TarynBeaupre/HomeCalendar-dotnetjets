using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF
{
    internal class UpdateEventsWindowPresenter
    {
        readonly private UpdateEventsWindowInterface view;
        readonly private HomeCalendar model;

        private List<Category> categoriesList;

        public UpdateEventsWindowPresenter(UpdateEventsWindowInterface view, HomeCalendar model)
        {
            this.view = view;
            this.model = model;

            this.categoriesList = model.categories.List();

            this.view.ShowDefaultCategories(categoriesList);
            this.view.ShowDefaultDateTime();
            this.view.PopulateFields();
        } 
        public void GetDefaultCategories()
        {
            view.ShowDefaultCategories(categoriesList);
        }
        public void UpdateEvent(int eventId, DateTime startDateTime, int categoryId, double durationInMinutes, string details)
        {
            model.events.UpdateProperties(eventId, startDateTime, categoryId + 1, durationInMinutes, details);
        }
    }
}
