using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;
using System.Runtime.CompilerServices;

// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Calendar
{
    public class Database
    {

        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================
        public static void newDatabase(string filename)
        {

            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

            // your code
            string connectionString = $"URI=file:{filename}";

            _connection = new SQLiteConnection(connectionString);
            _connection.Open();

            using var cmd = new SQLiteCommand(_connection);

            // TODO: Determine if I need to do this
            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "DROP TABLE IF EXISTS events";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS categoryTypes(" +
                                "CategoryTypeId INTEGER PRIMARY KEY AUTOINCREMENT," +
                                "Description TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS categories(" +
                                "CategoryId INTEGER PRIMARY KEY AUTOINCREMENT," +
                                "Description TEXT," +
                                "CategoryTypeId INTEGER," +
                                "FOREIGN KEY(CategoryTypeId) REFERENCES categoryTypes(CategoryTypeId))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS events(" +
                                "EventId INTEGER PRIMARY KEY AUTOINCREMENT," +
                                "StartDateTime TEXT," +
                                "Details TEXT," +
                                "DurationInMinutes DOUBLE," +
                                "CategoryId INTEGER," +
                                "FOREIGN KEY(CategoryId) REFERENCES category(CategoryId))";
            cmd.ExecuteNonQuery();
        }

       // ===================================================================
       // open an existing database
       // ===================================================================
       public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            // your code
            string connectionString = $"URI=file:{filename}";

            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
        }

       // ===================================================================
       // close existing database, wait for garbage collector to
       // release the lock before continuing
       // ===================================================================
        static public void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();
                

                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }

}
