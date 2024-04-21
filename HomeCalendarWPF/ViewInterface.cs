using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF
{
    public interface ViewInterface
    {
        void SetCalendarFilePath(string filePath);
        void ShowMessage(string message);
    }
}
