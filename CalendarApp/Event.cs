// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: Event
    //        - An individual event for calendar program
    // ====================================================================
    /// <summary>
    /// Represents an individual event for the Calendar program.
    /// </summary>
    public class Event
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets the unique id of the event.
        /// </summary>
        /// <value>The unique identifier of the event.</value>
        public int Id { get; }
        /// <summary>
        /// Gets the start date and time of the event.
        /// </summary>
        /// <value>The start date and time.</value>
        public DateTime StartDateTime { get; }
        /// <summary>
        /// Gets or sets the duration of the event.
        /// </summary>
        /// <value>The duration of the event (in minutes)</value>
        public Double DurationInMinutes { get; set; }
        /// <summary>
        /// Gets or sets the details of the event.
        /// </summary>
        /// <value>The details of the event.</value>
        public String Details { get; set; }
        /// <summary>
        /// Gets or sets the id category of the event.
        /// </summary>
        /// <value>The unique identifier of the category of the event.</value>
        public int Category { get; set; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the event category exists in the
        //        categories object
        // ====================================================================
        /// <summary>
        /// Initializes an instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the Event object.</param>
        /// <param name="date">The start date and time of the event.</param>
        /// <param name="category">The id category of the event.</param>
        /// <param name="duration">The duration of the event in minutes.</param>
        /// <param name="details">Details about the event.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// int eventId = 1;
        /// DateTime eventDate = new DateTime(DateTime.Now);
        /// int eventCategory = 5;
        /// double eventDuration = 60;
        /// string eventDetails = "Homework";
        /// Event newEvent = new Event(eventId, eventDate, eventCategory, eventDuration, eventDetails);
        /// ]]>
        /// </code>
        /// </example>
        public Event(int id, DateTime date, int category, Double duration, String details)
        {
            this.Id = id;
            this.StartDateTime = date;
            this.Category = category;
            this.DurationInMinutes = duration;
            this.Details = details;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================
        /// <summary>
        /// Creates a deep copy using the properties of an existing <see cref="Event"/> object.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Event existingEvent = new Event(1, DateTime.Now, 5, 60, "Homework");
        /// Event newEvent = new Event(existingEvent);
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="obj">An existing Event object.</param>
        public Event(Event obj)
        {
            this.Id = obj.Id;
            this.StartDateTime = obj.StartDateTime;
            this.Category = obj.Category;
            this.DurationInMinutes = obj.DurationInMinutes;
            this.Details = obj.Details;

        }
    }
}
