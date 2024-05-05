using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace HomeCalendarWPF.Interfaces.ViewInterfaces
{
    public interface EventViewInterface : ViewInterface
    {
        /// <summary>
        /// Displays the default categories in the categories dropdown list.
        /// </summary>
        /// <param name="categoriesList">The list of default categories to display.</param>
        /// <example>
        /// <code>
        /// For this example, assume we have implemented a categoriesList
        /// <![CDATA[
        /// ShowDefaultCategories(categoriesList);
        /// ]]></code></example>
        void ShowDefaultCategories(List<Category> categoriesList);
        /// <summary>
        /// Sets the default date and time values on the window.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowDefaultDateTime();
        /// ]]></code></example>
        void ShowDefaultDateTime();
        /// <summary>
        /// Resets the event form by clearing input fields and setting default values.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ResetEventForm();
        /// ]]></code></example>
        void ResetEventForm();
        bool HasSelectedDate();
        bool IsEndValue();
        bool HasDurationValue();
    };
}
