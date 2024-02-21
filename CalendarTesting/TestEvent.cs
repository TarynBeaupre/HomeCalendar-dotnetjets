using System;
using Xunit;
using Calendar;

namespace CalendarCodeTests
{
    public class TestEvent
    {
        // ========================================================================

        [Fact]
        public void EventObject_New()
        {

            // Arrange
            DateTime now = DateTime.Now;
            double DurationInMinutes = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;

            // Act
            Event Event = new Event(id, now, category, DurationInMinutes, descr);

            // Assert 
            Assert.IsType<Event>(Event);

            Assert.Equal(id, Event.Id);
            Assert.Equal(DurationInMinutes, Event.DurationInMinutes);
            Assert.Equal(descr, Event.Details);
            Assert.Equal(category, Event.Category);
            Assert.Equal(now, Event.StartDateTime);
        }

        // ========================================================================

        [Fact]
        public void EventCopyConstructoryIsDeepCopy()
        {

            // Arrange
            DateTime now = DateTime.Now;
            double DurationInMinutes = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;
            Event Event = new Event(id, now, category, DurationInMinutes, descr);

            // Act
            Event copy = new Event(Event);
            copy.DurationInMinutes = Event.DurationInMinutes + 15;

            // Assert 
            Assert.Equal(id, Event.Id);
            Assert.NotEqual(DurationInMinutes, copy.DurationInMinutes);
            Assert.Equal(Event.DurationInMinutes + 15, copy.DurationInMinutes);
            Assert.Equal(descr, Event.Details);
            Assert.Equal(category, Event.Category);
            Assert.Equal(now, Event.StartDateTime);
        }


        // ========================================================================

        [Fact]
        public void EventObjectGetSetProperties()
        {
            // question - why cannot I not change the date of an Event.  What if I got the date wrong?

            // Arrange
            DateTime now = DateTime.Now;
            double DurationInMinutes = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;
            double newDurationInMinutes = 54.55;
            string newDescr = "Angora Sweater";
            int newCategory = 38;

            Event Event = new Event(id, now, category, DurationInMinutes, descr);

            // Act
            Event.DurationInMinutes = newDurationInMinutes;
            Event.Category = newCategory;
            Event.Details = newDescr;

            // Assert 
            Assert.True(typeof(Event).GetProperty("StartDateTime").CanWrite == false);
            Assert.True(typeof(Event).GetProperty("Id").CanWrite == false);
            Assert.Equal(newDurationInMinutes, Event.DurationInMinutes);
            Assert.Equal(newDescr, Event.Details);
            Assert.Equal(newCategory, Event.Category);
        }


    }
}
