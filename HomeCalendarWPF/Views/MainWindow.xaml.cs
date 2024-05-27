using Calendar;
using DocumentFormat.OpenXml.Bibliography;
using HomeCalendarWPF.Interfaces.Views;
using HomeCalendarWPF.Presenters;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, MainWindowInterface
    {
        /// <summary>
        /// Represents initialization parameters used to configure the model.
        /// </summary>
        public struct InitializationParams
        {
            public string filePath;
            public bool newDB;

            /// <summary>
            /// Initializes a new instance of the <see cref="InitializationParams"/> struct with the specified file path and the new database indicator.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <param name="newDB">Indicates whether a new database needs to be created.</param>
            public InitializationParams(string filePath, bool newDB)
            {
                this.filePath = filePath; this.newDB = newDB;
            }
        }
        public static bool darkMode = false;
        bool awaitFilterInput = false;

        // My variables for the grid table
        public List<CalendarItem> eventsGridList = new();
        public List<CalendarItemsByMonth> eventsGridListByMonth = new();
        public List<CalendarItemsByCategory> eventsGridListByCat = new();
        public List<Dictionary<string, object>> eventsGridListByCatAndMonth = new();

        public static readonly string REGISTRY_SUB_KEY_NAME = "DotNetJetsCalendar";

        private MainWindowPresenter presenter;

        /// <summary>
        /// Initializes a new instance of the  <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            presenter = new MainWindowPresenter(this);
            filterStartDatePicker.SelectedDate = DateTime.Now.AddYears(-1);
            filterEndDatePicker.SelectedDate = DateTime.Now;
            if (calendarFiletxb.Text != "path here")
            {
                if (darkMode)
                    SetThemeDark();
                else
                    SetThemeLight();
                awaitFilterInput = true;
           

                // Output the default events
                RefreshGrid();
            }
            else
            {
                Close();
            }
        }

        #region Event Handlers
        private void Btn_Click_ChangeDBFile(object sender, RoutedEventArgs e)
        {
            presenter = new MainWindowPresenter(this);
        }
        private void Btn_Click_Change_Theme(object sender, RoutedEventArgs e)
        {
            Button? clickedButton = sender as Button;
            MainWindow.darkMode = dark_theme_star.Visibility == Visibility.Collapsed;
            if (clickedButton != null)
            {
                string theme = clickedButton.Name;
                presenter.SetTheme(theme);
            }
        }
        private void Btn_Click_ShowWarning(object sender, RoutedEventArgs e)
        {
            presenter.ShowWarning();
            Application.Current.Shutdown();
        }
        private void Event_Update_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = EventsGrid.CurrentItem as CalendarItem;

            if (selectedItem is null)
                return;

            int selectedIndex = EventsGrid.SelectedIndex;

            var updateEventsWindow = new UpdateEventsWindow(presenter.model!, calendarFiletxb.Text, selectedItem);
            updateEventsWindow.ShowDialog();
            RefreshGrid();

            if (selectedIndex >= 0 && selectedIndex < EventsGrid.Items.Count)
            {
                EventsGrid.SelectedIndex = selectedIndex;
                EventsGrid.ScrollIntoView(EventsGrid.SelectedItem);

                // Put colored background on the selected event
                var selectedRow = EventsGrid.ItemContainerGenerator.ContainerFromIndex(selectedIndex) as DataGridRow;
                if (selectedRow != null)
                {
                    if (darkMode)
                        selectedRow.Background = Brushes.LightGray;
                    else
                        selectedRow.Background = Brushes.LightGreen;
                }
            }
        }
        private void Event_Delete_Click(object sender, RoutedEventArgs e)
        {
            var eventToDelete = EventsGrid.CurrentItem as CalendarItem;
            if (eventToDelete is null)
                return;

            presenter.CheckValidDeletedEvent(eventToDelete);

            var choice = MessageBox.Show("Are you sure you want to delete Event?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (choice == MessageBoxResult.Yes)
            {
                int selectedIndex = EventsGrid.SelectedIndex;

                presenter.DeleteEvent(eventToDelete);
                this.RefreshGrid();

                // Wrap around the selected index if necessary
                if (selectedIndex >= EventsGrid.Items.Count)
                {
                    selectedIndex = selectedIndex % EventsGrid.Items.Count;
                }

                // Select the next item if available (If last is deleted, goes back to first event selected)
                if (selectedIndex >= 0 && EventsGrid.Items.Count > 0)
                {
                    EventsGrid.SelectedIndex = selectedIndex;
                    EventsGrid.ScrollIntoView(EventsGrid.SelectedItem);

                    // Put colored background on the selected event
                    var selectedRow = EventsGrid.ItemContainerGenerator.ContainerFromIndex(selectedIndex) as DataGridRow;
                    if (selectedRow != null)
                    {
                        if (darkMode)
                            selectedRow.Background = Brushes.LightGray;
                        else
                            selectedRow.Background = Brushes.LightGreen;
                    }
                }
            }
        }
        private void Event_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void EventsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Type that EventsGrid.CurrentItem returns is object, but actual type of
            // the object is Dictionary<string, object> if you check with a.GetType()
            // so I immediately cast it as such
            var calendarItem = EventsGrid.CurrentItem as CalendarItem;

            if (calendarItem is null)
                return;

            int selectedIndex = EventsGrid.SelectedIndex;

            var updateEventsWindow = new UpdateEventsWindow(presenter.model!, calendarFiletxb.Text, calendarItem);
            updateEventsWindow.ShowDialog();
            RefreshGrid();

            if (selectedIndex >= 0 && selectedIndex < EventsGrid.Items.Count)
            {
                EventsGrid.SelectedIndex = selectedIndex;
                EventsGrid.ScrollIntoView(EventsGrid.SelectedItem);
                // Put colored background on the selected event
                var selectedRow = EventsGrid.ItemContainerGenerator.ContainerFromIndex(selectedIndex) as DataGridRow;
                if (selectedRow != null)
                {
                    if (darkMode)
                        selectedRow.Background = Brushes.LightGray;
                    else
                        selectedRow.Background = Brushes.LightGreen;
                }
            }
        }
        #endregion

        #region Interface Methods
        /// <summary>
        /// Sets the file path for the calendar and updates the path text block.
        /// </summary>
        /// <param name="filePath">THe path to the chosen file.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetCalendarFilePath("./hello.db");
        /// ]]>
        /// </code></example>
        public void SetCalendarFilePath(string filePath)
        {
            calendarFiletxb.Text = filePath;
        }
        /// <summary>
        /// Sets the theme of the application to light mode.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetThemeLight();
        /// ]]>
        /// </code></example>
        public void SetThemeLight()
        {
            // Change the string in Window.Background > ImageSource to light theme image
            background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
            light_theme_star.Visibility = Visibility.Visible;
            light_chicken_image.Visibility = Visibility.Visible;
            light_tree_image.Visibility = Visibility.Visible;

            left_sidemenu_gradient.Color = Colors.LightGreen;
            right_sidemenu_gradient.Color = Colors.LightGreen;
            calendar_gradient.Color = Colors.LightGreen;
            file_sidemenu_gradient.Color = Colors.LightGreen;

            dark_chicken_image.Visibility = Visibility.Collapsed;
            dark_theme_star.Visibility = Visibility.Collapsed;
            dark_tree_image.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Sets the theme of the application to dark mode.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetThemeDark();
        /// ]]>
        /// </code></example>
        public void SetThemeDark()
        {
            // Change the string in ImageSource to dark theme image
            background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop-dark.jpg", UriKind.Relative));
            dark_theme_star.Visibility = Visibility.Visible;
            dark_chicken_image.Visibility = Visibility.Visible;
            dark_tree_image.Visibility = Visibility.Visible;

            right_sidemenu_gradient.Color = Colors.Gray;
            left_sidemenu_gradient.Color = Colors.Gray;
            calendar_gradient.Color = Colors.Gray;
            file_sidemenu_gradient.Color = Colors.Gray;

            light_theme_star.Visibility = Visibility.Collapsed;
            light_chicken_image.Visibility = Visibility.Collapsed;
            light_tree_image.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Sets the categories combobox's itemsource.
        /// </summary>
        /// <param name="categoryList">The list to be made itemsource.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetDefaultCategories(new List<Categories>() {});
        /// ]]></code></example>
        public void SetDefaultCategories(List<Category> categoryList)
        {
            filterCategoryCmbx.SelectedItem = null;
            filterCategoryCmbx.ItemsSource = categoryList;
        }
        /// <summary>
        /// Sets the datagrid's itemsource.
        /// </summary>
        /// <typeparam name="T">Type of the data contained in the list.</typeparam>
        /// <param name="eventsList">List to be made itemsource.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetEventsInGrid(new List<Category>() {});
        /// ]]></code></example>
        public void SetEventsInGrid<T>(List<T> eventsList)
        {
            EventsGrid.ItemsSource = eventsList;
        }
        /// <summary>
        /// Shows messages to the user
        /// </summary>
        /// <param name="message">Error message</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowMessage("you're good.");
        /// ]]>
        /// </code></example>
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// Shows error messages to the user
        /// </summary>
        /// <param name="message">Error message</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowMessage("Error, you're bad.");
        /// ]]>
        /// </code></example>
        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region Private Methods

        private Style CreateTotalsRowStyle()
        {
            // Creates a trigger that adds right-aligned style to only the TOTALS row in datagrid
            var style = new Style(typeof(DataGridCell));

            var trigger = new DataTrigger
            {
                Binding = new Binding("[Month]"),
                Value = "TOTALS"
            };
            trigger.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            style.Triggers.Add(trigger);

            return style;
        }

        #region Filtering Events
        private void FilterChoiceChanged(object sender, RoutedEventArgs e)
        {
            // Populate the list with the right events
            if (awaitFilterInput)
            {
                presenter.FindFilter(filterCategoryToggle, filterStartDatePicker, filterEndDatePicker);
                Category filterCategorySelected = filterCategoryCmbx.SelectedItem as Category;
                int filterCategoryId = filterCategorySelected!.Id;
                presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, filterCategoryId, filterStartDatePicker.SelectedDate, filterEndDatePicker.SelectedDate);
            }
        }

        #endregion

        #region Grid Methods
        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            // Calls a method that checks which checkbox is checked and changes value of global variable
            presenter.FindGroupBy(GroupByMonthToggle, GroupByCategoryToggle);

            // Populate the list with the right events
            //presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, groupByMonthFlag, groupByCatFlag);
            int filterCategoryId = filterCategoryCmbx.SelectedIndex + 1;
            presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat, filterCategoryId, filterStartDatePicker.SelectedDate, filterEndDatePicker.SelectedDate);
        }
        /// <summary>
        /// Sets all of the data grid's column names depending on the different group by flags specified.
        /// </summary>
        /// <param name="groupByMonthFlag">Specifies whether the information should be grouped by month.</param>
        /// <param name="groupByCatFlag">Specifies whether the information should be grouped by category.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// // Makes the data grid's columns those associated with the categories group by
        /// SetGridColumns(groupByMonthFlag=false, groupByCatFlag=true);
        /// ]]></code></example>
        public void SetGridColumns(bool groupByMonthFlag, bool groupByCatFlag)
        {
            // Columns to loop over for the normal events grid 
            List<string> columnProperties = new List<string>
            {
                "StartDate",
                "StartTime",
                "Category",
                "ShortDescription",
                "DurationInMinutes",
                "BusyTime"
            };
            List<string> header = new List<string>
            {
                "Start Date",
                "Start Time",
                "Category",
                "Description",
                "Duration",
                "Busy Time"
            };

            // Clear current columns
            EventsGrid.Columns.Clear();
            // Check which group by is active and create the columns depending on that
            if (!groupByMonthFlag && !groupByCatFlag)
            {
                for (int i = 0; i < columnProperties.Count(); i++)
                {
                    var column = new DataGridTextColumn();
                    column.Header = header[i];

                    if (columnProperties[i] == "StartDate")
                    {
                        column.Binding = new Binding("StartDateTime");
                        column.Binding.StringFormat = "dd/MM/yyyy";
                    }
                    else if (columnProperties[i] == "StartTime")
                    {
                        column.Binding = new Binding("StartDateTime");
                        column.Binding.StringFormat = "hh:mm tt";
                    }
                    else
                        column.Binding = new Binding(columnProperties[i]);
                    
                    // Add the style right-aligned
                    if (columnProperties[i] == "DurationInMinutes" || columnProperties[i] == "BusyTime")
                    {
                        Style s = new Style();
                        s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
                        column.CellStyle = s;
                    }

                    EventsGrid.Columns.Add(column);
                }
            }
            else if (groupByMonthFlag && groupByCatFlag)
            {
                var monthColumn = new DataGridTextColumn();
                monthColumn.Header = "Month";
                monthColumn.Binding = new Binding("[Month]");
                EventsGrid.Columns.Add(monthColumn);

                List<Category> cats = new();
                presenter.GetCategoryList(ref cats);

                foreach (var cat in cats)
                {
                    var column = new DataGridTextColumn();
                    column.Header = cat.Description;
                    column.Binding = new Binding($"[{cat.Description}]");
                    column.CellStyle = CreateTotalsRowStyle();
                    EventsGrid.Columns.Add(column);
                }

                // Get the busy time column
                var TBTcolumn = new DataGridTextColumn();
                TBTcolumn.Header = "TotalBusyTime";
                TBTcolumn.Binding = new Binding($"[TotalBusyTime]") { StringFormat = "0.00" };
                EventsGrid.Columns.Add(TBTcolumn);
            }
            else if (groupByMonthFlag)
            {
                // Month column
                var column = new DataGridTextColumn();
                column.Header = "Month";
                column.Binding = new Binding("Month");
                EventsGrid.Columns.Add(column);

                // Total Busy Time
                column = new DataGridTextColumn();
                column.Header = "Total Busy Time";
                column.Binding = new Binding("TotalBusyTime") { StringFormat = "0.00" };

                // Add the styles
                Style s = new Style();
                s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
                column.CellStyle = s;

                EventsGrid.Columns.Add(column);
            }

            else if (groupByCatFlag)
            {
                // Category column
                var column = new DataGridTextColumn();
                column.Header = "Category";
                column.Binding = new Binding("Category");
                EventsGrid.Columns.Add(column);

                // Total Busy Time
                column = new DataGridTextColumn();
                column.Header = "Total Busy Time";
                column.Binding = new Binding("TotalBusyTime") { StringFormat = "0.00" };
                
                // Add the styles
                Style s = new Style();
                s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
                column.CellStyle = s;

                EventsGrid.Columns.Add(column);

                int filterCategoryId = filterCategoryCmbx.SelectedIndex + 1;
            }
        }
        private void RefreshGrid()
        {
            presenter.SetGridEventsList(ref eventsGridList, ref eventsGridListByCatAndMonth, ref eventsGridListByMonth, ref eventsGridListByCat);
        }
        #endregion

        private void OpenEvent(object sender, RoutedEventArgs e)
        {
            EventsWindow eventWindow = new EventsWindow(presenter.model!, darkMode);
            eventWindow.ShowDialog();
            RefreshGrid();
        }
        private void OpenCategory(object sender, RoutedEventArgs e)
        {
            CategoriesWindow categoryWindow = new CategoriesWindow(presenter.model!, darkMode);
            categoryWindow.ShowDialog();
            RefreshGrid();
        }
        #endregion
    }

}

