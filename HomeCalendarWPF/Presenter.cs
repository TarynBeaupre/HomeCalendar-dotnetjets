using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF
{
    internal class Presenter
    {
        // Links from view, model to Presenter
        private readonly ViewInterface view;
        private readonly HomeCalendar model;

        // Presenter constructor
        public Presenter(ViewInterface v)
        {
            //model = new HomeCalendar(); 
            view = v;
        }


    }
}
