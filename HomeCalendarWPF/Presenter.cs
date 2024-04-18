using Calendar;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Data.SQLite;

namespace HomeCalendarWPF
{
    public class Presenter
    {
        // Links from view, model to Presenter
        private readonly ViewInterface view;
        private readonly HomeCalendar model;

        // Presenter constructor
        public Presenter(ViewInterface v, string filename, bool newDB = false)
        {
            model = new HomeCalendar(filename, newDB);
            view = v;
        }

        public void AddEvent(string name, string description, string categoryName, DateTime? start, DateTime? end, string text)
        {
            // Need to Get the Category object from CategoryName

            // Query to add the new event to db
            using var cmd = new SQLiteCommand(Database.dbConnection);

            cmd.CommandText = "INSERT INTO events (StartDateTime, Details, ";

            // Add a message in view, saying that 'event has been added'
            view.AddNewEvent();
        }
    }
}
