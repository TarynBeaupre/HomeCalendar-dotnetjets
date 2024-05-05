using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            view.ShowMessage("Event successfully updated");

        }
        public bool ValidateEventForm()
        {
            //Check that start date has a value
            if (!view.IsDateSelected())
            {
                view.ShowError("Please select a start date.");
                return false;
            }

            // Check if duration is provided and is a positive double
            if (!view.IsValidDuration())
            {
                view.ShowError("Please provide a valid duration in minutes. The duration should be a positive number.");
                return false;
            }
            return true;
        }
    }
}
