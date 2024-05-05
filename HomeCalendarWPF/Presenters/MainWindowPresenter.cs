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
using HomeCalendarWPF.Interfaces.Views;
using System.Windows.Controls;

namespace HomeCalendarWPF.Presenters
{
    /// <summary>
    /// Represents the Presenter for MainWindow in MVP design.
    /// </summary>
    public class MainWindowPresenter
    {
        // Links from view, model to Presenter
        private readonly MainWindowInterface? view;
        public readonly HomeCalendar? model;

        // Flags ==================================================
        public bool groupByMonthFlag = false, groupByCatFlag = false;

        // Filtering variables
        public bool filterByDateFlag = false, filterByCatFlag = false;
        // ========================================================

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
        public MainWindowPresenter(MainWindowInterface view)
        {
            InitializationParams initParams = GetInitParams();
            if (initParams.filePath is not null)
            {
                //GetTheme();

                model = new HomeCalendar(initParams.filePath, initParams.newDB);
                this.view = view;
                view.SetCalendarFilePath(initParams.filePath);
                SetDefaults();
            }
        }
        #endregion

        #region Initialization Methods
        private void SetDefaults()
        {
            List<Category> categoryList = model!.categories.List();
            view!.SetDefaultCategories(categoryList);
        }
        #endregion

        #region Public Methods
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
            view!.ShowMessage("If you close the next window, unsaved changes will be lost.");
        }
        /// <summary>
        /// Sets the color theme depening on the user's choice.
        /// </summary>
        /// <param name="theme">A string representing whether the theme should be set to light or dark.</param>
        /// <example><code><![CDATA[
        /// SetTheme("button_dark_theme");
        /// ]]></code></example>
        public void SetTheme(string theme)
        {
            SaveThemeSettingsToRegistry();
            if (theme == "button_dark_theme")
            {
                view!.SetThemeDark();
            }
            else if (theme == "button_light_theme")
            {
                view!.SetThemeLight();
            }
        }

        #endregion

        #region Private Methods
        private bool IsFirstUse()
        {
            // Credit for how to check if key exists in registry https://stackoverflow.com/a/4276150

            // Open software folder under HKEY_CURRENT_USER
            RegistryKey rKey = Registry.CurrentUser.OpenSubKey("Software", true)!;

            // Check if software folder in registry has our program's info (if not, must be first use)
            return !rKey.GetSubKeyNames().Contains(REGISTRY_SUB_KEY_NAME);
        }

        private void ReadyForUse()
        {
            // Credit for how to create & write to registry: https://stackoverflow.com/a/7230427 as well as C# Docs

            // Open software folder under HKEY_CURRENT_USER
            RegistryKey rKey = Registry.CurrentUser.OpenSubKey("Software", true)!;

            // creates our folder in the software folder
            rKey.CreateSubKey(REGISTRY_SUB_KEY_NAME);

            // Have to do it this way because just rKey.SetValue("FIRST_USE", 0) doesn't work (should work)
            string keyName = @$"HKEY_CURRENT_USER\Software\{REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "FIRST_USE", 0);
            Registry.SetValue(keyName, "DARK_THEME", 0);
        }

        private InitializationParams GetInitParams()
        {
            if (IsFirstUse())
                ReadyForUse();
            else
                GetTheme();

            FileSelectionWindow newFileSelectWindow = new FileSelectionWindow(darkMode);
            newFileSelectWindow.ShowDialog();

            return new InitializationParams(newFileSelectWindow.initParams.filePath, newFileSelectWindow.initParams.newDB);
        }

        private void GetTheme()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{REGISTRY_SUB_KEY_NAME}";
            var a = Registry.GetValue(keyName, "DARK_THEME", 0);
            int b = (int)a!;
            darkMode = b == 1 ? true : false;
        }

        private void SaveThemeSettingsToRegistry()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "DARK_THEME", darkMode == true ? 1 : 0);
        }

        public void SetGridEventsList(ref List<CalendarItem> eventsList, ref List<Dictionary<string, object>> eventsListByCatMonth,
            ref List<CalendarItemsByMonth> eventsListByMonth, ref List<CalendarItemsByCategory> eventsListByCategory, int filterCategoryId = 0, DateTime? filterByStartDate = null, DateTime? filterByEndDate = null)
        {
            // Presenter populates the list
            //! Yeah always passing all the lists is not super efficient...To improve - jh
            if (groupByMonthFlag && groupByCatFlag)
            {
                eventsListByCatMonth = model!.GetCalendarDictionaryByCategoryAndMonth(filterByStartDate, filterByEndDate, filterByCatFlag, filterCategoryId);
                view!.SetEventsInGrid(eventsListByCatMonth);
                view.SetGridColumns(groupByMonthFlag, groupByCatFlag);
            }
            else if (groupByMonthFlag)
            {
                eventsListByMonth = model!.GetCalendarItemsByMonth(filterByStartDate, filterByEndDate, filterByCatFlag, filterCategoryId);
                view!.SetEventsInGrid(eventsListByMonth);
                view.SetGridColumns(groupByMonthFlag, groupByCatFlag);


            }
            else if (groupByCatFlag)
            {
                eventsListByCategory = model!.GetCalendarItemsByCategory(filterByStartDate, filterByEndDate, filterByCatFlag, filterCategoryId);
                view!.SetEventsInGrid(eventsListByCategory);
                view.SetGridColumns(groupByMonthFlag, groupByCatFlag);

            }
            else
            {
                eventsList = model!.GetCalendarItems(filterByStartDate, filterByEndDate, filterByCatFlag, filterCategoryId);
                view!.SetEventsInGrid(eventsList);
                view.SetGridColumns(groupByMonthFlag, groupByCatFlag);
            }

        }

        /*
        internal void GetFilteredDateEvents(DateTime? selectedDate1, DateTime? selectedDate2, bool filterFlag, int categoryId)
        {
            List<CalendarItemsByMonth> filteredEvents = model.GetCalendarItemsByMonth(selectedDate1, selectedDate2, filterFlag, categoryId);
            view.ShowFilteredDateEventsInGrid(filteredEvents);
        }*/

        public void GetCategoryList(ref List<Category> cats)
        {
            cats = model!.categories.List();
        }
        public void DeleteEvent(CalendarItem? chosenEvent)
        {
            var eventId = chosenEvent!.EventID;
            // Delete the event in the db
            model!.events.Delete(eventId);
        }

        public void CheckValidDeletedEvent(CalendarItem eventToDelete)
        {
            if (groupByMonthFlag || groupByCatFlag)
            {
                view!.ShowError("Need to select singular event.");
                return;
            }
            else if (eventToDelete is null)
            {
                view!.ShowError("Event is null");
                return;
            }
        }

        public void FindFilter(CheckBox filterCategoryToggle, DatePicker filterStartDatePicker, DatePicker filterEndDatePicker)
        {
            if (filterCategoryToggle.IsChecked == true)
                filterByCatFlag = true;
            else
                filterByCatFlag = false;

            if (filterStartDatePicker.SelectedDate != null && filterEndDatePicker.SelectedDate != null)
                filterByDateFlag = true;
            else
                filterByDateFlag = false;
        }
        public void FindGroupBy(CheckBox GroupByMonthToggle, CheckBox GroupByCategoryToggle)
        {
            if (GroupByMonthToggle.IsChecked == true)
                groupByMonthFlag = true;
            else
                groupByMonthFlag = false;
            if (GroupByCategoryToggle.IsChecked == true)
                groupByCatFlag = true;
            else
                groupByCatFlag = false;
        }

    }
    #endregion
}