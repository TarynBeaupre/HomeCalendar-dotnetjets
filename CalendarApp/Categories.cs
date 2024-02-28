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
        private List<Category> _Categories = new List<Category>();
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

                using var cmd = new SQLiteCommand(query, categoriesConnection);
                using SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string description = reader.GetString(1);
                    Category.CategoryType type = (Category.CategoryType)reader.GetInt32(2); // Gets the typeId from db and typecast it to CategoryType

                    Category category = new Category(id, description, type);
                    _Categories.Add(category);
                }
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
            SetCategoriesToDefaults();
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
            var con = _Connection;

            //making a reader to retrieve the categories
            string stm = $"SELECT Description, TypeId FROM categories WHERE Id = {i}";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();
            string description = "";
            int id = 0, typeId = 0;
            while (rdr.Read())
            {
                id = rdr.GetInt32(0);
                description = rdr.GetString(1);
                typeId = rdr.GetInt32(2);
            }
            cmd.ExecuteNonQuery();
            Category newCategory = new Category(id, description, (Category.CategoryType)typeId);
            //if returned nothing, return null

            //if returned 
            return newCategory;
        }

        /// <summary>
        /// Populates the Categories property by reading data from a file.
        /// </summary>
        /// <param name="filepath">A given file path to read data from.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try 
        /// {
        ///     categories.ReadFromFile(categoriesInputFile);
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine(ex.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void ReadFromFile(String? filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current categories,
            // ---------------------------------------------------------------
            _Categories.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyReadFromFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // If file exists, read it
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        /// <summary>
        /// Writes data to a file.
        /// </summary>
        /// <param name="filepath">The file path to write data to.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try
        /// {
        ///     categories.SaveToFile(categorypath);
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine(ex.Message);
        /// }
        /// ]]></code></example>
        public void SaveToFile(String? filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = CalendarFiles.VerifyWriteToFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
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
            _Categories.Clear();

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
            _Categories.Add(category);
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
                int addedCategoryId = (int)con.LastInsertRowId;
                Category addedCategory = new Category(addedCategoryId, desc, type);
                _Categories.Add(addedCategory);
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
                //remove the category
                _Categories.Remove(GetCategoryFromId(Id));
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
                int typeid = (int)categoryType;
                cmd.Parameters.AddWithValue("@typeid", typeid + 1);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
                // have to update category object in 

                // create category with same id and give it new desc & type id
                Category updatedCategory = new Category(id, description, categoryType);

                // replace category in list
                _Categories[id - 1] = updatedCategory;
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
            foreach (Category category in _Categories)
            {
                newList.Add(new Category(category));
            }
            return newList;
        }

        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {

            // ---------------------------------------------------------------
            // read the categories from the xml file, and add to this instance
            // ---------------------------------------------------------------
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                foreach (XmlNode category in doc.DocumentElement.ChildNodes)
                {
                    String id = (((XmlElement)category).GetAttributeNode("ID")).InnerText;
                    String typestring = (((XmlElement)category).GetAttributeNode("type")).InnerText;
                    String desc = ((XmlElement)category).InnerText;

                    Category.CategoryType type;
                    switch (typestring.ToLower())
                    {
                        case "event":
                            type = Category.CategoryType.Event;
                            break;
                        case "alldayevent":
                            type = Category.CategoryType.AllDayEvent;
                            break;
                        case "holiday":
                            type = Category.CategoryType.Holiday;
                            break;
                        case "availability":
                            type = Category.CategoryType.Availability;
                            break;
                        default:
                            type = Category.CategoryType.Event;
                            break;
                    }
                    this.Add(new Category(int.Parse(id), desc, type));
                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadXMLFile: Reading XML " + e.Message);
            }

        }


        // ====================================================================
        // write all categories in our list to XML file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            try
            {
                // create top level element of categories
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Categories></Categories>");

                // foreach Category, create an new xml element
                foreach (Category cat in _Categories)
                {
                    XmlElement ele = doc.CreateElement("Category");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = cat.Id.ToString();
                    ele.SetAttributeNode(attr);
                    XmlAttribute type = doc.CreateAttribute("type");
                    type.Value = cat.Type.ToString();
                    ele.SetAttributeNode(type);

                    XmlText text = doc.CreateTextNode(cat.Description);
                    doc.DocumentElement.AppendChild(ele);
                    doc.DocumentElement.LastChild.AppendChild(text);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("_WriteXMLFile: Reading XML " + e.Message);
            }

        }

    }
}

