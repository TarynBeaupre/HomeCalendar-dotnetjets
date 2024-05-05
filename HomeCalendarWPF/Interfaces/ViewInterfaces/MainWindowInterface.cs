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
        void SetDefaultDateTime();
        void SetDefaultCategories(List<Category> categoryList);
        void SetEventsInGrid<T>(List<T> eventsList);
    }
}
