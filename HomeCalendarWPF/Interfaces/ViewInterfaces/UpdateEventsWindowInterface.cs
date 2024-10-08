﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using HomeCalendarWPF.Interfaces.ViewInterfaces;

namespace HomeCalendarWPF.Interfaces.Views
{
    internal interface UpdateEventsWindowInterface : EventViewInterface
    {
        /// <summary>
        /// Prefills all of the data fields with the information from the event the user decided to update.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// PopulateFields();
        /// ]]></code></example>
        void PopulateFields();
    }
}
