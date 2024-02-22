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
    // CLASS: Category
    //        - An individual category for Calendar program
    //        - Valid category types: Event,AllDayEvent,Holiday
    // ====================================================================
    /// <summary>
    /// Represents a Category object used to categorize Calendar events.
    /// </summary>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets or sets the unique identifier of the Category.
        /// </summary>
        /// <value>The Id of the Category.</value>
        public int Id { get;}
        /// <summary>
        /// Gets or sets a description of the Category.
        /// </summary>
        /// <value>A string description of the Category.</value>
        public String Description { get;}
        /// <summary>
        /// Gets or sets the type of the Category.
        /// </summary>
        /// <value>A CategoryType value representing the type of the Category.</value>
        public CategoryType Type { get;}
        /// <summary>
        /// Represents the types of categories that can be associated with Calendar Items.
        /// </summary>
        public enum CategoryType
        {
            /// <summary>
            /// Represents a category associated with a regular event.
            /// </summary>
            Event,
            /// <summary>
            /// Represents a category associated with an all day event.
            /// </summary>
            AllDayEvent,
            /// <summary>
            /// Represents a category associated with a holiday.
            /// </summary>
            Holiday,
            /// <summary>
            /// Represents a category associated with an availability.
            /// </summary>
            Availability
        };

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Creates a new instance of the <see cref="Category"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the Category.</param>
        /// <param name="description">A brief description of the Category.</param>
        /// <param name="type">The type of event associated with the Category.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// int categoryID = 5;
        /// string categoryDescription = "Homework";
        /// Category category = new Category(categoryId, categoryDescription, CategoryType.Event);
        /// ]]></code></example>
        public Category(int id, String description, CategoryType type = CategoryType.Event)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        // ====================================================================
        // Copy Constructor
        // ====================================================================
        /// <summary>
        /// Creates a new Category object with a the properties of an existing Category object.
        /// </summary>
        /// <param name="category">A Category object.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// existingCategoryObject = new Category(categoryId, categoryDescription, CategoryType.Event);
        /// Category category = new Category(existingCategoryObject);
        /// ]]>
        /// </code>
        /// </example>
        public Category(Category category)
        {
            this.Id = category.Id;;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================
        /// <summary>
        /// Returns the description of a Category object.
        /// </summary>
        /// <returns>The description property of the Category object.</returns>
        /// <example>
        /// <code>
        /// Category category = new Category(5, "Homework", CategoryType.Event);
        /// string categoryDescription = category.ToString();
        /// </code>
        /// </example>
        public override string ToString()
        {
            return Description;
        }

    }
}

