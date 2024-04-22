using Calendar;
using System.Data.SQLite;


namespace HomeCalendarWPF
{
    public class EventsPresenter
    {
        private readonly EventsViewInterface view;
        private readonly HomeCalendar model;

        private List<Category> categoriesList;

        public EventsPresenter(EventsViewInterface view, string path)
        {
            this.model = new HomeCalendar(path, false);
            this.view = view;

            categoriesList = model.categories.List();
        }

        public void AddNewEvent(string details, int categoryId, DateTime? start, double duration, string categoryName)
        {
            // Delegate to the model to add the new event
            if (start == null)
                start = DateTime.Now;


            // Category doesn't exist
            if (categoryId == -1)
            {
                AddNewCategory(categoryName);
                categoryId = categoriesList.Count;

                categoriesList = model.categories.List();
            }

            try
            {
                // Have to increment category id because it's 1 too low (school should be 1, it's actually 0)
                model.events.Add((DateTime)start, ++categoryId, duration, details);
                view.ShowMessage("Event successfully added!");
                view.ResetEventForm();
            }
            catch (SQLiteException ex)
            {
                view.ShowError(ex.Message);
            }
        }

        public void AddNewCategory(string categoryName)
        {
            // All new categories added in the events page will have the category type event
            Calendar.Category.CategoryType type = Calendar.Category.CategoryType.Event;
            try
            {
                model.categories.Add(categoryName, type);
                view.ShowMessage($"A new category {categoryName} has been added!");
            }
            catch (SQLiteException ex)
            {
                view.ShowError(ex.Message);
            }
        }

        public void GetDefaultCategories()
        {
            view.ShowDefaultCategories(categoriesList);
        }
    }
}
