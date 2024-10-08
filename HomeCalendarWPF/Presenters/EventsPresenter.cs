﻿using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("CalendarWPFTesting")]

namespace HomeCalendarWPF.Presenters
{
    /// <summary>
    /// Represents the Presenter for Events Window in MVP design.
    /// </summary>
    public class EventsPresenter
    {
        private readonly AddEventsViewInterface view;
        public readonly HomeCalendar model;

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
        /// path = "./calendar.db";
        /// presenter = new EventsPresenter(view, path);
        /// ]]></code></example>
        public EventsPresenter(AddEventsViewInterface view, HomeCalendar model, string path)
        {
            this.model = model;
            this.view = view;

            categoriesList = model.categories.List();
            view.ShowDefaultDateTime();
            view.ShowDefaultCategories(categoriesList);
        }

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
            Category.CategoryType type = Category.CategoryType.Event;
            try
            {
                view.ShowMessage($"A new category {categoryName} of type Event has been added!");
                model.categories.Add(categoryName, type);
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
            view.ShowDefaultCategories(model.categories.List());
        }
        /// <summary>
        /// Checks to see if the add event form has been filled correctly.
        /// </summary>
        /// <returns>A boolean representing if the add event form has been filled correctly.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (ValidateEventForm())
        ///     view.ShowMessage("That form's been stuffed full (of correct data)");
        /// ]]></code></example>
        public bool ValidateEventForm()
        {
            //Check that start date has a value
            if (!view.HasSelectedDate())
            {
                view.ShowError("Please select a start date.");
                return false;
            }

            // Check if duration is provided and is a positive double
            if (!view.HasDurationValue())
            {
                view.ShowError("Please provide a valid duration in minutes. The duration should be a positive number.");
                return false;
            }
            return true;
        }
    }
}
