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
        bool HasSelectedDate();
        bool HasDurationValue();
    };
}
