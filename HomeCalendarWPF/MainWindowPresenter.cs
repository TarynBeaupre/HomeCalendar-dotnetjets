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
using static HomeCalendarWPF.MainWindow;
using Microsoft.Win32;
using System.Windows.Media.Animation;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Represents the Presenter for MainWindow in MVP design.
    /// </summary>
    public class MainWindowPresenter
    {
        // Links from view, model to Presenter
        private readonly ViewInterface view;
        private readonly HomeCalendar model;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Presenter"/> class with the specified view interface.
        /// </summary>
        /// <param name="view">The view interface associated with the presenter.</param>
        /// <example>
        /// <code>
        /// For this example, assume we have well implemented IView
        /// <![CDATA[
        /// view = IView;
        /// presenter = new Presenter(view);
        /// ]]></code></example>
        public MainWindowPresenter(ViewInterface view)
        {
            InitializationParams initParams = this.GetInitParams();
            this.model = new HomeCalendar(initParams.filePath, initParams.newDB);
            this.view = view;
            view.SetCalendarFilePath(initParams.filePath);
            SetDefaults();
        }
        #endregion

        #region Initialization Methods
        private void SetDefaults()
        {
            view.SetDefaultDateTime();
            List<Category> categoryList  = model.categories.List();
            view.SetDefaultCategories(categoryList);
        }
        #endregion

        #region Public Methods
        public void FilterByCategory(int categoryIndex)
        {
            //1 get the events from the model
            //2 set the events into the grid
        }

        public void ShowAllEvents()
        {
            //show all events in the grid
        }

        /*
        /// <summary>
        /// Initializes a new instance of the <see cref="Presenter"/> class with the specified view interface, file path, and new database flag.
        /// </summary>
        /// <param name="view">View interface associated with the presenter.</param>
        /// <param name="filePath">The file path for the calendar.</param>
        /// <param name="newDB">A flag indicating whether a new database should be created.</param>
        /// <example>
        /// <code>
        /// For this example, assume we have well implemented IView
        /// <![CDATA[
        /// view = IView;
        /// presenter = new Presenter(view, "./hello", true);
        /// ]]></code></example>
        public MainWindowPresenter(ViewInterface view, string filePath, bool newDB = false)
        {
            this.model = new HomeCalendar(filePath, newDB);
            this.view = view;
        } */

        /// <summary>
        /// Shows a warning message to the user.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (!filepath)
        ///     ShowWarning();
        /// ]]></code></example>
        public void ShowWarning()
        {
            view.ShowMessage("If you close the next window, unsaved changes will be lost.");
        }

        public void SetTheme(string theme)
        {
            SaveThemeSettingsToRegistry();
            if (theme == "button_dark_theme")
            {
                view.SetThemeDark();
            }
            else if (theme == "button_light_theme")
            {
                view.SetThemeLight();
            }
        }

        #endregion

        #region Private Methods
        private bool IsFirstUse()
        {
            // Credit for how to check if key exists in registry https://stackoverflow.com/a/4276150

            // Open software folder under HKEY_CURRENT_USER
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            // Check if software folder in registry has our program's info (if not, must be first use)
            return !rKey.GetSubKeyNames().Contains(MainWindow.REGISTRY_SUB_KEY_NAME);
        }

        private void ReadyForUse()
        {
            // Credit for how to create & write to registry: https://stackoverflow.com/a/7230427 as well as C# Docs

            // Open software folder under HKEY_CURRENT_USER
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true)!;

            // creates our folder in the software folder
            rKey.CreateSubKey(REGISTRY_SUB_KEY_NAME);

            // Have to do it this way because just rKey.SetValue("FIRST_USE", 0) doesn't work (should work)
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "FIRST_USE", 0);
            Registry.SetValue(keyName, "DARK_THEME", 0);
        }

        private InitializationParams GetInitParams()
        {
            if (IsFirstUse())
                ReadyForUse();
            else
                GetTheme();

            FileSelectionWindow newFileSelectWindow = new FileSelectionWindow(MainWindow.darkMode);
            newFileSelectWindow.ShowDialog();

            return new InitializationParams(newFileSelectWindow.initParams.filePath, newFileSelectWindow.initParams.newDB);
        }

        private void GetTheme()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            var a = Registry.GetValue(keyName, "DARK_THEME", 0);
            int b = (int)a;
            MainWindow.darkMode = b == 1 ? true : false;
        }

        private void SaveThemeSettingsToRegistry()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "DARK_THEME", (MainWindow.darkMode == true) ? 1 : 0);
        }

        #endregion

        public void SetGridEventsList(ref List<Event> eventsList, ref List<Dictionary<string, object>> eventsListByCatMonth, 
            ref List<CalendarItemsByMonth> eventsListByMonth, ref List<CalendarItemsByCategory> eventsListByCategory,
            bool groupByMonth = false, bool groupByCat = false, bool filterByCat = false,  int filterCategoryId = 0, DateTime? filterByStartDate = null, DateTime? filterByEndDate = null)
        {
            // Presenter populates the list
            //! Yeah always passing all the lists is not super efficient...To improve - jh
            if (groupByMonth && groupByCat)
            {
                eventsListByCatMonth = model.GetCalendarDictionaryByCategoryAndMonth(filterByStartDate, filterByEndDate, filterByCat, filterCategoryId);
                view.SetEventsInGrid(eventsListByCatMonth);
            }
            else if (groupByMonth)
            {
                eventsListByMonth = model.GetCalendarItemsByMonth(filterByStartDate, filterByEndDate, filterByCat, filterCategoryId);
                view.SetEventsInGrid(eventsListByMonth);

            }
            else if (groupByCat)
            {
                eventsListByCategory = model.GetCalendarItemsByCategory(filterByStartDate, filterByEndDate, filterByCat, filterCategoryId);
                view.SetEventsInGrid(eventsListByCategory);
            }
            else if (filterByCat)
            {
                eventsListByCategory = model.GetCalendarItemsByCategory(filterByStartDate, filterByEndDate, filterByCat, filterCategoryId);
                if (eventsListByCategory.Count == 0)
                    view.ShowMessage("No events");
                else
                    view.SetEventsInGrid(eventsListByCategory);
            }
            else if (filterByStartDate != null || filterByEndDate != null)
            {
                eventsListByMonth = model.GetCalendarItemsByMonth(filterByStartDate, filterByEndDate, filterByCat, filterCategoryId);
                view.SetEventsInGrid(eventsListByMonth);

            }
            else
            {
                eventsList = model.events.List();
                view.SetEventsInGrid(eventsList);
            }

        }

        internal void GetFilteredDateEvents(DateTime? selectedDate1, DateTime? selectedDate2, bool filterFlag, int categoryId)
        {
            List<CalendarItemsByMonth> filteredEvents = model.GetCalendarItemsByMonth(selectedDate1, selectedDate2, filterFlag, categoryId);
            view.ShowFilteredDateEventsInGrid(filteredEvents);
        }
    }
}