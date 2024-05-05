using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HomeCalendarWPF.Presenters
{
    internal class UpdateEventsWindowPresenter
    {
        readonly private UpdateEventsWindowInterface view;
        readonly public HomeCalendar model;

        public UpdateEventsWindowPresenter(UpdateEventsWindowInterface view, HomeCalendar model)
        {
            this.view = view;
            this.model = model;

            this.view.ShowDefaultCategories(model.categories.List());
            this.view.ShowDefaultDateTime();
            this.view.PopulateFields();
        }

        /// <summary>
        /// Updates an event in the database.
        /// </summary>
        /// <param name="eventId">The id of the event to be updated.</param>
        /// <param name="startdp">The datepicker element containing the start date.</param>
        /// <param name="categoryId">The id of the category to be updated to.</param>
        /// <param name="txbDuration">The textblock element containing the duration.</param>
        /// <param name="details">The new details of the event.</param>
        /// <param name="cmbStartTimeHour">The combobox containing the start time's hour.</param>
        /// <param name="cmbStartTimeMins">The combobox comtaining the start time's minutes.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// UpdateEvent(eventToUpdate.EventID, startdp, categoriescmb.SelectedIndex, txbDuration, txbEventDescription.Text, cmbStartTimeHour, cmbStartTimeMins);
        /// ]]></code></example>
        public void UpdateEvent(int eventId, DatePicker startdp, int categoryId, TextBox txbDuration, string details, ComboBox cmbStartTimeHour, ComboBox cmbStartTimeMins)
        {
            if (!ValidateEventForm(startdp, txbDuration))
                return;
            else
            {
                try
                {
                    var tmp = (DateTime)startdp.SelectedDate!;
                    var date = new DateTime(tmp.Year, tmp.Month, tmp.Day, int.Parse(cmbStartTimeHour.Text), int.Parse(cmbStartTimeMins.Text), 0);
                    double duration = Convert.ToDouble(txbDuration.Text);
                    model.events.UpdateProperties(eventId, date, categoryId + 1, duration, details);
                    view.ShowMessage("Event successfully updated!");
                }
                catch (Exception ex)
                {
                    view.ShowError(ex.Message);
                }
            }
        }
        /// <summary>
        /// Gets the categories list from the database.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// GetDefaultCategories();
        /// ]]></code></example>
        public void GetDefaultCategories()
        {
            view.ShowDefaultCategories(model.categories.List());
        }

        private bool ValidateEventForm(DatePicker startdp, TextBox txbDuration)
        {
            //Check that start date has a value
            if (!startdp.SelectedDate.HasValue)
            {
                view.ShowError("Please select a start date.");
                return false;
            }
            // Check if duration is provided and is a positive double
            else if (!double.TryParse(txbDuration.Text, out double duration) || duration <= 0)
            {
                view.ShowError("Please provide a valid duration in minutes. The duration should be a positive number.");
                return false;
            }
            else 
                return true;
        }
    }
}
