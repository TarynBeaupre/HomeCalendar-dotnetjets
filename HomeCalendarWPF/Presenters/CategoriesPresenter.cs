using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using System.Data.SQLite;
using System.Threading.Channels;
using HomeCalendarWPF.Interfaces.Views;

namespace HomeCalendarWPF.Presenters
{
    public class CategoriesPresenter
    {
        private readonly CategoriesViewInterface view;
        private readonly HomeCalendar model;

        private readonly Category.CategoryType[] categoryTypes;

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
        /// presenter = new CategoriesPresenter(view, path);
        /// ]]></code></example>
        public CategoriesPresenter(CategoriesViewInterface view, HomeCalendar model, string path)
        {
            this.view = view;
            this.model = model;

            categoryTypes = PopulateCategoryTypes();
        }
        /// <summary>
        /// Adds a new category with the specified description and type.
        /// </summary>
        /// <param name="description">The description of the category.</param>
        /// <param name="type">The type of the category.</param>
        /// <example>
        /// <code>
        /// For this example, assume we have well implemented IView
        /// <![CDATA[
        /// description = "Job";
        /// type = Category.CategoryType.Event;
        /// AddNewCategory(description, type);
        /// ]]></code></example>
        public void AddNewCategory(string description, Category.CategoryType type)
        {
            if (description == string.Empty)
            {
                view.ShowError("Please provide a description for the category.");
                return;
            }

            try
            {
                //TODO: This makes it bug out, says categories table doesnt exist in db
                model.categories.Add(description, type);
                view.ResetCategoriesForm();
                CategoriesWindow.previousCategoryTypeIndex = (int)type - 1;

                view.ShowMessage("Category successfully added!");
            }
            catch (SQLiteException e)
            {
                view.ShowError(e.Message);
            }
        }
        /// <summary>
        /// Retrieves the available category types and sets them in the combo box.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// GetCategoryTypes();
        /// ]]></code></example>
        public void GetCategoryTypes()
        {
            view.SetComboBoxOptions(categoryTypes);
        }
        private Category.CategoryType[] PopulateCategoryTypes()
        {
            return (Category.CategoryType[])Enum.GetValues(typeof(Category.CategoryType));
        }
    }
}
