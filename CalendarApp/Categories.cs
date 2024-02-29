using System.Data.SQLite;
using System.Xml;
using static Calendar.Category;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    /// Defines the possible Category objects to be applied to a Calendar Item.
    /// </summary>
    public class Categories
    {
        private static String DefaultFileName = "calendarCategories.txt";
        private string? _FileName;
        private string? _DirName;
        private SQLiteConnection _Connection;

        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets the file name.
        /// </summary>
        /// <value>A file name. May be null.</value>
        public String? FileName { get { return _FileName; } }
        /// <summary>
        /// Get the directory name.
        /// </summary>
        /// <value>A directory name. May be null.</value>
        public String? DirName { get { return _DirName; } }

        public SQLiteConnection Connection { get { return _Connection; } }

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Initializes a new instance of the <see cref="Categories"/> class with the default Categories.
        /// <example>
        /// <code><![CDATA[
        /// Categories categories = new Categories();]]></code></example>
        /// </summary>
        public Categories()
        {
            SetCategoriesToDefaults();
        }

        public Categories(SQLiteConnection categoriesConnection, bool newDB = false)
        {
            //Opening connection
            _Connection = categoriesConnection;
            // If the database is a NOT a new database, create Categories based on existing db file
            if (!newDB)
            {
                // Retrieve categories from db file 
                string query = "SELECT Id, Description, TypeId FROM categories";

                //! PURPOSE?
                //using var cmd = new SQLiteCommand(query, categoriesConnection);
                //using SQLiteDataReader reader = cmd.ExecuteReader();

                //while (reader.Read())
                //{
                //    int id = reader.GetInt32(0);
                //    string description = reader.GetString(1);
                //    Category.CategoryType type = (Category.CategoryType)reader.GetInt32(2); // Gets the typeId from db and typecast it to CategoryType

                //    Category category = new Category(id, description, type);
                //    _Categories.Add(category);
                //}
            }
            else
            {
                //Set categoryTypes to defaults
                SetCategoryTypesToDefaults();
                //Set categories to defaults
                SetCategoriesToDefaults();
            }
        }

        private void SetCategoryTypesToDefaults()
        {
            //Make this a loop?
            var con = Database.dbConnection;
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Event')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('AllDayEvent')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Holiday')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Availability')";
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================
        /// <summary>
        /// Retrieves a Category given a Category Id.
        /// </summary>
        /// <param name="i">The Category Id.</param>
        /// <returns>A Category object.</returns>
        /// <exception cref="Exception">Thrown if no Category object was found with the given Id.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try
        /// {
        ///     int categoryId = 5;
        ///     int retrievedCategory = categories.GetCategoryFromId(categoryId);
        /// }
        /// catch(Exception ex)
        /// {
        ///     Console.WriteLine(ex.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public Category GetCategoryFromId(int i)
        {
            int id = 0, type = 1;
            string description = "";
            var con = _Connection;

            //making a reader to retrieve the categories
            string stm = $"SELECT Id, Description, TypeId FROM categories WHERE Id = {i}";

            using var cmd = new SQLiteCommand(stm, con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetInt32(0);
                description = reader.GetString(1);
                type = reader.GetInt32(2);
            }
            Category foundCategory = new Category(id, description, (Category.CategoryType)type);
            //if returned nothing, return null

            //if returned 
            return foundCategory;
        }

        // ====================================================================
        // set categories to default
        // ====================================================================
        /// <summary>
        /// Initializes default Category objects.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// Categories categories = new Categories();
        /// SetCategoriesToDefaults();
        /// ]]>
        /// </code></example>
        public void SetCategoriesToDefaults()
        {

            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            var con = _Connection;
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = "DELETE FROM categories"; // Deletes every row in the specified table but not the table itself
            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------


            Add("School", Category.CategoryType.Event);
            Add("Personal", Category.CategoryType.Event);
            Add("VideoGames", Category.CategoryType.Event);
            Add("Medical", Category.CategoryType.Event);
            Add("Sleep", Category.CategoryType.Event);
            Add("Vacation", Category.CategoryType.AllDayEvent);
            Add("Travel days", Category.CategoryType.AllDayEvent);
            Add("Canadian Holidays", Category.CategoryType.Holiday);
            Add("US Holidays", Category.CategoryType.Holiday);
        }

        // ====================================================================
        // Add category
        // ====================================================================
        private void Add(Category category)
        {           
            var con = _Connection;
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = "INSERT INTO categories(Id, Description, TypeId) VALUES(@id, @desc, @typeid) RETURNING ID";
            cmd.Parameters.AddWithValue("@id", category.Id);
            cmd.Parameters.AddWithValue("@desc", category.Description);
            cmd.Parameters.AddWithValue("@typeid", category.Type);
        }

        /// <summary>
        /// Creates and adds a new Category object to the Categories List.
        /// </summary>
        /// <param name="desc">A description of the Category.</param>
        /// <param name="type">The type of the Category.</param>
        /// <example>
        /// The Category.CategoryType is the type of event that the new Category Type will have.
        /// <code>
        /// <![CDATA[
        /// Categories categories = new Categories();
        /// SetCategoriesToDefault(){
        ///     categories.Add("New Category", Category.CategoryType.Event);
        /// }
        /// ]]></code></example>
        public void Add(String desc, Category.CategoryType type)
        {
            try
            {
                //Opening connection
                var con = _Connection;
                using var cmd = new SQLiteCommand(con);

                //Insert into categories the new category
                //Check valid type id?
                //Check valid description length?
                cmd.CommandText = "INSERT INTO categories(Description, TypeId) VALUES(@desc, @typeid) RETURNING ID";
                cmd.Parameters.AddWithValue("@desc", desc);
                int typeid = (int)type;
                cmd.Parameters.AddWithValue("@typeid", typeid + 1);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        // ====================================================================
        // Delete category
        // ====================================================================
        /// <summary>
        /// Removes a Category from the Categories List.
        /// </summary>
        /// <param name="Id">The id of the Category to remove.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try 
        /// {
        ///     int indexOfCategoryToRemove = 5;
        ///     categories.Delete(indedOfCategoryToRemove);
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine(ex.Message);
        /// }
        /// ]]></code></example>
        public void Delete(int Id)
        {
            try
            {
                //connect to category
                var con = Database.dbConnection;
                using var cmd = new SQLiteCommand(con);
                //find the corresponding category with the id
                cmd.CommandText = $"DELETE FROM categories WHERE Id = {Id}";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateProperties(int id, string description, Category.CategoryType categoryType)
        {
            try
            {
                var con = _Connection;
                using var cmd = new SQLiteCommand(con);

                //cmd.CommandText = "INSERT INTO categories(Description, TypeId) VALUES(@desc, @typeid) RETURNING ID";
                cmd.CommandText = "UPDATE categories SET Description = @desc, TypeId = @typeid WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@typeid", (int)categoryType);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //throw new NotImplementedException();
        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Creates a new List of Categories.
        /// </summary>
        /// <returns>A new list of Category objects.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// List<Category> populatedCategories = categoriesInstance.List();
        /// ]]>
        /// </code></example>
        public List<Category> List()
        {
            List<Category> newList = new List<Category>();

            // Retrieve categories from db file 
            string query = "SELECT Id, Description, TypeId FROM categories";

            using var cmd = new SQLiteCommand(query, _Connection);
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string description = reader.GetString(1);
                Category.CategoryType type = (Category.CategoryType)reader.GetInt32(2);

                Category category = new Category(id, description, type);
                newList.Add(category);
            }
            return newList;
        }

    }
}

