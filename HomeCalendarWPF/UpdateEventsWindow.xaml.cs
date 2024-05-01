using Calendar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for UpdateEventsWindow.xaml
    /// </summary>
    public partial class UpdateEventsWindow : Window, UpdateEventsWindowInterface
    {
        readonly private UpdateEventsWindowPresenter presenter;
        readonly private string dbPath;
        readonly private Dictionary<string, object> eventToUpdate;
        public static int previousCategoryIndex = 0;

        public UpdateEventsWindow(HomeCalendar model, string filePath, Dictionary<string, object> eventToUpdate)
        {
            InitializeComponent();
            this.eventToUpdate = eventToUpdate;
            this.dbPath = filePath;
            presenter = new UpdateEventsWindowPresenter(this, model);
        }

        public void ShowDefaultCategories(List<Category> categoriesList)
        {
            categoriescmb.SelectedIndex = previousCategoryIndex != -1 ? previousCategoryIndex : categoriesList.Count - 1;
            categoriescmb.ItemsSource = categoriesList;
        }
        public void PopulateFields()
        {
            txbEventDescription.Text = eventToUpdate["Description"] as string;
            txbCalendarFileinEvents.Text = dbPath;
            categoriescmb.SelectedIndex = (int)eventToUpdate["Category"] - 1;

            // TODO: Start / End date, Start Time, Duration
            var a = eventToUpdate;
            foreach (var b in a)
            {
                Trace.WriteLine(b.Key + ": " + b.Value);
            }
        }

        private void Btn_Click_AddNewCategory(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Btn_Click_UpdateEvent(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Btn_Click_CancelUpdate(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
