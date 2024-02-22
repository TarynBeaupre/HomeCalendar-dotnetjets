using System;
using Xunit;
using Calendar;

namespace CalendarCodeTests
{
    [Collection("Sequential")]
    public class TestCategory
    {
        // ========================================================================

        [Fact]
        public void CategoryObject_New()
        {
            // Arrange
            string descr = "Project Coordination";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.AllDayEvent;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.IsType<Category>(category);
            Assert.Equal(id, category.Id);
            Assert.Equal(descr, category.Description);
            Assert.Equal(type, category.Type);
        }

        [Fact]
        public void CategoryObject_PropertiesAreReadOnly()
        {
            // Arrange
            string descr = "Project Coordination";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Holiday;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.IsType<Category>(category);
            Assert.True(typeof(Category).GetProperty("Id").CanWrite == false);
            Assert.True(typeof(Category).GetProperty("Description").CanWrite == false);
            Assert.True(typeof(Category).GetProperty("Type").CanWrite == false);
        }


        // ========================================================================

        [Fact]
        public void CategoryObject_New_WithDefaultType()
        {

            // Arrange
            string descr = "Project Coordination";
            int id = 42;
            Category.CategoryType defaultType = Category.CategoryType.Event;

            // Act
            Category category = new Category(id, descr);

            // Assert 
            Assert.Equal(defaultType, category.Type);
        }

        // ========================================================================

        [Fact]
        public void CategoryObject_New_Event()
        {

            // Arrange
            string descr = "Work";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Event;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.Equal(type, category.Type);

        }

        // ========================================================================

        [Fact]
        public void CategoryObjectType_New_Availability()
        {

            // Arrange
            string descr = "Busy";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Availability;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.Equal(type, category.Type);

        }

        // ========================================================================

        [Fact]
        public void CategoryObject_New_Holiday()
        {

            // Arrange
            string descr = "Religious Dates";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Holiday;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.Equal(type, category.Type);

        }

        // ========================================================================

        [Fact]
        public void CategoryObject_New_AllDayEvent()
        {

            // Arrange
            string descr = "Festival of Lights";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.AllDayEvent;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.Equal(type, category.Type);

        }


        // ========================================================================

        [Fact]
        public void CategoryObject_ToString()
        {

            // Arrange
            string descr = "Book vacation";
            int id = 42;

            // Act
            Category category = new Category(id, descr);

            // Assert 
            Assert.Equal(descr, category.ToString());
        }

    }
}

