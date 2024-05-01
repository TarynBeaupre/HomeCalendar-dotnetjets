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


        // Presenter constructor
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
        }
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
        }
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
            {
                ReadyForUse();
            }
            else
            {
                GetTheme();
            }

            // IDK why i called this fop it should be fow (don't change fop sounds better)
            // Also renamed it so it makes even less sense now :shrug: -ec
            FileSelectionWindow fop = new FileSelectionWindow(MainWindow.darkMode);
            fop.ShowDialog();

            return new InitializationParams(fop.initParams.filePath, fop.initParams.newDB);
        }
        private void GetTheme()
        {
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            var a = Registry.GetValue(keyName, "DARK_THEME", 0);
            int b = (int)a;
            MainWindow.darkMode = b == 1 ? true : false;
            //MainWindow.darkMode = ((int)Registry.GetValue(keyName, "DARK_THEME", 0)! == 1) ? true : false;
        }

        public void SetTheme(string theme)
        {
            if (theme == "button_dark_theme")
            {
                view.SetThemeDark();
            }
            else if (theme == "button_light_theme")
            {
                view.SetThemeLight();
            }
        }

        public void SetGridEventsList(ref List<CalendarItem> eventsList, ref List<Dictionary<string, object>> eventsListByCatMonth, 
            ref List<CalendarItemsByMonth> eventsListByMonth, ref List<CalendarItemsByCategory> eventsListByCategory,
            bool groupByMonth = false, bool groupByCat = false)
        {
            // Presenter populates the list
            //! Yeah always passing all the lists is not super efficient...To improve - jh
            if (groupByMonth && groupByCat)
            {
                eventsListByCatMonth = model.GetCalendarDictionaryByCategoryAndMonth(null, null, false, 0);
                view.SetEventsInGrid(eventsListByCatMonth);
            }
            else if (groupByMonth)
            {
                eventsListByMonth = model.GetCalendarItemsByMonth(null, null, false, 0);
                view.SetEventsInGrid(eventsListByMonth);

            }
            else if (groupByCat)
            {
                eventsListByCategory = model.GetCalendarItemsByCategory(null, null, false, 0);
                view.SetEventsInGrid(eventsListByCategory);
            }
            else
            {
                eventsList = model.GetCalendarItems(null, null, false, 0);
                view.SetEventsInGrid(eventsList);
            }
        }

        public void DeleteEvent(CalendarItem chosenEvent)
        {
            var eventId = chosenEvent.EventID;

            // Delete the event in the db
            model.events.Delete(eventId);
        }
    }
}