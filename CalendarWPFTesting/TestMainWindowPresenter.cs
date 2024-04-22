using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using HomeCalendarWPF;

namespace CalendarWPFTesting
{

    public class TestViewMainWindow : ViewInterface
    {
        bool calledSetCalendarFilePath = false, calledShowMessage = false, calledSetThemeLight = false, calledSetThemeDark = false;

        public void SetCalendarFilePath(string filePath)
        {
            calledSetCalendarFilePath= true;
        }

        public void SetThemeDark()
        {
            calledSetThemeDark= true;
        }

        public void SetThemeLight()
        {
            calledSetThemeLight= true;
        }

        public void ShowMessage(string message)
        {
            calledShowMessage= true;
        }
    }

    public class TestMainWindowPresenter
    {
       
    }
}
