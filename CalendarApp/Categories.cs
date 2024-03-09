using System;
using System.Data.SQLite;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// Get the database connection.
        /// </summary>
        /// <value>A database connection. Cannot be null and needs to be valid.</value>
        //TODO: verify the connection
        public SQLiteConnection Connection { get { return _Connection; } }

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Sets default categories in the categories table within the database
        /// <example>
        /// <code><![CDATA[
        /// SetCategoriesToDefaults();
        /// ]]></code></example>
        /// </summary>
        public Categories()
        {
            SetCategoriesToDefaults();
        }

        /// <summary>
        /// Fills the categories and categoryTypes tables with default values if it is a newly created database. 
        /// </summary>
        /// <param name="categoriesConnection">A valid database connection.</param>
        /// <param name="newDB">If true, sets up default values in the database.</param>
        public Categories(SQLiteConnection categoriesConnection, bool newDB = false)
        {
            //Opening connection
            _Connection = categoriesConnection;
            // If the database is a NOT a new database, create Categories based on existing db file
            if (newDB)
            {
                //Set categoryTypes to defaults
                SetCategoryTypesToDefaults();
                //Set categories to defaults
                SetCategoriesToDefaults();
            }
        }

        private void SetCategoryTypesToDefaults()
        {
            var con = Database.dbConnection;
            using var cmd = new SQLiteCommand(con);
            cmd.CommandText = "DELETE FROM categoryTypes";
            cmd.ExecuteNonQuery();

            for (int i = 1; i <= Enum.GetNames(typeof(CategoryType)).Length; i++)
            {
                cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES(@enum)";
                cmd.Parameters.AddWithValue("@enum", ((CategoryType)i).ToString());
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
        }

        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================
        /// <summary>
        /// Retrieves a specific Category from the database given a Category Id.
        /// </summary>
        /// <param name="i">The Category Id.</param>
        /// <returns>The retrieved Category object.</returns>
        /// <exception cref="Exception">Thrown if no Category object was found in the database with the given Id.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try
        /// {
        ///   Database.existingDatabase(newDB);
        ///   SQLiteConnection conn = Database.dbConnection;
        ///   Categories categories = new Categories(conn, false);
        ///   int catID = 7;
        ///   Category category = categories.GetCategoryFromId(catID);
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

            using var cmd = new SQLiteCommand(con);

            //making a reader to retrieve the categories
            cmd.CommandText = $"SELECT Id, Description, TypeId FROM categories WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", i);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetInt32(0);
                description = reader.GetString(1);
                type = reader.GetInt32(2);
            }
            Category foundCategory = new Category(id, description, (Category.CategoryType)type);

            return foundCategory;
        }

        // ====================================================================
        // set categories to default
        // ====================================================================
        /// <summary>
        /// Initializes default Category objects. Clears any existing categories before adding new ones.
        /// </summary>
        /// <example>
        /// For this example, assume we have a valid connection to the database:
        /// <code>
        /// <![CDATA[
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
            // Deletes every row in the specified table but not the table itself
            cmd.CommandText = "DELETE FROM categories"; 
            cmd.ExecuteNonQuery();
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
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates and adds a new Category object to the Categories table.
        /// </summary>
        /// <param name="desc">A description of the Category.</param>
        /// <param name="type">The type of the Category.</param>
        /// <example>
        /// The Category.CategoryType is the type of event that the new Category Type will have. In the database, this will be stored as its integer value.
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
        /// Removes a specific Category from the categories table given a Category Id.
        /// </summary>
        /// <param name="Id">The id of the Category to remove.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try
        /// {
        ///     categories.Delete(IdToDelete);
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
                cmd.CommandText = $"DELETE FROM events WHERE CategoryId = @id";
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"DELETE FROM categories WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// Updates properties in the categories table.
        /// </summary>
        /// <param name="id">The id of the category to update.</param>
        /// <param name="description">An updated description.</param>
        /// <param name="categoryType">A category enum representing the type of category.</param>
        /// 
        public void UpdateProperties(int id, string description, Category.CategoryType categoryType)
        {
            try
            {
                var con = _Connection;
                using var cmd = new SQLiteCommand(con);

                int type = (int)categoryType + 1;
                cmd.CommandText = $"UPDATE categories SET Description = @desc, TypeId = @typeid WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@typeid", (int)categoryType);

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
        /// Creates a new List of Categories. Reads the categories stored in the database.
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

