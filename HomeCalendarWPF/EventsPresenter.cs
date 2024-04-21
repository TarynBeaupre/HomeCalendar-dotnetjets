using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using static HomeCalendarWPF.MainWindow;


namespace HomeCalendarWPF
{
    public class EventsPresenter
    {
        private readonly EventsViewInterface view;
        private readonly HomeCalendar model;

        public EventsPresenter(EventsViewInterface view)
        {
            //Get file and db
            this.model = new HomeCalendar();
            this.view = view;
        }

        public void AddNewEvent(string details, int categoryId, DateTime? start, DateTime? end)
        {
            // Delegate to the model to add the new event
            double duration = Convert.ToDouble(end - start);
            if (start != null)
            {
                try
                {
                    model.events.Add((DateTime)start, categoryId, duration, details);
                    view.ShowMessage("Event successfully added!");
                }
                catch(SQLiteException ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        public void AddNewCategory(string categoryName)
        {
            // All new categories added in the events page will have the category type event
            Calendar.Category.CategoryType type = Calendar.Category.CategoryType.Event;
            try
            {
                model.categories.Add(categoryName, type);
                view.ShowMessage($"A new category {categoryName} has been added");
            }
            catch( SQLiteException ex)
            {
                ShowError(ex.Message);
            }
        }

        //Wondering if we even need this tbh?
        public void ShowError(string message)
        {
            view.ShowError($"Error: {message}");
        }
    }
}
