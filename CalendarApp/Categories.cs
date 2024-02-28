using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Xml;

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
            _Connection = categoriesConnection;
            if (newDB)
            {
                SetCategoryTypesToDefaults();
                SetCategoriesToDefaults();
            }
            //create category table
        }

        private void SetCategoryTypesToDefaults()
        {
            //Make this a loop?
            using var con = Database.dbConnection;
            con.Open();
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
            using var con = Database.dbConnection;
            con.Open();
            //TODO: remove *
            string stm = "SELECT * FROM categories ";

            Category? c = _Categories.Find(x => x.Id == i);
            return c;
        }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================
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
            //TODO: how to connect?
            try
            {
                using var con = Database.dbConnection;
                con.Open();
                using var cmd = new SQLiteCommand(con);
                cmd.CommandText = "INSERT INTO categories(Description, TypeId) VALUES(@desc, @typeid)";

                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@typeid", type);
                cmd.Prepare();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //connect
            SQLiteConnection con = Database.dbConnection;
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = "INSERT INTO categories VALUES(@desc, @type)";
            cmd.Parameters.AddWithValue("@desc", desc);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
            Console.WriteLine("Category inserted");
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
                using var con = Database.dbConnection;
                con.Open();
                using var cmd = new SQLiteCommand(con);
                //find the corresponding category with the id
                cmd.CommandText = $"DELETE FROM categories WHERE Id = {Id}";
                //remove the category
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateProperties(int id, string description, Category.CategoryType categoryType)
        {
            throw new NotImplementedException();
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

