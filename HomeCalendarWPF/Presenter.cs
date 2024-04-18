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

        public void AddEvent(string details, int categoryId, DateTime? start, DateTime? end, string fileName)
        {
            //TODO: Get the Category ID object from CategoryName selected

            //TODO: calculate the duration
            int durationInMinutes = 30;

            // Add a message in view
            view.AddNewEvent();

            //Add event in model
            model.events.Add(DateTime.Now, categoryId, durationInMinutes, details);
        }
    }
}
