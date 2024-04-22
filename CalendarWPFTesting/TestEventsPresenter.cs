using Calendar;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeCalendarWPF;

namespace CalendarWPFTesting
{
    public class TestEventsPresenter : EventsViewInterface
    {
        public bool calledShowError, calledShowMessage, calledShowDefaultCat, calledShowDefaultDate, calledReset;

        public void ShowError(string message)
        {

        }
        public void ShowMessage(string message)
        {

        }
        public void ShowDefaultCategories(List<Category> categoriesList)
        {

        }
        public void ShowDefaultDateTime()
        {

        }
        public void ResetEventForm()
        {

        }
    }
}