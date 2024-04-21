﻿using Calendar;
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

namespace HomeCalendarWPF
{
    public class Presenter
    {
        // Links from view, model to Presenter
        private readonly ViewInterface view;
        private readonly HomeCalendar model;


        // Presenter constructor
        public Presenter(ViewInterface view, string filename, bool newDB = false)
        {
            this.model = new HomeCalendar(filename, newDB);
            this.view = view;
        }

        public void Initialize()
        {
            if (IsFirstUse())
            {
                ReadyForFirstUse();
            }

            FirstOpenWindow fop = new FirstOpenWindow();
            fop.ShowDialog();

            
            view.SetCalendarFilePath(fop.initParams.filePath);
        }

        public void ShowWarning()
        {
            view.ShowMessage("If you close the next window without saving, your changes will be lost.");
        }

        private bool IsFirstUse()
        {
            // Credit for how to check if key exists in registry https://stackoverflow.com/a/4276150

            // Open software folder under HKEY_CURRENT_USER
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            // Check if software folder in registry has our program's info (if not, must be first use)
            return !rKey.GetSubKeyNames().Contains(MainWindow.REGISTRY_SUB_KEY_NAME);
        }

        private void ReadyForFirstUse()
        {
            // Credit for how to create & write to registry: https://stackoverflow.com/a/7230427 as well as C# Docs

            // Open software folder under HKEY_CURRENT_USER
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true)!;

            // creates our folder in the software folder
            rKey.CreateSubKey(REGISTRY_SUB_KEY_NAME);

            // Have to do it this way because just rKey.SetValue("FIRST_USE", 0) doesn't work (should work)
            string keyName = @$"HKEY_CURRENT_USER\Software\{MainWindow.REGISTRY_SUB_KEY_NAME}";
            Registry.SetValue(keyName, "FIRST_USE", 0);
        }
    }
}



// Template code for Events presenter:
//public void AddEvent(string details, int categoryId, DateTime? start, DateTime? end, string fileName)
//{
//    //TODO: Get the Category ID object from CategoryName selected

//    //TODO: calculate the duration
//    int durationInMinutes = 30;

//    // Add a message in view
//    view.AddNewEvent();

//    //Add event in model
//    model.events.Add(DateTime.Now, categoryId, durationInMinutes, details);
//}
