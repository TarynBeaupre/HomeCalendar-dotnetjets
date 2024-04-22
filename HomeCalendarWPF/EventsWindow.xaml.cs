using Calendar;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class EventsWindow : Window, EventsViewInterface
    {
        private EventsPresenter presenter;
        private DateTime previousDate = System.DateTime.Now;
        private int previousCategoryIndex = 0;

        public EventsWindow(bool darkmode)
        {
            InitializeComponent();
            txbCalendarFileinEvents.Text = ((MainWindow)Application.Current.MainWindow).calendarFiletxb.Text;
            string filePath = txbCalendarFileinEvents.Text;
            this.presenter = new EventsPresenter(this, filePath);

            // Sets default date and times on the window
            ShowDefaultDateTime();
            // Sets default categories on the window
            presenter.GetDefaultCategories();


            // Set the theme from the mainWindow
            SetTheme(darkmode);
        }

        private void LoadPreviousValues()
        {
            // Load previous date


            // Load previous category index

        }

        // If user types in a category that doesn't exist, runs the addCategory code
        public void AddNewCategory()
        {
            ComboBoxItem typeItem = (ComboBoxItem)categoriescmb.SelectedItem;
            string categoryName = typeItem.Content.ToString();
            presenter.AddNewCategory(categoryName);
        }

        public void Btn_Click_Add_Event(object sender, RoutedEventArgs e)
        {
            //Add the event to the database and the view calendar via the presenter
            string details = eventDescription.Text;
            int categoryId = categoriescmb.SelectedIndex;

            if (!startdp.SelectedDate.HasValue)
            {
                startdp.SelectedDate = System.DateTime.Now;
            }

            previousDate = (DateTime)startdp.SelectedDate;
            //Add checks here for a double value entered
            double duration = Convert.ToDouble(txbDuration.Text);

            //Replacing previous options
            previousCategoryIndex = categoryId;

            presenter.AddNewEvent(details, categoryId, startdp.SelectedDate, duration);
        }
        public void Btn_Click_Cancel_Event(object sender, EventArgs e)
        {
            // if user cancels addition, resent the default values for the category and the dates
            this.Close();
        }

        private void SetTheme(bool darkmode)
        {
            if (darkmode)
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop-dark.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.Gray;
                light_theme_star.Visibility = Visibility.Collapsed;
                dark_theme_star.Visibility = Visibility.Visible;
            }
            else
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.LightGreen;
                light_theme_star.Visibility = Visibility.Visible;
                dark_theme_star.Visibility = Visibility.Collapsed;
            }
        }

        public void ShowDefaultCategories(List<Category> categoriesList)
        {
            categoriescmb.SelectedIndex = previousCategoryIndex;
            categoriescmb.ItemsSource = categoriesList;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowDefaultDateTime()
        {
            startdp.SelectedDate = previousDate;
            int startHour;
            DateTime date = System.DateTime.Now;

            List<string> hourList = new List<string> { };
            for (int i = 1; i <= 24; i++)
            {
                hourList.Add(i.ToString());
            }
            List<string> minList = new List<string>
            {
                "00", "15", "30", "45"
            };

            cmbStartTimeHour.ItemsSource = hourList;


            cmbStartTimeMins.ItemsSource = minList;
            if (date.Minute < 30)
            {
                startHour = date.Hour;
                cmbStartTimeMins.SelectedIndex = 2;
            }
            else
            {
                startHour = date.Hour + 1;
                cmbStartTimeMins.SelectedIndex = 0;
            }
            cmbStartTimeHour.SelectedIndex = startHour - 1;

            //Default duration to 30mins
            txbDuration.Text = "30";
        }
    }
}
