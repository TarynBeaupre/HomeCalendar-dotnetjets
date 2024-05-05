using Calendar;
using HomeCalendarWPF.Interfaces.ViewInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.Interfaces.Views
{
    /// <summary>
    /// Represents the interface for the View component in MVP design.
    /// </summary>
    public interface MainWindowInterface : ViewInterface
    {
        /// <summary>
        /// Sets the file path for the calendar and updates the path text block.
        /// </summary>
        /// <param name="filePath">THe path to the chosen file.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetCalendarFilePath("./hello.db");
        /// ]]>
        /// </code></example>
        void SetCalendarFilePath(string filePath);
        /// <summary>
        /// Sets the theme of the application to light mode.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetThemeLight();
        /// ]]>
        /// </code></example>
        void SetThemeLight();
        /// <summary>
        /// Sets the theme of the application to dark mode.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetThemeDark();
        /// ]]>
        /// </code></example>
        void SetThemeDark();
        /// <summary>
        /// Sets the categories combobox's itemsource.
        /// </summary>
        /// <param name="categoryList">The list to be made itemsource.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetDefaultCategories(new List<Categories>() {});
        /// ]]></code></example>
        void SetDefaultCategories(List<Category> categoryList);
        /// <summary>
        /// Sets the datagrid's itemsource.
        /// </summary>
        /// <typeparam name="T">Type of the data contained in the list.</typeparam>
        /// <param name="eventsList">List to be made itemsource.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetEventsInGrid(new List<Category>() {});
        /// ]]></code></example>
        void SetEventsInGrid<T>(List<T> eventsList);
        /// <summary>
        /// Sets the grid column headers.
        /// </summary>
        /// <param name="groupByMonth">If true, columns for the by Month display will be shown.</param>
        /// <param name="groupByCat">if true, columns for the by Category display will be shown.</param>
        /// <remarks>If neither group is true, default column headers are shown.</remarks>
        void SetGridColumns(bool groupByMonth, bool groupByCat);
    }
}
