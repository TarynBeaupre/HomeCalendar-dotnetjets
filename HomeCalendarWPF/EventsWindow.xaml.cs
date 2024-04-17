using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class EventsWindow : Window
    {
        private int defaultCategoryIndex = 0;

        public EventsWindow()
        {
            InitializeComponent();
            startdp.SelectedDate = System.DateTime.Now;
            enddp.SelectedDate = System.DateTime.Now;
            categoriescmb.SelectedIndex = defaultCategoryIndex;
            List<string> categoriesList = new List<string>
            {
                "Homework", "Event", "Work", "Meeting"
            };
            categoriescmb.ItemsSource = categoriesList;
        }
        public void Btn_Click_Add_Event(object sender, RoutedEventArgs e)
        {
            //Add the event to the database and the view calendar via the presenter
            
        }
        public void Btn_Click_Cancel_Event(object sender, EventArgs e)
        {
            // if user cancels addition, resent the default values for the category and the dates
            this.Close();
        }
    }
}
