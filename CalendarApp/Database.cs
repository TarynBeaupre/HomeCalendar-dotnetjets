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
            string connectionString = $"Data Source={filename}; Foreign Keys=1";

            _connection = new SQLiteConnection(connectionString);
            _connection.Open();

            using var cmd = new SQLiteCommand(_connection);

            /*  ==============================================================
             *  Need to drop tables because this creates a new DB.
             *  Use the existing database method to connect to an existing DB.
             *  ==============================================================
             */
            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "DROP TABLE IF EXISTS events";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS categoryTypes(" +
                                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                "Description TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS categories(" +
                                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                "Description TEXT," +
                                "TypeId INTEGER," +
                                "FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS events(" +
                                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                "StartDateTime TEXT," +
                                "Details TEXT," +
                                "DurationInMinutes DOUBLE," +
                                "CategoryId INTEGER," +
                                "FOREIGN KEY(CategoryId) REFERENCES categories(Id))";
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
