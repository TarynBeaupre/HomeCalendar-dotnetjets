using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: CalendarItem
    //        A single calendar item, includes a Category and an Event
    // ====================================================================

    /// <summary>
    /// Represents an item in a Calendar, typically with a start date and time, description, busy time, Category and Event type.
    /// </summary>
    public class CalendarItem
    {
        /// <summary>
        /// Gets and sets the Id of the Category.
        /// </summary>
        /// <value>The Id of the Category.</value>
        public int CategoryID { get; set; }
        /// <summary>
        /// Gets and sets the Id of the Event.
        /// </summary>
        /// <value>The Id of the Event.</value>
        public int EventID { get; set; }
        /// <summary>
        /// Gets and sets the start date and time of the Calendar Item.
        /// </summary>
        /// <value>The start date and time of the Calendar Item.</value>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// Gets and sets the name of the Category.
        /// </summary>
        /// <value>The name of the Category. Can be null.</value>
        public String? Category { get; set; }
        /// <summary>
        /// Gets and sets the description of the Calendar Item.
        /// </summary>
        /// <value>A description of the Calendar Item. Can be null.</value>
        public String? ShortDescription { get; set; }
        /// <summary>
        /// Gets and sets the duration of the Calendar Item.
        /// </summary>
        /// <value>The duration of the Calendar Item in number of minutes.</value>
        public Double DurationInMinutes { get; set; }
        /// <summary>
        /// Gets and sets the duration of the busy time during the Calendar Item.
        /// </summary>
        /// <value>The duration of the busy time during the Calendar Item. (in minutes)</value>
        public Double BusyTime { get; set; }
    }

    /// <summary>
    /// Represents Calendar Items organized by Month.
    /// </summary>
    public class CalendarItemsByMonth
    {
        /// <summary>
        /// Gets and sets the month of the Calendar Item.
        /// </summary>
        /// <value>The month of the Calendar Item. Can be null.</value>
        public String? Month { get; set; }
        /// <summary>
        /// Gets and sets a List of Calendar Items in a specified month.
        /// </summary>
        /// <value>The List of Calendar Items in the specified month. Can be null.</value>
        public List<CalendarItem>? Items { get; set; }
        /// <summary>
        /// Gets and sets the total busy time for the Calendar Items in a specified month.
        /// </summary>
        /// <value>The total busy time for the Calendar Items in a specified month (in minutes).</value>
        public Double TotalBusyTime { get; set; }
    }

    /// <summary>
    /// Represents Calendar Items organized by Category.
    /// </summary>
    public class CalendarItemsByCategory
    {
        /// <summary>
        /// Gets and sets the category of the Calendar Item. 
        /// </summary>
        /// <value>The category of the Calendar Item. Can be null.</value>
        public String? Category { get; set; }
        /// <summary>
        /// Gets and sets a list of Calendar Items of the specified Category.
        /// </summary>
        /// <value>The list of Calendar Items of the specified Category. Can be null.</value>
        public List<CalendarItem>? Items { get; set; }
        /// <summary>
        /// Gets and sets the total busy time of the Calendar Items in the specified Cateory.
        /// </summary>
        /// <value>The total busy time of the Calendar Items in the specified Cateory (in minutes).</value>
        public Double TotalBusyTime { get; set; }

    }


}
