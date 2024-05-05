using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using System.Windows.Controls;

namespace HomeCalendarWPF.Presenters
{
    internal class UpdateEventsWindowPresenter
    {
        readonly private UpdateEventsWindowInterface view;
        readonly private HomeCalendar model;

        private List<Category> categoriesList;

        public UpdateEventsWindowPresenter(UpdateEventsWindowInterface view, HomeCalendar model)
        {
            this.view = view;
            this.model = model;
            categoriesList = model.categories.List();

            this.view.ShowDefaultCategories(categoriesList);
            this.view.ShowDefaultDateTime();
            this.view.PopulateFields();
        }

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
