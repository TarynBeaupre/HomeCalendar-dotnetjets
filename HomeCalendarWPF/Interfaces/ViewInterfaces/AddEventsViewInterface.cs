using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using HomeCalendarWPF.Interfaces.ViewInterfaces;

namespace HomeCalendarWPF.Interfaces.Views
{
    /// <summary>
    /// Represents the interface for the Events view in the MVP design.
    /// </summary>
    public interface AddEventsViewInterface : EventViewInterface
    {
        /// <summary>
        /// Resets the event form by clearing input fields and setting default values.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ResetEventForm();
        /// ]]></code></example>
        void ResetEventForm();
        /// <summary>
        /// Checks if a date is selected in the view.
        /// </summary>
        /// <returns>A boolean representing whether a date is selected in the view.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (HasSelectedDate())
        ///     ShowMessage("Yep, that's a date that's selected");
        /// ]]></code></example>
        bool HasSelectedDate();
        /// <summary>
        /// Checks if a duration is set in the view.
        /// </summary>
        /// <returns>A boolean representing whether a duration is set in the view.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (HasDurationValue())
        ///     ShowMessage("Yep, that's a duration with a value");
        /// ]]></code></example>
        bool HasDurationValue();
    };
}
