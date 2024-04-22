﻿using Calendar;
using System.Data.SQLite;


namespace HomeCalendarWPF
{
    /// <summary>
    /// Represents the Presenter for Events Window in MVP design.
    /// </summary>
    public class EventsPresenter
    {
        private readonly EventsViewInterface view;
        private readonly HomeCalendar model;

        private List<Category> categoriesList;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsPresenter"/> class with the specified view and calendar file path.
        /// </summary>
        /// <param name="view">View interface for events.</param>
        /// <param name="path">File path to the calendar.</param>
        /// <example>
        /// <code>
        /// For this example, assume we have well implemented IView
        /// <![CDATA[
        /// view = IView;
        /// path = "./hello";
        /// presenter = new EventsPresenter(view, path);
        /// ]]></code></example>
        public EventsPresenter(EventsViewInterface view, string path)
        {
            this.model = new HomeCalendar(path, false);
            this.view = view;

            categoriesList = model.categories.List();
        }

        public void AddNewEvent(string details, int categoryId, DateTime? start, double duration, string categoryName)
        /// <summary>
        /// Adds a new event with the specified details, category ID, start time, and duration.
        /// </summary>
        /// <param name="details">Details of the event.</param>
        /// <param name="categoryId">ID of the category.</param>
        /// <param name="start">Start time of the event.</param>
        /// <param name="duration">Duration of the event.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// AddNewEvent("ok", 2, 10:24:15, 360)
        /// ]]>
        /// </code></example>
        public void AddNewEvent(string details, int categoryId, DateTime? start, double duration)
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

        /// <summary>
        /// Adds a new category with the specified name.
        /// </summary>
        /// <param name="categoryName">Name of the new category.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// AddNewCategory("job")
        /// ]]>
        /// </code></example>
        public void AddNewCategory(string categoryName)
        {
            // All new categories added in the events page will have the category type event
            Calendar.Category.CategoryType type = Calendar.Category.CategoryType.Event;
            try
            {
                model.categories.Add(categoryName, type);
                view.ShowMessage($"A new category {categoryName} of type Event has been added!");
            }
            catch (SQLiteException ex)
            {
                view.ShowError(ex.Message);
            }
        }
        /// <summary>
        /// Gets the default categories from the model and displays them in the view.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// GetDefaultCategories();
        /// ]]>
        /// </code></example>
        public void GetDefaultCategories()
        {
            view.ShowDefaultCategories(categoriesList);
        }
    }
}
